////古澤
//using Cinemachine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Experimental.GlobalIllumination;
//using UnityEngine.InputSystem;
//using UnityEngine.Windows;
////using static UnityEditor.Experimental.GraphView.GraphView;

//public class PlayerController : MonoBehaviour
//{
//    private Rigidbody m_Rigidbody;
//    private Vector3 velocity;

//    Animator animator;
//    AnimatorStateInfo stateInfo;

//    //地面の上なら歩きモーション、違うなら落下モーション              

//    private bool isJump = false;           //ジャンプ中かどうか
//    private bool isFall = false;
//    public bool isAttack = false;
//    public bool isHit = false;
//    private bool isDidHit = false;
//    private bool canMove = true;
//    private bool onlyFirst = false;
//    //private bool isKnockBack = false;

//    [Header("移動スピード")]
//    [SerializeField] private float walkSpeed = 4f;
//    [Header("落下速度の調整　-つける")]
//    [SerializeField] float gravityPower = default!;
//    [Header("攻撃モーションの長さ")]
//    [SerializeField] float Attack_Finish_Time = 0.3f;    //攻撃モーションの長さに応じて変える (連打とTriggerによる連続入力を防ぐため)
//    [Header("攻撃モーションのスピード")]
//    [SerializeField] float Attack_Motion_Speed = 1.0f;
//    [Header("判定が現れる時間")]
//    [SerializeField] float Collider_Start_Time = 0.1f;
//    [Header("判定が消える時間")]
//    [SerializeField] float Collider_Stop_Time = 0.3f;
//    [Header("パーティクルが現れる時間")]
//    [SerializeField] float Particle_Start_Time = 0.1f;
//    [Header("パーティクルが消える時間")]
//    [SerializeField] float Particle_Stop_Time = 0.3f;


//    [Header("トリガーの反応タイミング")]
//    [SerializeField] float triggerTiming = 0.5f;         //トリガーがどこまで押し込まれたら反応するか 要調整 
//    [Header("回転時間")]
//    [SerializeField] float smoothTime = 0.3f;                //進行方向への回転にかかる時間
//    [Header("ジャンプの強さ")]
//    [SerializeField] private float jumpPower = 5f;             //ジャンプのつよさ
//    [Header("ノックバックの強さ")]
//    [SerializeField] private float knockBackP = 5f;                //ノックバックの強さ
//    [Header("ノックバック時上方向の力")]
//    [SerializeField] float knockBackUpP = 3f;            //ノックバック時少し上に浮かす
//    [Header("パーティクル")]
//    [Tooltip("1ヒット、2素振り、3ジャンプ")]
//    [SerializeField] private ParticleSystem[] particles = default!;
//    [Header("だるまに当たった時のエフェクトを再生するやつ")]
//    [SerializeField] private ParticlePlayer hitParticlePlayer;

//    [SerializeField] Collider boxCollider = default!;
//    [SerializeField] GameObject CinemachineCameraTarget;    //カメラのターゲットを別オブジェクトにすることで頭の部分を追尾

//    [SerializeField] AudioClip jumpS = default!;
//    [SerializeField] AudioClip attack_true_S = default!;
//    [SerializeField] AudioClip fallS = default!;
//    [Header("当てた時の音")]
//    [SerializeField] AudioClip hitS = default!;
//    [Header("被ダメ音")]
//    [SerializeField] AudioClip damagedS = default!;
//    // [SerializeField]  AudioClip cameraResetS = default!;

//    [SerializeField] MainGameManager _MainGameManager;

//    [SerializeField] ResultManager _ResultManager; //デバッグ用

//    float inputHorizontal;      //水平方向の入力値
//    float inputVertical;        //垂直方向の入力値
//    //float L_inputTrigger;
//    float R_inputTrigger;
//    bool inputAttack;

//    bool R_isReset;
//    //  bool L_isReset;
//    float targetRotation;   //回転に使う
//    float yVelocity = 0.0f;
//    float motionTime = 0.0f;     // 攻撃モーションが始まってからの経過時間を格納->animationの遷移でできそう 
//    float time = 0f;                //Runningモーションに使う

//    //Quaternion defaultCameraRot;
//    //[SerializeField] CinemachineFreeLook _freeLookCamera;
//    //[SerializeField] Camera _camera;


//    void Start()
//    {
//        m_Rigidbody = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();
//        _MainGameManager.isInvincible = false;
//        //defaultCameraRot = Camera.main.transform.rotation;
//        animator.SetTrigger("toIdle");
//        particles[0].Stop();
//        particles[1].Stop();
//    }

//    void Update()
//    {
//        Input();
//        // CamaraReset();
//        Jump();
//        Attack();
//        transitionAnim();
//        ParticleManage();
//        if (isAttack)
//        {
//            AttackMotionManage();
//        }
//    }
//    private void FixedUpdate()
//    {
//        Gravity();

//        //if (stateInfo.IsName("Running") || stateInfo.IsName("Jumping")) //たまになぜか滑るからなし
//        Move();
//    }

//    private void Input()
//    {
//        if (UnityEngine.Input.GetKeyDown(KeyCode.X))    //デバッグ用無敵モードon
//        {
//            _MainGameManager.isInvincible = true;
//            Debug.Log("無敵");
//        }
//        if (UnityEngine.Input.GetKeyDown(KeyCode.M))    //デバッグ用無敵モードoff
//        {
//            _MainGameManager.isInvincible = false;
//            Debug.Log("無敵解除");
//        }

//        // 入力不可なら終わり
//        if (GameManager.instance.ReturnInputState() != InputState.OnInput)
//        {
//            inputHorizontal = 0;
//            inputVertical = 0;
//            animator.ResetTrigger("toIdle");
//            return;
//        }

//        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

//        inputHorizontal = UnityEngine.Input.GetAxisRaw("Horizontal");   //入力値の格納
//        inputVertical = UnityEngine.Input.GetAxisRaw("Vertical");
//        // L_inputTrigger = UnityEngine.Input.GetAxis("L_Trigger");
//        R_inputTrigger = UnityEngine.Input.GetAxis("R_Trigger");
//        inputAttack = UnityEngine.Input.GetButtonDown("Attack");


//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        //難しい方法はできないからTriggerで判定したい
//        if (isJump == true || isFall == true)
//        {
//            if (collision.gameObject.CompareTag("Ground"))  //着地した時
//            {
//                animator.Play("Landing");
//                isJump = false;
//                isFall = false;
//                canMove = true;
//                Debug.Log("toLanding");
//            }
//        }

//        if (_MainGameManager.isInvincible == false)
//        {
//            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DarumaBall") || collision.gameObject.CompareTag("Ball"))     //無敵時間中はダメージを食らわない
//            {
//                canMove = false;
//                KnockBack(collision);
//                _MainGameManager.Miss();
//            }
//        }
//    }

//    void ParticleManage()
//    {
//        if (isHit == true && isDidHit == false)      //ヒット時のエフェクト再生
//        {
//            //particles[0].Play();
//            hitParticlePlayer.Play();
//            isDidHit = true;
//        }
//        if (isHit == false && isDidHit == true)
//        {
//            isDidHit = false;
//        }
//        //if (stateInfo.IsName("Attacking") && particles[1].isPlaying == true)
//        //{
//        //    particles[1].Stop();    //ハンマーを後ろに構える時間はパーティクル停止
//        //}
//        //if (stateInfo.IsName("Running") || stateInfo.IsName("Idle"))
//        //{
//        //    if (particles[1].isPlaying == true)
//        //        particles[1].Stop();
//        //}
//    }
//    void transitionAnim()
//    {
//        if (stateInfo.IsName("Jumping") ||     //移動モーションの遷移
//           stateInfo.IsName("Attacking"))
//            return;

//        if (inputHorizontal != 0 || inputVertical != 0)
//        {
//            //入力が0じゃなければ移動モーションに遷移
//            time += Time.deltaTime;
//            animator.ResetTrigger("toIdle");
//        }
//        else if (inputHorizontal == 0 && inputVertical == 0 && stateInfo.IsName("Running") && isJump == false) //isJumpの条件を入れないとたまにジャンプモーションがキャンセルされる
//        {
//            time = 0f;
//            animator.SetTrigger("toIdle");
//        }
//        //  Debug.Log("time = " + time);
//        animator.SetFloat("time", time);
//    }

//    void AttackMotionManage()
//    {
//        //攻撃モーション中ハンマーの当たり判定が存在する時間の管理

//        motionTime += Time.deltaTime * Attack_Motion_Speed;
//        Attack_Colider_Manage();
//        Attack_Particle_Manage();
//    }

//    void Attack_Colider_Manage()
//    {
//        if (isHit && onlyFirst == false)
//        {
//            GameManager.instance.PlaySE(hitS);
//            boxCollider.enabled = false;
//            onlyFirst = true;
//        }
//        // checkHit();
//        if (motionTime >= Collider_Start_Time && motionTime < Collider_Stop_Time && boxCollider.enabled == false)  //一定時間経過で判定出現(振り始めの一瞬は当たらない)
//        {
//            boxCollider.enabled = true;
//            //Debug.Log("コライダー" + boxCollider.enabled);
//        }
//        if (motionTime >= Collider_Stop_Time && boxCollider.enabled == true)   //一定時間経過で判定が消える(振り切った最後の方は当たらない)
//        {
//            boxCollider.enabled = false;
//            // Debug.Log("コライダー" + boxCollider.enabled);
//        }
//        if (motionTime >= Attack_Finish_Time && isAttack == true)
//        {
//            isHit = false;
//            isAttack = false;
//            onlyFirst = false;
//            motionTime = 0.0f;
//        }
//    }
//    void Attack_Particle_Manage()       //ハンマーの当たり判定が存在する時間とパーティクルが存在する時間は分ける
//    {
//        if (motionTime >= Particle_Start_Time && motionTime < Particle_Stop_Time && particles[1].isStopped == true)  //一定時間経過でパーティクル出現
//        {
//            particles[1].Play();    //振り始めは再生　調整するかも               
//        }
//        if (motionTime >= Particle_Stop_Time && particles[1].isPlaying == true)   //一定時間経過でパーティクルが消える
//        {
//            particles[1].Stop(); //振り終わったら停止                   
//        }
//    }
//    //void CamaraReset()    //カメラリセット機能つけたかったが断念
//    //{
//    //    if (L_inputTrigger == 0)
//    //    {
//    //        L_isReset = true;
//    //        return;
//    //    }
//    //    if (L_isReset == false)
//    //     return; 

//    //    if (L_inputTrigger > triggerTiming)
//    //    {
//    //        Debug.Log("Reset");
//    //        // Camera.main.transform.position = this.transform.forward;
//    //        _freeLookCamera.ForceCameraPosition(this.transform.forward, defaultCameraRot);
//    //        Camera.main.transform.rotation = defaultCameraRot;
//    //        Camera.main.transform.position = this.transform.forward;             
//    //        _freeLookCamera.m_YAxis.Value = 0.5f;
//    //        _freeLookCamera.m_XAxis.Value = 0;


//    //        //_camera.transform.position =  this.transform.forward;
//    //        GameManager.instance.PlaySE(cameraResetS);
//    //        L_isReset = false;
//    //    }
//    //} 

//    void Attack()   //ジャンプ中は攻撃できない
//    {
//        if (R_inputTrigger == 0 && inputAttack == false)
//        {
//            R_isReset = true;
//            return;
//        }
//        if (isAttack == true || isJump == true || R_isReset == false)
//            return;

//        if (R_inputTrigger > triggerTiming || inputAttack)  //AボタンかRTで攻撃
//        {
//            animator.Play("Attacking", 0, 0.0f);
//            GameManager.instance.PlaySE(attack_true_S);         //仮 当たったかどうかで音変えると思われる
//            isAttack = true;
//            R_isReset = false;
//            boxCollider.enabled = true;
//            motionTime = 0.0f;
//            // particles[1].Stop();    //ハンマーを後ろに構える時間はパーティクル停止

//            //Debug.Log("boxCollider.enabled = " + boxCollider.enabled);
//            //   Debug.Log("Rトリガー = " + R_inputTrigger);
//            //攻撃モーションへの遷移
//            //_ResultManager.NormalHit(); //デバッグ用
//        }
//    }
//    void KnockBack(Collision collision)
//    {
//        GameManager.instance.PlaySE(damagedS);
//        isJump = true;
//        Debug.Log("isKnockBack");
//        Vector3 direction = collision.gameObject.transform.forward;

//        m_Rigidbody.AddForce(-direction * knockBackP, ForceMode.Impulse);
//        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //若干上方向にも飛ばす

//    }

//    public void fall()  //落下判定エリアで使う
//    {

//        GameManager.instance.PlaySE(fallS);
//        isFall = true;
//        canMove = false;

//        //ここで操作不能にすればすれすれから復帰した時にジャンプができなくなることを防げそう
//        //落下モーションへの遷移
//    }

//    void Move()
//    {
//        if (!canMove) //攻撃中は移動もジャンプもできない->returnじゃなくてその場で固定させたい
//            return;
//        //else if (Mathf.Approximately(inputHorizontal, 0.0f) && Mathf.Approximately(inputVertical, 0.0f) && isJump == true)    
//        //{
//        //    //inputHorizontal += 0.1f;  入力は正負の値だからこれだとダメ    beforePosとnowPosを使えばできそう？
//        //    //inputVertical   += 0.1f;
//        //    return;     //ジャンプ中に入力値がほぼゼロならreturnすれば自然な慣性が働きそう -> 若干不自然な動きに。要改善
//        //}

//        if (isAttack == true)
//        {
//            inputHorizontal = 0;
//            inputVertical = 0;
//        }

//        // カメラの方向から、X-Z平面の単位ベクトルを取得
//        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

//        // 方向キーの入力値とカメラの向きから、移動方向を決定
//        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;


//        //移動速度の計算
//        //clampは値の範囲制限
//        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);   //GetAxisは0から1で入力値を管理する、斜め移動でWとAを同時押しすると
//                                                                      //1以上の値が入ってくるからVector3.ClampMagnitudeメソッドを使って入力値を１に制限する(多分)

//        velocity = clampedInput * walkSpeed;
//        // transform.LookAt(m_Rigidbody.position + input); //キャラクターの向きを現在地＋入力値の方に向ける

//        //Rigidbodyに一度力を加えると抵抗する力がない限りずっと力が加わる
//        //AddForceに加える力をwalkSpeedで設定した速さ以上にはならないように
//        //今入力から計算した速度から現在のRigidbodyの速度を引く
//        velocity = velocity - m_Rigidbody.velocity;

//        //　速度のXZを-walkSpeedとwalkSpeed内に収めて再設定
//        velocity = new Vector3(Mathf.Clamp(velocity.x, -walkSpeed, walkSpeed), 0f, Mathf.Clamp(velocity.z, -walkSpeed, walkSpeed));

//        if (moveForward != Vector3.zero)
//        {
//            //SmoothDampAngleで滑らかな回転をするためには引数（moveForwardとvelocityだけ）をVector3からfloatに変換しなければいけない

//            targetRotation = Mathf.Atan2(moveForward.x, moveForward.z) * Mathf.Rad2Deg;     //Atan2, ベクトルを角度(ラジアン)に変換する Rad2Deg(radian to degrees?)ラジアンから度に変換する

//            //SmoothDampAngle(現在の値, 目的の値, ref 現在の速度, 遷移時間, 最高速度); 現在の速度はnullで良いっぽい？
//            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref yVelocity, smoothTime);
//            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
//        }

//        m_Rigidbody.AddForce(m_Rigidbody.mass * velocity / Time.fixedDeltaTime, ForceMode.Force);
//        // F・・・力  
//        // m・・・質量  
//        // a・・・加速度
//        // Δt・・・力を加えた時間 (Time.fixedDeltatime) 
//        //F = ｍ * a / Δt    Forceは力を加えた時間を使って計算
//    }

//    void Jump()
//    {
//        if (isJump == true || isFall == true || isAttack == true) return;   //落下中と攻撃中はジャンプをさせない

//        if (UnityEngine.Input.GetButtonDown("Jump"))
//        {
//            //移動中またはその場でジャンプした時の遷移

//            animator.ResetTrigger("toIdle");
//            animator.Play("Jumping", 0, 0.0f);
//            m_Rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
//            isJump = true;
//            //Debug.Log("isjump = " + isJump);         


//            //ジャンプモーション→落下モーションに遷移
//        }
//    }

//    void Gravity()
//    {
//        //落下速度の調整用
//        if (isJump == true)
//        {
//            m_Rigidbody.AddForce(new Vector3(0, gravityPower, 0));
//        }
//    }
//}
