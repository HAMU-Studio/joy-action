using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float enemyMoveSpeed = 3.5f; // SerializeFieldを使用してEnemyの移動速度を設定

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        // このスクリプトを設定したGameObjectのNavMeshAgentコンポーネントを取得
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Enemyの移動速度を設定
        navMeshAgent.speed = enemyMoveSpeed;
    }

    // PlayerDetectorクラスに作ったonTriggerStayEventにセットする。
    public void OnDetectObject(Collider collider)
    {
        // 検知したオブジェクトに"Player"タグが付いてれば、そのオブジェクトを追いかける
        if (collider.gameObject.tag == "Player")
        {
            // 対象のオブジェクトに向かって移動する
            navMeshAgent.SetDestination(collider.gameObject.transform.position);
        }
    }

    // PlayerDetectorクラスに作ったonTriggerExitEventにセットする。 
    public void OnLoseObject(Collider collider)
    {
        // 検知したオブジェクトに"Player"タグが付いてれば、その場で止まる
        if (collider.gameObject.tag == "Player")
        {
            // その場で止まる（目的地を今の自分自身の場所にすることにより止めている）
            navMeshAgent.SetDestination(transform.position);
        }
    }
}