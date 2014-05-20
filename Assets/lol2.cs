using UnityEngine;
using System.Collections;

public class lol2 : MonoBehaviour {
    float currentX;
    // Use this for initialization
    void Start () {
	
    }
	
    // Update is called once per frame
    void Update () {
        currentX += 1f * Time.deltaTime;
        transform.Rotate (new Vector3 (currentX, 0, 0));
        if (currentX > 10f) {
            currentX = 0f;
        }
    }
}
