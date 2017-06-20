/*! \file solveeom.cpp
    \brief 単振り子に対して運動方程式を解くクラスの実装

    Copyright ©  2016 @dc1394 All Rights Reserved.
    This software is released under the BSD 2-Clause License.
*/
#include "solveeom.h"
#include <cmath>                                // for std::sin, std::cos
#include <fstream>                              // for std::ofstream
#include <boost/assert.hpp>                     // for BOOST_ASSERT
#include <boost/format.hpp>                     // for boost::format
#include <boost/math/constants/constants.hpp>   // for boost::math::constants::pi

namespace solveeom {
    // #region コンストラクタ・デストラクタ

    SolveEOM::SolveEOM(float l, float r, bool resistance, bool simpleharmonic, float theta0) :
        Resistance(nullptr, [this](auto resistance) { return resistance_ = resistance; }),
		Simpleharmonic(nullptr, [this](auto simpleharmonic) { return simpleharmonic_ = simpleharmonic; }),
        Theta([this] { return static_cast<float>(x_[0]); }, [this](auto theta) { return x_[0] = theta; }),
        V([this] { return static_cast<float>(x_[1]); }, [this](auto v) { return x_[1] = v; }),
        l_(l),
        r_(r),
        m_(4.0 / 3.0 * boost::math::constants::pi<double>() * r * r * r * SolveEOM::ALUMINIUMRHO),
        myu_(SolveEOM::AIRMYU),     // 空気の粘度
        resistance_(resistance),    // 空気抵抗
        simpleharmonic_(simpleharmonic),
        rho_(SolveEOM::AIRRHO),     // 空気の密度
        stepper_(SolveEOM::EPS, SolveEOM::EPS)
    {
        nyu_ = myu_ / rho_;
        x_ = { theta0, 0.0 };
    }

    // #endregion コンストラクタ・デストラクタ

    // #region publicメンバ関数

    float SolveEOM::kinetic_energy() const
    {
        return static_cast<float>(0.5 * m_ * sqr(l_ * x_[1]));
    }

    float SolveEOM::operator()(float dt)
    {
        boost::numeric::odeint::integrate_adaptive(
            stepper_,
            getEOM(),
            x_,
            0.0,
            static_cast<double>(dt),
            SolveEOM::DX);

        return static_cast<float>(x_[0]);
    }

    void SolveEOM::operator()(double dt, std::string const & filename, double t)
    {
        std::ofstream result(filename);

        boost::numeric::odeint::integrate_const(
            stepper_,
            getEOM(),
            x_,
            0.0,
            t,
            dt,
            [&result, this](auto const & x, auto const t)
            {
				result << boost::format("%.3f, %.15f, %.15f\n") % t % x[0] % total_energy();
            });
    }

    float SolveEOM::potential_energy() const
    {
        return static_cast<float>(m_ * SolveEOM::g * l_ * (1.0f - std::cos(x_[0])));
    }


    void SolveEOM::setfluid(std::int32_t fluid)
    {
        switch (static_cast<SolveEOM::Fluid_type>(fluid)) {
        case SolveEOM::Fluid_type::AIR:
            myu_ = SolveEOM::AIRMYU;
            rho_ = SolveEOM::AIRRHO;
            break;

        case SolveEOM::Fluid_type::WATER:
            myu_ = SolveEOM::WATERMYU;
            rho_ = SolveEOM::WATERRHO;
            break;

        default:
            BOOST_ASSERT(!"ここに来てはいけない!");
            break;
        }

        nyu_ = myu_ / rho_;
    }

    // #endregion publicメンバ関数

    // #region privateメンバ関数

	std::function<void(SolveEOM::state_type const &, SolveEOM::state_type &, double const)> SolveEOM::getEOM() const
    {
        auto const eom = [this](state_type const & x, state_type & dxdt, double const)
        {
            // dθ/dt = v / l
            dxdt[0] = x[1];

            // 振り子に働く力
            double f1;
            if (simpleharmonic_) {
                f1 = -SolveEOM::g * x[0] / l_;
            }
            else {
                f1 = -SolveEOM::g * std::sin(x[0]) / l_;
            }

            // 空気抵抗を有効にしていなかったら
            if (!resistance_) {
                dxdt[1] = f1;
                return;
            }

            // レイノルズ数
            auto const Re = 2.0 * r_ * std::fabs(l_ * x[1]) / nyu_;

            // 粘性抵抗
            auto const F = 6.0 * boost::math::constants::pi<double>() * myu_ * r_ * l_ * x[1];

            // 粘性抵抗のみ
            if (Re < SolveEOM::THRESHOLD) {
                dxdt[1] = f1 - F / (m_ * l_);

                return;
            }

            auto const FD = 0.5 * rho_ * boost::math::constants::pi<double>() * sqr(r_ * (l_ * x[1]));

            // Drag coefficient
            double CD;

            // N.-S. Cheng, Comparison of formulas for drag coefficient and settling velocity of
            // spherical particles, Powder Technology 189 (2009) 395–398.
            if (Re <= 3000) {
                CD = 24.0 / Re * std::pow(1.0 + 0.27 * Re, 0.43) + 0.47 * (1.0 - std::exp(-0.04 * std::pow(Re, 0.38)));
            }
            else {
                // Re > 3000
                // Almedeij J. Drag coefficient of flow around a sphere: Matching asymptotically the wide
                // trend. std::powder Technology. (2008);doi:10.1016/j.std::powtec.2007.12.006.
                auto const phi1 = std::pow(24.0 / Re, 10) + std::pow(21.0 * std::pow(Re, -0.67), 10) +
                    std::pow(4.0 * std::pow(Re, -0.33), 10) + std::pow(0.4, 10);
                auto const phi2 = 1.0 / (1.0 / std::pow(0.148 * std::pow(Re, 0.11), 10) + 1.0 / std::pow(0.5, 10));
                auto const phi3 = std::pow((1.57E+8) * std::pow(Re, -1.625), 10);
                auto const phi4 = 1.0 / (1.0 / std::pow((6.0E-17) * std::pow(Re, 2.63), 10) + 1.0 / std::pow(0.2, 10));

                CD = std::pow((1.0 / (1.0 / (phi1 + phi2) + 1.0 / phi3) + phi4), 0.1);
            }

            // 慣性抵抗÷(m×l)
            auto const f2 = (x[1] >= 0.0) ? -FD * CD / (m_ * l_) : FD * CD / (m_ * l_);
            dxdt[1] = f1 + f2 - F / (m_ * l_);
        };

        return eom;
    }
	
	double SolveEOM::total_energy() const
	{
		auto const kinetic = 0.5 * m_ * sqr(l_ * x_[1]);
		auto const potential = m_ * SolveEOM::g * l_ * (1.0 - std::cos(x_[0]));

		return kinetic + potential;
	}

    // #endregion privateメンバ関数
}