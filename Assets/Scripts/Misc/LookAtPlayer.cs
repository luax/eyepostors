using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour
{

    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 r = transform.position - player.transform.position;
        r.y = 0;
        transform.rotation = Quaternion.LookRotation(r);
    }
}
