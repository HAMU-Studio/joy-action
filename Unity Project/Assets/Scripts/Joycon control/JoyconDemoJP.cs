using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDemoJP : MonoBehaviour
{

    private List<Joycon> joycons;

    // Unity �o�R�ŗ��p�\�Ȓl
    public float[] stick; // �X�e�B�b�N
    public Vector3 gyro; // �W���C��
    public Vector3 accel; // �A�N�Z��
    public Quaternion orientation; // ����
    public int jc_ind = 0; // �C���f�b�N�X 0�A�W���C�R���̐ڑ�1p, 1�A�W���C�R���̐ڑ��Qp

    void Start()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        // �V�[������ JoyconManager �ɃA�^�b�`���ꂽ public Joycon �z����擾
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Joycon ���A�^�b�`����Ă���ꍇ�ɂ̂݊m�F�����悤�ɂ��܂�
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];
            // GetButtonDown �̓{�^���������ꂽ���Ƃ��m�F���܂��i�z�[���h���ꂽ��Ԃł͂���܂���j
            if (j.GetButtonDown(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("�V�����_�[�{�^�� 2 ��������܂���");
                // GetStick �� x/y �W���C�X�e�B�b�N�̃R���|�[�l���g������2�v�f�̃x�N�g����Ԃ��܂�
                Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}", j.GetStick()[0], j.GetStick()[1]));

                // Joycon �ɂ͎��C�Z���T�[���Ȃ����߁A���m�ȃ��[�l�����肷�邱�Ƃ��ł��܂���B Joycon.Recenter ���g�p���ă��[�l�����Z�b�g�ł��܂��B
                j.Recenter();
            }
            // GetButtonUp �̓{�^���������ꂽ���ǂ������m�F���܂�
            if (j.GetButtonUp(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("�V�����_�[�{�^�� 2 ��������܂���");
            }
            // GetButton �̓{�^�������݉�����Ă��邩�ǂ������m�F���܂��i������Ă��邩�z�[���h����Ă��邩�j
            if (j.GetButton(Joycon.Button.SHOULDER_2))
            {
                Debug.Log("�V�����_�[�{�^�� 2 ��������Ă��܂�");
            }

            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                Debug.Log("�U��");

                // ����g����160Hz�A�����g����320Hz�̐U���ŁA200�~���b�U�����܂��B�ڍׂ͂�������Q�Ƃ��Ă��������F
                // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

                j.SetRumble(160, 320, 0.6f, 200);

                // SetRumble �̍Ō�̈����itime�j�̓I�v�V�����ł��B�I�t�ɂ��鎞�Ԃ��w�肹����3�̈����ŌĂяo���Ă��������B
                // �i�U���l�𓮓I�ɕύX����ꍇ�ɕ֗��ł��B�j
                // �I�t�ɂ���Ƃ��� SetRumble(0,0,0) ���Ăяo���܂��B
            }

            stick = j.GetStick();

            // Gyro �l�Fx�Ay�Az ���̒l�i�b������̃��W�A���j
            gyro = j.GetGyro();

            // Accel �l�Fx�Ay�Az ���̒l�iGs�j
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
