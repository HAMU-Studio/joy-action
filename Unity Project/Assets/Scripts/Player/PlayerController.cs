//for prototype

using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
//using UnityEngine.InputSystem;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{

    private static readonly Joycon.Button[] m_buttons =
      Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private Rigidbody m_Rigidbody;
    private Vector3 m_velocity;   

    //地面の上なら歩きモーション、違うなら落下モーション              

    private bool isKnockBack = false;           //ノックバック中かどうか
    private bool isFall = false;
    public bool isAttack = false;
    public bool isHit = false;   
    private bool canMove = true;
    
    //private bool isKnockBack = false;

    [Header("移動スピード")]
    [SerializeField] private float walkSpeed = 4f;
    [Header("落下速度の調整　-つける")]
    [SerializeField] float gravityPower = default!;

    //[Header("トリガーの反応タイミング")]
    //[SerializeField] float triggerTiming = 0.5f;         //トリガーがどこまで押し込まれたら反応するか 要調整 
    [Header("回転時間")]
    [SerializeField] float smoothTime = 0.3f;                //進行方向への回転にかかる時間

    //[Header("ジャンプの強さ")]
    //[SerializeField] private float jumpPower = 5f;             //ジャンプのつよさ
    [Header("ノックバックの強さ")]
    [SerializeField] private float knockBackP = 5f;                //ノックバックの強さ
    [Header("ノックバック時上方向の力")]
    [SerializeField] float knockBackUpP = 3f;            //ノックバック時少し上に浮かす


   // [SerializeField] Collider boxCollider = default!;

    [SerializeField] MainGameManager _MainGameManager;

  //  [SerializeField] ResultManager _ResultManager; //デバッグ用

    float inputHorizontal;      //水平方向の入力値
    float inputVertical;        //垂直方向の入力値
 
    float targetRotation;   //回転に使う
    float yVelocity = 0.0f;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();      
       // _MainGameManager.isInvincible = false;
        SetControllers();
    }

    void Update()
    {
        //このif文でjoycon接続の検知ができるらしい
        if (m_joycons == null || m_joycons.Count <= 0)
            return;

        Input();          
       // Attack();

    }
    private void FixedUpdate()
    {
        Gravity();
        Move();
    }

    private void Input()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.X))    //デバッグ用無敵モードon
        {
            _MainGameManager.isInvincible = true;
            Debug.Log("無敵");
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.M))    //デバッグ用無敵モードoff
        {
            _MainGameManager.isInvincible = false;
            Debug.Log("無敵解除");
        }

        float[] stick = m_joyconL.GetStick();

        //stick0が水平方向の入力値、1が垂直方向の入力値
        inputHorizontal = stick[0];
        inputVertical = stick[1];
    }

    private void OnCollisionEnter(Collision collision)
    {
        //難しい方法はできないからTriggerで判定したい
        if (isKnockBack == true)
        {
            if (collision.gameObject.CompareTag("Ground"))  //着地した時
            {              
                isKnockBack = false;
                isFall = false;
                canMove = true;
                Debug.Log("toLanding");
            }
        }

        if (_MainGameManager.isInvincible == false)
        {
            //無敵時間中はダメージを食らわない
            if (collision.gameObject.CompareTag("Enemy") )
            {
                canMove = false;
                KnockBack(collision);
                _MainGameManager.Miss();
            }
        }
    } 

    void Attack()   //ジャンプ中は攻撃できない
    {

        
    }
    void KnockBack(Collision collision)
    {   
        isKnockBack = true;
        Debug.Log("isKnockBack");
        Vector3 direction = collision.gameObject.transform.forward;

        m_Rigidbody.AddForce(-direction * knockBackP, ForceMode.Impulse);
        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //若干上方向にも飛ばす

    }

    public void fall()  //落下判定エリアで使う
    {
        isFall = true;
        canMove = false;

        //ここで操作不能にすればすれすれから復帰した時にジャンプができなくなることを防げそう
        //落下モーションへの遷移
    }

    void Move()
    {
        if (!canMove) //攻撃中は移動もジャンプもできない->returnじゃなくてその場で固定させたい
            return;
     
        if (isAttack == true)
        {
            inputHorizontal = 0;
            inputVertical = 0;
        }

        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        
        //プレイヤーの向きだとその場で回転しだす
        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;


        //移動速度の計算
        //clampは値の範囲制限
        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);   //GetAxisは0から1で入力値を管理する、斜め移動でWとAを同時押しすると
                                                                      //1以上の値が入ってくるからVector3.ClampMagnitudeメソッドを使って入力値を１に制限する(多分)

        m_velocity = clampedInput * walkSpeed;
        // transform.LookAt(m_Rigidbody.position + input); //キャラクターの向きを現在地＋入力値の方に向ける

        //Rigidbodyに一度力を加えると抵抗する力がない限りずっと力が加わる
        //AddForceに加える力をwalkSpeedで設定した速さ以上にはならないように
        //今入力から計算した速度から現在のRigidbodyの速度を引く
        m_velocity = m_velocity - m_Rigidbody.velocity;

        //　速度のXZを-walkSpeedとwalkSpeed内に収めて再設定
        m_velocity = new Vector3(Mathf.Clamp(m_velocity.x, -walkSpeed, walkSpeed), 0f, Mathf.Clamp(m_velocity.z, -walkSpeed, walkSpeed));

        if (moveForward != Vector3.zero)
        {
            //SmoothDampAngleで滑らかな回転をするためには引数（moveForwardとvelocityだけ）をVector3からfloatに変換しなければいけない

            targetRotation = Mathf.Atan2(moveForward.x, moveForward.z) * Mathf.Rad2Deg;     //Atan2, ベクトルを角度(ラジアン)に変換する Rad2Deg(radian to degrees?)ラジアンから度に変換する

            //SmoothDampAngle(現在の値, 目的の値, ref 現在の速度, 遷移時間, 最高速度); 現在の速度はnullで良いっぽい？
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref yVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        m_Rigidbody.AddForce(m_Rigidbody.mass * m_velocity / Time.fixedDeltaTime, ForceMode.Force);
        // F・・・力  
        // m・・・質量  
        // a・・・加速度
        // Δt・・・力を加えた時間 (Time.fixedDeltatime) 
        //F = ｍ * a / Δt    Forceは力を加えた時間を使って計算
    }   

    void Gravity()
    {
        //落下速度の調整用
        if (isKnockBack == true)
        {
            m_Rigidbody.AddForce(new Vector3(0, gravityPower, 0));
        }
    }

    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }
    }
    private void SetControllers()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0)
            return;

        //このへんはLINQを使用したデータ処理。用勉強。
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
        //isRightはないっぽい。節約かな
    }

}
