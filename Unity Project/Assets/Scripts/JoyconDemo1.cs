using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDemo1: MonoBehaviour
{
    private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    //public Vector3 gyro;
    public Vector3 accel;
    // jc_ind = 1:left = 0:Right
    public int jc_ind = 0;
    //public Quaternion orientation;


    private float rightTime;
    private float leftTime;
    private bool ableRightHit;
    private bool ableLeftHit;
    private bool rightFlag;
    private bool leftFlag;

    public GameObject Player;
    public float CoolTime = 0.3f;

    void Start()
    {
        //gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);

        // get the public Joycon array attached to the JoyconManager in scene
        // シーン内のジョイコンマネージャーにアタッチされているジョイコン配列を取得
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }

        ableLeftHit = true;
        ableRightHit = true;
        rightFlag = false;
        leftFlag = false;
    }

    // Update is called once per frame
    void Update()
    { 
        // make sure the Joycon only gets checked if attached
        // ジョイコンが接続されている時だけチェックする
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Accel values:  x, y, z axis values (in Gs)
            // 加速度
            accel = j.GetAccel();

            // ポジション取得
            Transform PlayerTransform = Player.transform;
            Vector3 PlayerPos = PlayerTransform.localPosition;
            Transform ArmTransform = this.transform;
            Vector3 ArmPos = ArmTransform.position;

            Rigidbody rb = this.GetComponent<Rigidbody>();
            // パンチの威力
            Vector3 force = new Vector3(0.0f, 0.0f, 0.5f);

            // Right
            if (jc_ind == 0)
            {
                if (ableRightHit)
                {
                    ArmPos.x = PlayerPos.x - 2.0f;
                    //ArmPos.y = PlayerPos.y;
                    ArmPos.z = PlayerPos.z;
                    ArmTransform.position = ArmPos;

                    if (j.GetVector().x < 0f && j.GetAccel().x < 0)
                    {
                        rightFlag = true;
                        ableRightHit = false;
                    }
                }
            }
            if (rightFlag)
            {
                rightTime += Time.deltaTime;

                rb.AddForce(force,ForceMode.Impulse);

                if (CoolTime <= rightTime)
                {
                    rightFlag = false;
                    ableRightHit = true;

                    rightTime = 0f;
                }
            }

            // Left
            if (jc_ind == 1)
            {
                if (ableLeftHit)
                {
                    ArmPos.x = PlayerPos.x + 2.0f;
                    ArmPos.y = PlayerPos.y;
                    ArmPos.z = PlayerPos.z;
                    ArmTransform.position = ArmPos;

                    if (j.GetVector().x < 0f && j.GetAccel().x < 0)
                    {
                        leftFlag = true;
                        ableLeftHit = false;
                    }
                }
            }
            if(leftFlag)
            {
                leftTime += Time.deltaTime;

                rb.AddForce(force, ForceMode.Impulse);

                if (CoolTime < leftTime)
                {
                    leftFlag = false;
                    ableLeftHit = true;

                    leftTime = 0f;
                }
            }
        }
    }

}