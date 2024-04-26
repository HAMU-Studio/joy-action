using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;
    public Transform SpawnPoint;

    private float time;
    private float spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 3.0f;
        //Enemy.transform.position = SpawnPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (spawnTime < time)
        {
            GameObject newEnemy = Instantiate(Enemy);
            newEnemy.transform.position = SpawnPoint.position;

            time = 0;
        }
    }
}
