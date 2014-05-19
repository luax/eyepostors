using UnityEngine;
using System.Collections;

public class GazeLOD : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private Material red;
    private Material blue;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        red = (Material)Resources.Load("Red");
        blue = (Material)Resources.Load("Blue");
        meshRenderer.material = red;
    }

    void Update()
    {
        float distance = GazeDistance.Instance.CalculateDistance(transform.position);
        Debug.Log(distance);
        if (distance < 0.25)
        {
            meshRenderer.material = red;
        }
        else
        {
            meshRenderer.material = blue;
        }
    }
}
