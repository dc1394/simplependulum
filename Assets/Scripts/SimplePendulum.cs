//-----------------------------------------------------------------------
// <copyright file="SimplePendulum.cs" company="dc1394's software">
//     Copyright ©  2016 @dc1394 All Rights Reserved.
//     This software is released under the BSD 2-Clause License.
// </copyright>
//-----------------------------------------------------------------------
namespace SimplePendulum
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 振り子クラスの実装
    /// </summary>
    internal class SimplePendulum : MonoBehaviour
    {
        #region フィールド

        /// <summary>
        /// 重力ベクトルの向き
        /// </summary>
        private static readonly Vector3 GravityDirection = Physics.gravity.normalized;

        /// <summary>
        /// ボタンの「Start」テキスト
        /// </summary>
        private static readonly String StartText = "Start";

        /// <summary>
        /// ボタンの「Stop」テキスト
        /// </summary>
        private static readonly String StopText = "Stop";

        /// <summary>
        /// ロープの重心のスケール
        /// </summary>
        [SerializeField]
        private float CenterOfGarvityForRopeScale = 0.4f;

        /// <summary>
        /// 球の質量
        /// </summary>
        [SerializeField]
        private float Mass = 1.0f;

        /// <summary>
        /// 原点の座標
        /// </summary>
        [SerializeField]
        private Vector3 Origin = Vector3.zero;

        /// <summary>
        /// 前回のフレームで取得した時間
        /// </summary>
        private float previousTime = 0.0f;

        /// <summary>
        /// 球の半径
        /// </summary>
        [SerializeField]
        private float Radius = 0.1f;

        /// <summary>
        /// ボタンのテキスト
        /// </summary>
        private String buttontext = SimplePendulum.StartText;

        /// <summary>
        /// 実行中かどうかを示すフラグ
        /// </summary>
        private bool exec = false;

        /// <summary>
        /// 空気抵抗のチェックボックス
        /// </summary>
        private bool resistance = true;

        /// <summary>
        /// ロープオブジェクト
        /// </summary>
        [SerializeField]
        private GameObject Rope = null;

        /// <summary>
        /// ロープの長さ
        /// </summary>
        private float ropeLength;

        /// <summary>
        /// 流体の種類を示すラジオボタンの項目
        /// </summary>
        private String[] selCaptions = new String[] { "空気", "水" };

        /// <summary>
        /// 流体の種類を示すラジオボタンの値
        /// </summary>
        private Int32 selGridInt = 0;

        /// <summary>
        /// 単振動にするかどうかのチェックボックス
        /// </summary>
        private bool simpleharmonic = false;

        /// <summary>
        /// 球オブジェクト
        /// </summary>
        [SerializeField]
        private GameObject Sphere = null;

        /// <summary>
        /// 角度
        /// </summary>
        private float thetadeg;

        /// <summary>
        /// 速度
        /// </summary>
        private float velocity = 0.0f;

        #endregion フィールド

        #region メソッド

        /// <summary>
        /// 原点から測定した、球の座標と重力ベクトルの成す角度を与える関数
        /// </summary>
        /// <returns>球の座標と重力ベクトルの成す角度</returns>
        private float GetSphereAndGravityAngle()
        {
            return Vector3.Angle(this.Sphere.transform.position - this.Origin, SimplePendulum.GravityDirection);
        }

        /// <summary>
        /// 原点から測定した、球の座標のz座標を与える関数
        /// </summary>
        /// <returns>球の座標のz座標</returns>
        private float GetSpherePosZ()
        {
            return (this.Sphere.transform.position - this.Origin).z;
        }

        /// <summary>
        /// GUIイベントの処理
        /// </summary>
        private void OnGUI()
        {
            var ypos = 20;

            // ラベルに角度θの値を表示する
            var angle = this.GetSphereAndGravityAngle();
            GUI.Label(
                new Rect(20.0f, (float)ypos, 160.0f, 20.0f),
                String.Format("角度θ = {0:F3}°", this.GetSpherePosZ() > 0.0f ? angle : -angle));

            ypos += 20;

            // ラベルに速度vの値を表示する
            var vel = this.ropeLength * Solveeomcs.SolveEOMcs.GetV();
            GUI.Label(
                new Rect(20.0f, (float)ypos, 160.0f, 20.0f),
                String.Format("速度v = {0:F3}(m/s)", vel));

            ypos += 20;

            // ラベルに運動エネルギーの値を表示する
            var kinetic = Solveeomcs.SolveEOMcs.Kinetic_Energy();
            GUI.Label(
                new Rect(20.0f, (float)ypos, 170.0f, 20.0f),
                String.Format("運動エネルギー = {0:F3}(J)", kinetic));

            ypos += 20;

            // ラベルにポテンシャルエネルギーの値を表示する
            var potential = Solveeomcs.SolveEOMcs.Potential_Energy();
            GUI.Label(
                new Rect(20.0f, (float)ypos, 170.0f, 20.0f),
                String.Format("ポテンシャル = {0:F3}(J)", potential));

            ypos += 20;

            // ラベルに全エネルギーの値を表示する
            GUI.Label(
                new Rect(20.0f, (float)ypos, 170.0f, 20.0f),
                String.Format("全エネルギー = {0:F3}(J)", kinetic + potential));

            var ypos2 = 20;

            // 単振動にするかどうかのチェックボックスを表示する
            var sincurvbefore = this.simpleharmonic;
            this.simpleharmonic = GUI.Toggle(new Rect(200.0f, (float)ypos2, 150.0f, 20.0f), this.simpleharmonic, "単振動");
            if (sincurvbefore != this.simpleharmonic)
            {
                Solveeomcs.SolveEOMcs.SetSimpleharmonic(this.simpleharmonic);
            }

            ypos2 += 20;

            // 空気抵抗のチェックボックスを表示する
            var resbefore = this.resistance;
            this.resistance = GUI.Toggle(new Rect(200.0f, (float)ypos2, 150.0f, 20.0f), this.resistance, "流体の抵抗あり");
            if (resbefore != this.resistance)
            {
                Solveeomcs.SolveEOMcs.SetResistance(this.resistance);
            }

            ypos2 += 25;

            // 「角度θ」と表示する
            GUI.Label(new Rect(200.0f, (float)ypos2, 100.0f, 20.0f), "角度θ");

            ypos2 += 20;

            // 角度を変更するスライダーを表示する
            var thetadegbefore = this.thetadeg;
            this.thetadeg = GUI.HorizontalSlider(new Rect(200.0f, (float)ypos2, 100.0f, 20.0f), this.thetadeg, -180.0f, 180.0f);
            if (Mathf.Abs(this.thetadeg - thetadegbefore) > Mathf.Epsilon)
            {
                var theta = Mathf.Deg2Rad * this.thetadeg;
                Solveeomcs.SolveEOMcs.SetTheta(theta);
                this.SphereRotate(theta);
                this.RopeUpdate();
            }

            ypos2 += 20;

            // 「速度v」と表示する
            GUI.Label(new Rect(200.0f, (float)ypos2, 100.0f, 20.0f), "速度v");

            ypos2 += 20;

            // 速度を変更するスライダーを表示する
            var velocitybefore = this.velocity;
            this.velocity = GUI.HorizontalSlider(new Rect(200.0f, (float)ypos2, 100.0f, 20.0f), this.velocity, -10.0f, 10.0f);
            if (Mathf.Abs(this.velocity - velocitybefore) > Mathf.Epsilon)
            {
                Solveeomcs.SolveEOMcs.SetV(this.velocity / this.ropeLength);
            }

            ypos2 += 20;

            GUILayout.BeginArea(new Rect(200.0f, ypos2, 200.0f, ypos2));
            GUILayout.BeginVertical();

            var selgridintbefore = this.selGridInt;
            this.selGridInt = GUILayout.SelectionGrid(this.selGridInt, this.selCaptions, 1, "Toggle");
            if (selgridintbefore != this.selGridInt)
            {
                Solveeomcs.SolveEOMcs.SetFluid(this.selGridInt);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            var ypos3 = 20.0f;

            if (GUI.Button(new Rect(320.0f, (float)ypos3, 110.0f, 20.0f), this.buttontext))
            {
                if (this.exec)
                {
                    this.exec = false;
                    this.buttontext = SimplePendulum.StartText;
                }
                else
                {
                    this.exec = true;
                    this.buttontext = SimplePendulum.StopText;
                }
            }
        }

        /// <summary>
        /// ロープの座標と角度を更新する
        /// </summary>
        private void RopeUpdate()
        {
            // ロープの座標を更新
            this.Rope.transform.position = new Vector3(
                0.0f,
                this.CenterOfGarvityForRopeScale * this.Sphere.transform.position.y,
                this.CenterOfGarvityForRopeScale * this.Sphere.transform.position.z);

            // ロープの角度を初期化
            this.Rope.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            // 角度が正かどうか
            if (this.GetSpherePosZ() > 0.0f)
            {
                this.Rope.transform.Rotate(new Vector3(-this.GetSphereAndGravityAngle(), 0.0f, 0.0f));
            }
            else
            {
                this.Rope.transform.Rotate(new Vector3(this.GetSphereAndGravityAngle(), 0.0f, 0.0f));
            }
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Start()
        {
            this.PendulumInit();
        }

        /// <summary>
        /// 球の角度を更新する
        /// </summary>
        /// <param name="theta">新しい角度</param>
        private void SphereRotate(float theta)
        {
            // rcosθの計算
            var rcostheta = this.ropeLength * Mathf.Cos(theta);

            // rsinθの計算
            var rsintheta = this.ropeLength * Mathf.Sin(theta);

            // 球の角度を更新
            this.Sphere.transform.position = new Vector3(
                0.0f,
                (rsintheta * SimplePendulum.GravityDirection.z) + (rcostheta * SimplePendulum.GravityDirection.y),
                (rcostheta * SimplePendulum.GravityDirection.z) - (rsintheta * SimplePendulum.GravityDirection.y));
        }

        /// <summary>
        /// フレーム処理
        /// </summary>
        private void Update()
        {
            // 時間差を取得
            var frameTime = Time.time - this.previousTime;

            // 新しく取得した時間を代入
            this.previousTime = Time.time;

            if (this.exec)
            {
                // 球の座標を更新
                this.SphereUpdate(frameTime);

                // ロープの座標と角度を更新
                this.RopeUpdate();
            }
        }

        /// <summary>
        /// 振り子の状態を初期化する
        /// </summary>
        private void PendulumInit()
        {
            this.ropeLength = Vector3.Distance(this.Origin, this.Sphere.transform.position);

            this.thetadeg = this.GetSphereAndGravityAngle();
            if (this.GetSpherePosZ() < 0.0f)
            {
                this.thetadeg *= -1.0f;
            }

            Solveeomcs.SolveEOMcs.Init(
                this.ropeLength,
                this.Mass,
                this.Radius,
                this.resistance,
                this.simpleharmonic,
                Mathf.Deg2Rad * this.thetadeg);
        }

        /// <summary>
        /// 球の状態を更新する
        /// </summary>
        /// <param name="frameTime">経過時間</param>
        private void SphereUpdate(float frameTime)
        {
            // 運動方程式を解いてθを求める
            var theta = Solveeomcs.SolveEOMcs.NextStep(frameTime);

            // 球の角度を更新
            this.SphereRotate(theta);
        }

        #endregion メソッド
    }
}
