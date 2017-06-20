//-----------------------------------------------------------------------
// <copyright file="CameraController.cs" company="@TAK-EMI">
//     Unityでメタセコイアのようにカメラを操作できるようにするためのスクリプト
//     ( https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d )
//     Copyright ©  2015 @TAK-EMI All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

// クラス名が被っているといけないので、namespaceで囲む
namespace TAK_CameraController
{
    using System;
    using UnityEngine;

    /// <summary>
    /// マウスのボタンをあらわす番号がわかりにくかったので名前を付けた
    /// </summary>
    internal enum MouseButtonDown
    {
        /// <summary>
        /// マウスの左ボタン
        /// </summary>
        MBD_LEFT = 0,

        /// <summary>
        /// マウスの右ボタン
        /// </summary>
        MBD_RIGHT,

        /// <summary>
        /// マウスのスクロールボタン
        /// </summary>
        MBD_MIDDLE
    }

    /// <summary>
    /// メタセコイアのようにカメラを操作できるようにするためのクラス
    /// </summary>
    internal class CameraController : MonoBehaviour
    {
        #region フィールド
        
        /// <summary>
        /// 注視点となるオブジェクト
        /// </summary>
        [SerializeField]    // privateなメンバもインスペクタで編集したいときに付ける
        private GameObject focusObj = null;

        /// <summary>
        /// マウスの位置を保存する変数
        /// </summary>
        private Vector3 oldPos;

        #endregion フィールド

        #region メソッド

        /// <summary>
        /// カメラを回転する関数
        /// </summary>
        /// <param name="eulerAngle">カメラの回転する角度（オイラー角）</param>
        private void CameraRotate(Vector3 eulerAngle)
        {
            var focusPos = this.focusObj.transform.position;
            var trans = this.transform;

            // 回転前のカメラの情報を保存する
            var preUpV = trans.up;
            var preAngle = trans.localEulerAngles;
            var prePos = trans.position;

            // カメラの回転
            // 横方向の回転はグローバル座標系のY軸で回転する
            trans.RotateAround(focusPos, Vector3.up, eulerAngle.y);

            // 縦方向の回転はカメラのローカル座標系のX軸で回転する
            trans.RotateAround(focusPos, trans.right, -eulerAngle.x);

            // カメラを注視点に向ける
            trans.LookAt(focusPos);

            // ジンバルロック対策
            // カメラが真上や真下を向くとジンバルロックがおきる
            // ジンバルロックがおきるとカメラがぐるぐる回ってしまうので、一度に90度以上回るような計算結果になった場合は回転しないようにする(計算を元に戻す)
            var up = trans.up;
            if (Vector3.Angle(preUpV, up) > 90.0f)
            {
                trans.localEulerAngles = preAngle;
                trans.position = prePos;
            }

            return;
        }

        /// <summary>
        /// カメラを移動する関数
        /// </summary>
        /// <param name="vec">カメラの移動量ベクトル</param>
        private void CameraTranslate(Vector3 vec)
        {
            var focusTrans = this.focusObj.transform;
            var trans = this.transform;

            // カメラのローカル座標軸を元に注視点オブジェクトを移動する
            focusTrans.Translate((trans.right * -vec.x) + (trans.up * vec.y));

            return;
        }

        /// <summary>
        /// 注視点オブジェクトが未設定の場合、新規に生成する
        /// </summary>
        /// <param name="name">注視点オブジェクトの名前</param>
        private void SetupFocusObject(String name)
        {
            var obj = this.focusObj = new GameObject(name);
            obj.transform.position = Vector3.zero;

            return;
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Start()
        {
            // 注視点オブジェクトの有無を確認
            if (this.focusObj == null)
            {
                this.SetupFocusObject("CameraFocusObject");
            }

            // 注視点オブジェクトをカメラの親にする
            var trans = this.transform;
            transform.parent = this.focusObj.transform;

            // カメラの方向ベクトル(ローカルのZ方向)を注視点オブジェクトに向ける
            trans.LookAt(this.focusObj.transform.position);

            return;
        }

        /// <summary>
        /// フレーム処理
        /// </summary>
        private void Update()
        {
            // マウス関係のイベントを関数にまとめる
            this.MouseEvent();

            return;
        }

        /// <summary>
        /// マウスドラッグイベント関数
        /// </summary>
        /// <param name="mousePos">マウスの現在の位置</param>
        private void MouseDragEvent(Vector3 mousePos)
        {
            // マウスの現在の位置と過去の位置から差分を求める
            var diff = mousePos - this.oldPos;

            // 差分の長さが極小数より小さかったら、ドラッグしていないと判断する
            if (diff.magnitude < Vector3.kEpsilon)
            {
                return;
            }

            if (Input.GetMouseButton((int)MouseButtonDown.MBD_LEFT))
            {
                // マウス左ボタンをドラッグした場合(なにもしない)
            }
            else if (Input.GetMouseButton((int)MouseButtonDown.MBD_MIDDLE))
            {
                // マウス中ボタンをドラッグした場合(注視点の移動)
                this.CameraTranslate(-diff / 10.0f);
            }
            else if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
            {
                // マウス右ボタンをドラッグした場合(カメラの回転)
                this.CameraRotate(new Vector3(diff.y, diff.x, 0.0f));
            }

            // 現在のマウス位置を、次回のために保存する
            this.oldPos = mousePos;

            return;
        }

        /// <summary>
        /// マウス関係のイベント
        /// </summary>
        private void MouseEvent()
        {
            // マウスホイールの回転量を取得
            var delta = Input.GetAxis("Mouse ScrollWheel");
            
            // 回転量が0でなければホイールイベントを処理
            if (delta != 0.0f)
            {
                this.MouseWheelEvent(delta);
            }

            // マウスボタンが押されたタイミングで、マウスの位置を保存する
            if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
            Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
            Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
            {
                this.oldPos = Input.mousePosition;
            }

            // マウスドラッグイベント
            this.MouseDragEvent(Input.mousePosition);

            return;
        }
                
        /// <summary>
        /// マウスホイールイベント
        /// </summary>
        /// <param name="delta">マウスホイールの量</param>
        private void MouseWheelEvent(float delta)
        {
            var value = 1.0f - (0.8f * delta);
            this.transform.position *= value;
        }

        #endregion メソッド
    }
}
