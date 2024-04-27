using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;
    public Transform SpawnPoint;

    private float time;
    private bool hasSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSpawned)
        {
            time += Time.deltaTime;

            if (time >= 3.0f)
            {
                SpawnEnemy();
                hasSpawned = true;
            }
        }
    }

    void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(Enemy);
        newEnemy.transform.position = SpawnPoint.position;
    }
}
