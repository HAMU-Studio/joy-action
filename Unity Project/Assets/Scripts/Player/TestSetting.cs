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
            //キー入力を取得
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            //移動方向の計算
            Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            //移動方向が変わる場合のみ回転を計算
            if (moveDirection.magnitude >= 0.1f)
            {
                //移動
                Vector3 moveVector = moveDirection * speed * Time.deltaTime;
                transform.Translate(moveVector, Space.World);

                Vector3 newPosition = transform.position;
                newPosition.x = transform.position.x + 1.1f;
                newPosition.z = transform.position.z + 0.2f;

                //プレイヤーの正面を移動方向に向ける
                Quaternion toRotation = Quaternion.LookRotation(-moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 0.1f);
            }
    }
}
