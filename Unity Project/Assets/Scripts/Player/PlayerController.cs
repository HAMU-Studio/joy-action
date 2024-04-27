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

    //�n�ʂ̏�Ȃ�������[�V�����A�Ⴄ�Ȃ痎�����[�V����              

    private bool isKnockBack = false;           //�m�b�N�o�b�N�����ǂ���
    private bool isFall = false;
    public bool isAttack = false;
    public bool isHit = false;   
    private bool canMove = true;
    
    //private bool isKnockBack = false;

    [Header("�ړ��X�s�[�h")]
    [SerializeField] private float walkSpeed = 4f;
    [Header("�������x�̒����@-����")]
    [SerializeField] float gravityPower = default!;

    //[Header("�g���K�[�̔����^�C�~���O")]
    //[SerializeField] float triggerTiming = 0.5f;         //�g���K�[���ǂ��܂ŉ������܂ꂽ�甽�����邩 �v���� 
    [Header("��]����")]
    [SerializeField] float smoothTime = 0.3f;                //�i�s�����ւ̉�]�ɂ����鎞��

    //[Header("�W�����v�̋���")]
    //[SerializeField] private float jumpPower = 5f;             //�W�����v�̂悳
    [Header("�m�b�N�o�b�N�̋���")]
    [SerializeField] private float knockBackP = 5f;                //�m�b�N�o�b�N�̋���
    [Header("�m�b�N�o�b�N��������̗�")]
    [SerializeField] float knockBackUpP = 3f;            //�m�b�N�o�b�N��������ɕ�����


   // [SerializeField] Collider boxCollider = default!;

    [SerializeField] MainGameManager _MainGameManager;

  //  [SerializeField] ResultManager _ResultManager; //�f�o�b�O�p

    float inputHorizontal;      //���������̓��͒l
    float inputVertical;        //���������̓��͒l
 
    float targetRotation;   //��]�Ɏg��
    float yVelocity = 0.0f;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();      
       // _MainGameManager.isInvincible = false;
        SetControllers();
    }

    void Update()
    {
        //����if����joycon�ڑ��̌��m���ł���炵��
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
        if (UnityEngine.Input.GetKeyDown(KeyCode.X))    //�f�o�b�O�p���G���[�hon
        {
            _MainGameManager.isInvincible = true;
            Debug.Log("���G");
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.M))    //�f�o�b�O�p���G���[�hoff
        {
            _MainGameManager.isInvincible = false;
            Debug.Log("���G����");
        }

        float[] stick = m_joyconL.GetStick();

        //stick0�����������̓��͒l�A1�����������̓��͒l
        inputHorizontal = stick[0];
        inputVertical = stick[1];
    }

    private void OnCollisionEnter(Collision collision)
    {
        //������@�͂ł��Ȃ�����Trigger�Ŕ��肵����
        if (isKnockBack == true)
        {
            if (collision.gameObject.CompareTag("Ground"))  //���n������
            {              
                isKnockBack = false;
                isFall = false;
                canMove = true;
                Debug.Log("toLanding");
            }
        }

        if (_MainGameManager.isInvincible == false)
        {
            //���G���Ԓ��̓_���[�W��H���Ȃ�
            if (collision.gameObject.CompareTag("Enemy") )
            {
                canMove = false;
                KnockBack(collision);
                _MainGameManager.Miss();
            }
        }
    } 

    void Attack()   //�W�����v���͍U���ł��Ȃ�
    {

        
    }
    void KnockBack(Collision collision)
    {   
        isKnockBack = true;
        Debug.Log("isKnockBack");
        Vector3 direction = collision.gameObject.transform.forward;

        m_Rigidbody.AddForce(-direction * knockBackP, ForceMode.Impulse);
        m_Rigidbody.AddForce(transform.up * knockBackUpP, ForceMode.Impulse);   //�኱������ɂ���΂�

    }

    public void fall()  //��������G���A�Ŏg��
    {
        isFall = true;
        canMove = false;

        //�����ő���s�\�ɂ���΂��ꂷ�ꂩ�畜�A�������ɃW�����v���ł��Ȃ��Ȃ邱�Ƃ�h������
        //�������[�V�����ւ̑J��
    }

    void Move()
    {
        if (!canMove) //�U�����͈ړ����W�����v���ł��Ȃ�->return����Ȃ��Ă��̏�ŌŒ肳������
            return;
     
        if (isAttack == true)
        {
            inputHorizontal = 0;
            inputVertical = 0;
        }

        // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        
        //�v���C���[�̌������Ƃ��̏�ŉ�]������
        // �����L�[�̓��͒l�ƃJ�����̌�������A�ړ�����������
        Vector3 moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;


        //�ړ����x�̌v�Z
        //clamp�͒l�͈̔͐���
        var clampedInput = Vector3.ClampMagnitude(moveForward, 1f);   //GetAxis��0����1�œ��͒l���Ǘ�����A�΂߈ړ���W��A�𓯎����������
                                                                      //1�ȏ�̒l�������Ă��邩��Vector3.ClampMagnitude���\�b�h���g���ē��͒l���P�ɐ�������(����)

        m_velocity = clampedInput * walkSpeed;
        // transform.LookAt(m_Rigidbody.position + input); //�L�����N�^�[�̌��������ݒn�{���͒l�̕��Ɍ�����

        //Rigidbody�Ɉ�x�͂�������ƒ�R����͂��Ȃ����肸���Ɨ͂������
        //AddForce�ɉ�����͂�walkSpeed�Őݒ肵�������ȏ�ɂ͂Ȃ�Ȃ��悤��
        //�����͂���v�Z�������x���猻�݂�Rigidbody�̑��x������
        m_velocity = m_velocity - m_Rigidbody.velocity;

        //�@���x��XZ��-walkSpeed��walkSpeed���Ɏ��߂čĐݒ�
        m_velocity = new Vector3(Mathf.Clamp(m_velocity.x, -walkSpeed, walkSpeed), 0f, Mathf.Clamp(m_velocity.z, -walkSpeed, walkSpeed));

        if (moveForward != Vector3.zero)
        {
            //SmoothDampAngle�Ŋ��炩�ȉ�]�����邽�߂ɂ͈����imoveForward��velocity�����j��Vector3����float�ɕϊ����Ȃ���΂����Ȃ�

            targetRotation = Mathf.Atan2(moveForward.x, moveForward.z) * Mathf.Rad2Deg;     //Atan2, �x�N�g�����p�x(���W�A��)�ɕϊ����� Rad2Deg(radian to degrees?)���W�A������x�ɕϊ�����

            //SmoothDampAngle(���݂̒l, �ړI�̒l, ref ���݂̑��x, �J�ڎ���, �ō����x); ���݂̑��x��null�ŗǂ����ۂ��H
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref yVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        m_Rigidbody.AddForce(m_Rigidbody.mass * m_velocity / Time.fixedDeltaTime, ForceMode.Force);
        // F�E�E�E��  
        // m�E�E�E����  
        // a�E�E�E�����x
        // ��t�E�E�E�͂����������� (Time.fixedDeltatime) 
        //F = �� * a / ��t    Force�͗͂����������Ԃ��g���Čv�Z
    }   

    void Gravity()
    {
        //�������x�̒����p
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
            GUILayout.Label("Joy-Con ���ڑ�����Ă��܂���");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) ���ڑ�����Ă��܂���");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) ���ڑ�����Ă��܂���");
            return;
        }
    }
    private void SetControllers()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0)
            return;

        //���̂ւ��LINQ���g�p�����f�[�^�����B�p�׋��B
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
        //isRight�͂Ȃ����ۂ��B�ߖ񂩂�
    }

}
