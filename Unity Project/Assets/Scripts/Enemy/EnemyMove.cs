using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float enemyMoveSpeed = 3.5f; // SerializeField���g�p����Enemy�̈ړ����x��ݒ�

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        // ���̃X�N���v�g��ݒ肵��GameObject��NavMeshAgent�R���|�[�l���g���擾
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Enemy�̈ړ����x��ݒ�
        navMeshAgent.speed = enemyMoveSpeed;
    }

    // PlayerDetector�N���X�ɍ����onTriggerStayEvent�ɃZ�b�g����B
    public void OnDetectObject(Collider collider)
    {
        // ���m�����I�u�W�F�N�g��"Player"�^�O���t���Ă�΁A���̃I�u�W�F�N�g��ǂ�������
        if (collider.gameObject.tag == "Player")
        {
            // �Ώۂ̃I�u�W�F�N�g�Ɍ������Ĉړ�����
            navMeshAgent.SetDestination(collider.gameObject.transform.position);
        }
    }

    // PlayerDetector�N���X�ɍ����onTriggerExitEvent�ɃZ�b�g����B 
    public void OnLoseObject(Collider collider)
    {
        // ���m�����I�u�W�F�N�g��"Player"�^�O���t���Ă�΁A���̏�Ŏ~�܂�
        if (collider.gameObject.tag == "Player")
        {
            // ���̏�Ŏ~�܂�i�ړI�n�����̎������g�̏ꏊ�ɂ��邱�Ƃɂ��~�߂Ă���j
            navMeshAgent.SetDestination(transform.position);
        }
    }
}