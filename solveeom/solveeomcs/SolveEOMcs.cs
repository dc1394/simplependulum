//-----------------------------------------------------------------------
// <copyright file="SolveEOMcs.cs" company="dc1394's software">
//     Copyright © 2016-2018 @dc1394 All Rights Reserved.
//     This software is released under the BSD 2-Clause License.
// </copyright>
//-----------------------------------------------------------------------
namespace Solveeomcs
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// C++で書かれたSolveEoMクラスをC#からアクセスするためのラッパークラス
    /// </summary>
    public sealed class SolveEoMcs
    {
        #region メソッド

        /// <summary>
        /// 角度θの値に対するgetter
        /// </summary>
        /// <returns>角度θ</returns>
        [DllImport("solveeom", EntryPoint = "gettheta")]
        public static extern float GetTheta();

        /// <summary>
        /// 速度vの値に対するgetter
        /// </summary>
        /// <returns>速度v</returns>
        [DllImport("solveeom", EntryPoint = "getv")]
        public static extern float GetV();

        /// <summary>
        /// seオブジェクトを初期化する
        /// </summary>
        /// <param name="l">ロープの長さ</param>
        /// <param name="r">球の半径</param>
        /// <param name="resistance">空気抵抗の有無</param>
        /// <param name="simpleharmonic">単振動にするかどうか</param>
        /// <param name="theta0">θの初期値</param>
        [DllImport("solveeom", EntryPoint = "init")]
        public static extern void Init(float l, float r, bool resistance, bool simpleharmonic, float theta0);

        /// <summary>
        /// 運動エネルギーを求める
        /// </summary>
        /// <returns>運動エネルギー</returns>
        [DllImport("solveeom", EntryPoint = "kinetic_energy")]
        public static extern float Kinetic_Energy();

        /// <summary>
        /// 次のステップを計算する
        /// </summary>
        /// <param name="dt">経過した時間</param>
        /// <returns>新しい角度θの値</returns>
        [DllImport("solveeom", EntryPoint = "nextstep")]
        public static extern float NextStep(float dt);

        /// <summary>
        /// ポテンシャルエネルギーを求める
        /// </summary>
        /// <returns>ポテンシャルエネルギー</returns>
        [DllImport("solveeom", EntryPoint = "potential_energy")]
        public static extern float Potential_Energy();

        /// <summary>
        /// 流体の種類を切り替える
        /// </summary>
        /// <param name="fluid">流体の種類</param>
        [DllImport("solveeom", EntryPoint = "setfluid")]
        public static extern void SetFluid(Int32 fluid);

        /// <summary>
        /// 空気抵抗の有無に対するsetter
        /// </summary>
        /// <param name="resistance">設定する角度θ</param>
        [DllImport("solveeom", EntryPoint = "setresistance")]
        public static extern void SetResistance(bool resistance);

        /// <summary>
        /// 単振動にするかどうかのsetter
        /// </summary>
        /// <param name="simpleharmonic">単振動にするかどうか</param>
        [DllImport("solveeom", EntryPoint = "setsimpleharmonic")]
        public static extern void SetSimpleharmonic(bool simpleharmonic);

        /// <summary>
        /// 角度θの値に対するsetter
        /// </summary>
        /// <param name="theta">設定する角度θ</param>
        [DllImport("solveeom", EntryPoint = "settheta")]
        public static extern void SetTheta(float theta);

        /// <summary>
        /// 速度vの値に対するsetter
        /// </summary>
        /// <param name="v">設定する速度v</param>
        [DllImport("solveeom", EntryPoint = "setv")]
        public static extern void SetV(float v);

        #endregion メソッド
    }
}