/*! \file solveeommain.h
    \brief 単振り子に対して運動方程式を解く関数群の宣言

    Copyright ©  2016 @dc1394 All Rights Reserved.
    This software is released under the BSD 2-Clause License.
*/
#ifndef _SOLVEEOMMAIN_H_
#define _SOLVEEOMMAIN_H_

#ifdef __cplusplus
#define DLLEXPORT extern "C" __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllexport)
#endif

#include "solveeom.h"
#include <boost/optional.hpp>   // for boost::optional

extern "C" {
    //! A global variable.
    /*!
        SolveEOMクラスのオブジェクトへのポインタ
    */
    static boost::optional<solveeom::SolveEOM> pse;
    
    //! A global function.
    /*!
        角度θの値に対するgetter
        \return 角度θ
    */
    DLLEXPORT float __stdcall gettheta();
    
    //! A global function.
    /*!
        速度vの値に対するgetter
        \return 速度v
    */
    DLLEXPORT float __stdcall getv();

    //! A global function.
    /*!
        seオブジェクトを初期化する
        \param l ロープの長さ
        \param r 球の半径
        \param resistance 空気抵抗の有無
        \param simpleharmonic 単振動にするかどうか
        \param theta0 θの初期値
    */
    DLLEXPORT void __stdcall init(float l, float r, bool resistance, bool simpleharmonic, float theta0);

    //! A global function.
    /*!
        運動エネルギーを求める
        \return 運動エネルギー
    */
    DLLEXPORT float __stdcall kinetic_energy();

    //! A global function.
    /*!
        次のステップを計算する
        \param dt 経過した時間
        \return 新しい角度θの値
    */
    DLLEXPORT float __stdcall nextstep(float dt);

    //! A global function.
    /*!
        ポテンシャルエネルギーを求める
        \return ポテンシャルエネルギー
    */
    DLLEXPORT float __stdcall potential_energy();

    //! A global function.
    /*!
        運動方程式を、指定された時間まで積分し、その結果を時間間隔Δtごとにファイルに保存する
        \param dt 時間刻み
        \param filename 保存ファイル名
        \param t 指定時間
    */
    DLLEXPORT void __stdcall saveresult(double dt, std::string const & filename, double t);

    //! A global function.
    /*!
        流体の種類を切り替える
        \param fluid 流体の種類
    */
    DLLEXPORT void __stdcall setfluid(std::int32_t fluid);

    //! A global function.
    /*!
        空気抵抗の有無に対するsetter
        \param resistance 空気抵抗の有無
    */
    DLLEXPORT void __stdcall setresistance(bool resistance);

    //! A global function.
    /*!
        単振動にするかどうかのsetter
        \param simpleharmonic 空気抵抗の有無
    */
    DLLEXPORT void __stdcall setsimpleharmonic(bool simpleharmonic);

    //! A global function.
    /*!
        角度θの値に対するsetter
        \param theta 設定する角度θ
    */
    DLLEXPORT void __stdcall settheta(float theta);

    //! A global function.
    /*!
        速度vの値に対するsetter
        \return 設定する速度v
    */
    DLLEXPORT void __stdcall setv(float v);
}

#endif  // _SOLVEEOMMAIN_H_

