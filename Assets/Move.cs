using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    private Transform myTransform;
    private float moveSpeed = 2f;

    void Start () {
        myTransform = gameObject.transform;
    }
	
    void Update () {
        MoveForward ();
    }

    private void MoveForward () {
        myTransform.Translate (Vector3.forward * Time.deltaTime * moveSpeed);
    }
}
