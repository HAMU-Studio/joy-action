using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetting : MonoBehaviour
{
    [SerializeField] float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            //�L�[���͂��擾
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            //�ړ������̌v�Z
            Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            //�ړ��������ς��ꍇ�̂݉�]���v�Z
            if (moveDirection.magnitude >= 0.1f)
            {
                //�ړ�
                Vector3 moveVector = moveDirection * speed * Time.deltaTime;
                transform.Translate(moveVector, Space.World);

                Vector3 newPosition = transform.position;
                newPosition.x = transform.position.x + 1.1f;
                newPosition.z = transform.position.z + 0.2f;

                //�v���C���[�̐��ʂ��ړ������Ɍ�����
                Quaternion toRotation = Quaternion.LookRotation(-moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 0.1f);
            }
    }
}
