using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    public GameObject Enemy;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3(0, 0, -0.01f);
        transform.Translate(position);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
