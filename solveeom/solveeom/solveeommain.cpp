/*! \file solveeommain.cpp
    \brief 単振り子に対して運動方程式を解く関数群の実装

    Copyright © 2016-2018 @dc1394 All Rights Reserved.
    This software is released under the BSD 2-Clause License.
*/
#include "solveeommain.h"

extern "C" {
    float __stdcall gettheta()
    {
        return pse->Theta();
    }

    float __stdcall getv()
    {
        return pse->V();
    }

    void __stdcall init(float l, float r, bool resistance, bool simpleharmonic, float theta0)
    {
        pse.emplace(l, r, resistance, simpleharmonic, theta0);
    }
    
    float __stdcall kinetic_energy()
    {
        return pse->kinetic_energy();
    }
    
    float __stdcall nextstep(float dt)
    {
        return (*pse)(dt);
    }

    float __stdcall potential_energy()
    {
        return pse->potential_energy();
    }

    void __stdcall saveresult(double dt, std::string const & filename, double t)
    {
        (*pse)(dt, filename, t);
    }

    void __stdcall setfluid(std::int32_t fluid)
    {
        pse->setfluid(fluid);
    }

    void __stdcall setresistance(bool resistance)
    {
        pse->Resistance(resistance);
    }

    void __stdcall setsimpleharmonic(bool simpleharmonic)
    {
        pse->Simpleharmonic(simpleharmonic);
    }

    void __stdcall settheta(float theta)
    {
        pse->Theta(theta);
    }

    void __stdcall setv(float v)
    {
        pse->V(v);
    }
}
