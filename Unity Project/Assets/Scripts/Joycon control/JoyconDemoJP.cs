using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDemoJP : MonoBehaviour
{

    private List<Joycon> joycons;

    // Unity 経由で利用可能な値
    public float[] stick; // スティック
    public Vector3 gyro; // ジャイロ
    public Vector3 accel; // アクセル
    public Quaternion orientation; // 方向
    public int jc_ind = 0; // インデックス 0、ジョイコンの接続1p, 1、ジョイコンの接続２p

    void Start()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // シーン内の JoyconManager にアタッチされた public Joycon 配列を取得
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Joycon がアタッチされている場合にのみ確認されるようにします
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];
            // GetButtonDown はボタンが押されたことを確認します（ホールドされた状態ではありません）
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("ショルダーボタン 2 が押されました");
                // GetStick は x/y ジョイスティックのコンポーネントを持つ2要素のベクトルを返します
                Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}", j.GetStick()[0], j.GetStick()[1]));

                // Joycon には磁気センサーがないため、正確なヨー値を決定することができません。 Joycon.Recenter を使用してヨー値をリセットできます。
                j.Recenter();
            }
            // GetButtonUp はボタンが離されたかどうかを確認します
            if (j.GetButtonUp(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("ショルダーボタン 2 が離されました");
            }
            // GetButton はボタンが現在押されているかどうかを確認します（押されているかホールドされているか）
            if (j.GetButton(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("ショルダーボタン 2 が押されています");
            }

            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Debug.Log("振動");

                // 低周波数が160Hz、高周波数が320Hzの振動で、200ミリ秒振動します。詳細はこちらを参照してください：
                // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

                j.SetRumble(160, 320, 0.6f, 200);

                // SetRumble の最後の引数（time）はオプションです。オフにする時間を指定せずに3つの引数で呼び出してください。
                // （振動値を動的に変更する場合に便利です。）
                // オフにするときは SetRumble(0,0,0) を呼び出します。
            }

            stick = j.GetStick();

            // Gyro 値：x、y、z 軸の値（秒あたりのラジアン）
            gyro = j.GetGyro();

            // Accel 値：x、y、z 軸の値（Gs）
            accel = j.GetAccel();

            orientation = j.GetVector();
            if (j.GetButton(Joycon.Button.DPAD_UP))
            {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            gameObject.transform.rotation = orientation;
        }
    }
}
