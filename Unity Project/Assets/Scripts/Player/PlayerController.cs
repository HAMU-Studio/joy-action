////���V
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

//    //�n�ʂ̏�Ȃ�������[�V�����A�Ⴄ�Ȃ痎�����[�V����              

//    private bool isJump = false;           //�W�����v�����ǂ���
//    private bool isFall = false;
//    public bool isAttack = false;
//    public bool isHit = false;
//    private bool isDidHit = false;
//    private bool canMove = true;
//    private bool onlyFirst = false;
//    //private bool isKnockBack = false;

//    [Header("�ړ��X�s�[�h")]
//    [SerializeField] private float walkSpeed = 4f;
//    [Header("�������x�̒����@-����")]
//    [SerializeField] float gravityPower = default!;
//    [Header("�U�����[�V�����̒���")]
//    [SerializeField] float Attack_Finish_Time = 0.3f;    //�U�����[�V�����̒����ɉ����ĕς��� (�A�ł�Trigger�ɂ��A�����͂�h������)
//    [Header("�U�����[�V�����̃X�s�[�h")]
//    [SerializeField] float Attack_Motion_Speed = 1.0f;
//    [Header("���肪����鎞��")]
//    [SerializeField] float Collider_Start_Time = 0.1f;
//    [Header("���肪�����鎞��")]
//    [SerializeField] float Collider_Stop_Time = 0.3f;
//    [Header("�p�[�e�B�N��������鎞��")]
//    [SerializeField] float Particle_Start_Time = 0.1f;
//    [Header("�p�[�e�B�N���������鎞��")]
//    [SerializeField] float Particle_Stop_Time = 0.3f;


//    [Header("�g���K�[�̔����^�C�~���O")]
//    [SerializeField] float triggerTiming = 0.5f;         //�g���K�[���ǂ��܂ŉ������܂ꂽ�甽�����邩 �v���� 
//    [Header("��]����")]
//    [SerializeField] float smoothTime = 0.3f;                //�i�s�����ւ̉�]�ɂ����鎞��
//    [Header("�W�����v�̋���")]
//    [SerializeField] private float jumpPower = 5f;             //�W�����v�̂悳
//    [Header("�m�b�N�o�b�N�̋���")]
//    [SerializeField] private float knockBackP = 5f;                //�m�b�N�o�b�N�̋���
//    [Header("�m�b�N�o�b�N��������̗�")]
//    [SerializeField] float knockBackUpP = 3f;            //�m�b�N�o�b�N��������ɕ�����
//    [Header("�p�[�e�B�N��")]
//    [Tooltip("1�q�b�g�A2�f�U��A3�W�����v")]
//    [SerializeField] private ParticleSystem[] particles = default!;
//    [Header("����܂ɓ����������̃G�t�F�N�g���Đ�������")]
//    [SerializeField] private ParticlePlayer hitParticlePlayer;

//    [SerializeField] Collider boxCollider = default!;
//    [SerializeField] GameObject CinemachineCameraTarget;    //�J�����̃^�[�Q�b�g��ʃI�u�W�F�N�g�ɂ��邱�Ƃœ��̕�����ǔ�

//    [SerializeField] AudioClip jumpS = default!;
//    [SerializeField] AudioClip attack_true_S = default!;
//    [SerializeField] AudioClip fallS = default!;
//    [Header("���Ă����̉�")]
//    [SerializeField] AudioClip hitS = default!;
//    [Header("��_����")]
//    [SerializeField] AudioClip damagedS = default!;
//    // [SerializeField]  AudioClip cameraResetS = default!;

//    [SerializeField] MainGameManager _MainGameManager;

//    [SerializeField] ResultManager _ResultManager; //�f�o�b�O�p

//    float inputHorizontal;      //���������̓��͒l
//    float inputVertical;        //���������̓��͒l
//    //float L_inputTrigger;
//    float R_inputTrigger;
//    bool inputAttack;

//    bool R_isReset;
//    //  bool L_isReset;
//    float targetRotation;   //��]�Ɏg��
//    float yVelocity = 0.0f;
//    float motionTime = 0.0f;     // �U�����[�V�������n�܂��Ă���̌o�ߎ��Ԃ��i�[->animation�̑J�ڂłł����� 
//    float time = 0f;                //Running���[�V�����Ɏg��

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

//        //if (stateInfo.IsName("Running") || stateInfo.IsName("Jumping")) //���܂ɂȂ������邩��Ȃ�
//        Move();
//    }

//    private void Input()
//    {
//        if (UnityEngine.Input.GetKeyDown(KeyCode.X))    //�f�o�b�O�p���G���[�hon
//        {
//            _MainGameManager.isInvincible = true;
//            Debug.Log("���G");
//        }
//        if (UnityEngine.Input.GetKeyDown(KeyCode.M))    //�f�o�b�O�p���G���[�hoff
//        {
//            _MainGameManager.isInvincible = false;
//            Debug.Log("���G����");
//        }

//        // ���͕s�Ȃ�I���
//        if (GameManager.instance.ReturnInputState() != InputState.OnInput)
//        {
//            inputHorizontal = 0;
//            inputVertical = 0;
//            animator.ResetTrigger("toIdle");
//            return;
//        }

//        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

//        inputHorizontal = UnityEngine.Input.GetAxisRaw("Horizontal");   //���͒l�̊i�[
//        inputVertical = UnityEngine.Input.GetAxisRaw("Vertical");
//        // L_inputTrigger = UnityEngine.Input.GetAxis("L_Trigger");
//        R_inputTrigger = UnityEngine.Input.GetAxis("R_Trigger");
//        inputAttack = UnityEngine.Input.GetButtonDown("Attack");


//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        //������@�͂ł��Ȃ�����Trigger�Ŕ��肵����
//        if (isJump == true || isFall == true)
//        {
//            if (collision.gameObject.CompareTag("Ground"))  //���n������
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
//            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("DarumaBall") || collision.gameObject.CompareTag("Ball"))     //���G���Ԓ��̓_���[�W��H���Ȃ�
//            {
//                canMove = false;
//                KnockBack(collision);
//                _MainGameManager.Miss();
//            }
//        }
//    }

//    void ParticleManage()
//    {
//        if (isHit == true && isDidHit == false)      //�q�b�g���̃G�t�F�N�g�Đ�
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
//        //    particles[1].Stop();    //�n���}�[�����ɍ\���鎞�Ԃ̓p�[�e�B�N����~
//        //}
//        //if (stateInfo.IsName("Running") || stateInfo.IsName("Idle"))
//        //{
//        //    if (particles[1].isPlaying == true)
//        //        particles[1].Stop();
//        //}
//    }
//    void transitionAnim()
//    {
//        if (stateInfo.IsName("Jumping") ||     //�ړ����[�V�����̑J��
//           stateInfo.IsName("Attacking"))
//            return;

//        if (inputHorizontal != 0 || inputVertical != 0)
//        {
//            //���͂�0����Ȃ���Έړ����[�V�����ɑJ��
//            time += Time.deltaTime;
//            animator.ResetTrigger("toIdle");
//        }
//        else if (inputHorizontal == 0 && inputVertical == 0 && stateInfo.IsName("Running") && isJump == false) //isJump�̏��������Ȃ��Ƃ��܂ɃW�����v���[�V�������L�����Z�������
//        {
//            time = 0f;
//            animator.SetTrigger("toIdle");
//        }
//        //  Debug.Log("time = " + time);
//        animator.SetFloat("time", time);
//    }

//    void AttackMotionManage()
//    {
//        //�U�����[�V�������n���}�[�̓����蔻�肪���݂��鎞�Ԃ̊Ǘ�

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
//        if (motionTime >= Collider_Start_Time && motionTime < Collider_Stop_Time && boxCollider.enabled == false)  //��莞�Ԍo�߂Ŕ���o��(�U��n�߂̈�u�͓�����Ȃ�)
//        {
//            boxCollider.enabled = true;
//            //Debug.Log("�R���C�_�[" + boxCollider.enabled);
//        }
//        if (motionTime >= Collider_Stop_Time && boxCollider.enabled == true)   //��莞�Ԍo�߂Ŕ��肪������(�U��؂����Ō�̕��͓�����Ȃ�)
//        {
//            boxCollider.enabled = false;
//            // Debug.Log("�R���C�_�[" + boxCollider.enabled);
//        }
//        if (motionTime >= Attack_Finish_Time && isAttack == true)
//        {
//            isHit = false;
//            isAttack = false;
//            onlyFirst = false;
//            motionTime = 0.0f;
//        }
//    }
//    void Attack_Particle_Manage()       //�n���}�[�̓����蔻�肪���݂��鎞�Ԃƃp�[�e�B�N�������݂��鎞�Ԃ͕�����
//    {
//        if (motionTime >= Particle_Start_Time && motionTime < Particle_Stop_Time && particles[1].isStopped == true)  //��莞�Ԍo�߂Ńp�[�e�B�N���o��
//        {
//            particles[1].Play();    //�U��n�߂͍Đ��@�������邩��               
//        }
//        if (motionTime >= Particle_Stop_Time && particles[1].isPlaying == true)   //��莞�Ԍo�߂Ńp�[�e�B�N����������
//        {
//            particles[1].Stop(); //�U��I��������~                   
//        }
//    }
//    //void CamaraReset()    //�J�������Z�b�g�@�\�������������f�O
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

//    void Attack()   //�W�����v���͍U���ł��Ȃ�
//    {
//        if (R_inputTrigger == 0 && inputAttack == false)
//        {
//            R_isReset = true;
//            return;
//        }
//        if (isAttack == true || isJump == true || R_isReset == false)
//            return;

//        if (R_inputTrigger > triggerTiming || inputAttack)  //A�{�^����RT�ōU��
//        {
//            animator.Play("Attacking", 0, 0.0f);
//            GameManager.instance.PlaySE(attack_true_S);         //�� �����������ǂ����ŉ��ς���Ǝv����
//            isAttack = true;
//            R_isReset = false;
//            boxCollider.enabled = true;
//            motionTime = 0.0f;
//            // particles[1].Stop();    //�n���}�[�����ɍ\���鎞�Ԃ̓p�[�e�B�N����~

//            //Debug.Log("boxCollider.enabled = " + boxCollider.enabled);
//            //   Debug.Log("R�g���K�[ = " + R_inputTrigger);
//            //�U�����[�V�����ւ̑J��
//            //_ResultManager.NormalHit(); //�f�o�b�O�p
//        }
//    }
//    void KnockBack(Collision collision)
//    {
//        GameManager.instance.PlaySE(damagedS);
//        isJump = true;
//        Debug.Log("isKnockBack");
//        Vector3 direction = collision.gameObject.transform.forward;

//        m_Rigidbody.AddForce(-direction * knockBackP, ForceMode.Impulse);
//        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //�኱������ɂ���΂�

//    }

//    public void fall()  //��������G���A�Ŏg��
//    {

//        GameManager.instance.PlaySE(fallS);
//        isFall = true;
//        canMove = false;

//        //�����ő���s�\�ɂ���΂��ꂷ�ꂩ�畜�A�������ɃW�����v���ł��Ȃ��Ȃ邱�Ƃ�h������
//        //�������[�V�����ւ̑J��
//    }

//    void Move()
//    {
//        if (!canMove) //�U�����͈ړ����W�����v���ł��Ȃ�->return����Ȃ��Ă��̏�ŌŒ肳������
//            return;
//        //else if (Mathf.Approximately(inputHorizontal, 0.0f) && Mathf.Approximately(inputVertical, 0.0f) && isJump == true)    
//        //{
//        //    //inputHorizontal += 0.1f;  ���͂͐����̒l�����炱�ꂾ�ƃ_��    beforePos��nowPos���g���΂ł������H
//        //    //inputVertical   += 0.1f;
//        //    return;     //�W�����v���ɓ��͒l���قڃ[���Ȃ�return����Ύ��R�Ȋ������������� -> �኱�s���R�ȓ����ɁB�v���P
//        //}

//        if (isAttack == true)
//        {
//            inputHorizontal = 0;
//            inputVertical = 0;
//        }

//        // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
//        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

//        // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
//        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;


//        //�ړ����x�̌v�Z
//        //clamp�͒l�͈̔͐���
//        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);   //GetAxis��0����1�œ��͒l���Ǘ�����A�΂߈ړ���W��A�𓯎����������
//                                                                      //1�ȏ�̒l�������Ă��邩��Vector3.ClampMagnitude���\�b�h���g���ē��͒l���P�ɐ�������(����)

//        velocity = clampedInput * walkSpeed;
//        // transform.LookAt(m_Rigidbody.position + input); //�L�����N�^�[�̌��������ݒn�{���͒l�̕��Ɍ�����

//        //Rigidbody�Ɉ�x�͂�������ƒ�R����͂��Ȃ����肸���Ɨ͂������
//        //AddForce�ɉ�����͂�walkSpeed�Őݒ肵�������ȏ�ɂ͂Ȃ�Ȃ��悤��
//        //�����͂���v�Z�������x���猻�݂�Rigidbody�̑��x������
//        velocity = velocity - m_Rigidbody.velocity;

//        //�@���x��XZ��-walkSpeed��walkSpeed���Ɏ��߂čĐݒ�
//        velocity = new Vector3(Mathf.Clamp(velocity.x, -walkSpeed, walkSpeed), 0f, Mathf.Clamp(velocity.z, -walkSpeed, walkSpeed));

//        if (moveForward != Vector3.zero)
//        {
//            //SmoothDampAngle�Ŋ��炩�ȉ�]�����邽�߂ɂ͈����imoveForward��velocity�����j��Vector3����float�ɕϊ����Ȃ���΂����Ȃ�

//            targetRotation = Mathf.Atan2(moveForward.x, moveForward.z) * Mathf.Rad2Deg;     //Atan2, �x�N�g�����p�x(���W�A��)�ɕϊ����� Rad2Deg(radian to degrees?)���W�A������x�ɕϊ�����

//            //SmoothDampAngle(���݂̒l, �ړI�̒l, ref ���݂̑��x, �J�ڎ���, �ō����x); ���݂̑��x��null�ŗǂ����ۂ��H
//            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref yVelocity, smoothTime);
//            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
//        }

//        m_Rigidbody.AddForce(m_Rigidbody.mass * velocity / Time.fixedDeltaTime, ForceMode.Force);
//        // F�E�E�E��  
//        // m�E�E�E����  
//        // a�E�E�E�����x
//        // ��t�E�E�E�͂����������� (Time.fixedDeltatime) 
//        //F = �� * a / ��t    Force�͗͂����������Ԃ��g���Čv�Z
//    }

//    void Jump()
//    {
//        if (isJump == true || isFall == true || isAttack == true) return;   //�������ƍU�����̓W�����v�������Ȃ�

//        if (UnityEngine.Input.GetButtonDown("Jump"))
//        {
//            //�ړ����܂��͂��̏�ŃW�����v�������̑J��

//            animator.ResetTrigger("toIdle");
//            animator.Play("Jumping", 0, 0.0f);
//            m_Rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
//            isJump = true;
//            //Debug.Log("isjump = " + isJump);         


//            //�W�����v���[�V�������������[�V�����ɑJ��
//        }
//    }

//    void Gravity()
//    {
//        //�������x�̒����p
//        if (isJump == true)
//        {
//            m_Rigidbody.AddForce(new Vector3(0, gravityPower, 0));
//        }
//    }
//}
