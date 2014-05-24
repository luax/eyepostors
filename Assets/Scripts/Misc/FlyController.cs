using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Translate((new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) + (Input.GetKey(KeyCode.Space) ? Vector3.up : Vector3.zero)) * Time.deltaTime * 20f);
    }
}
