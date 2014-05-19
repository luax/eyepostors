using UnityEngine;
using System.Collections;

public class GazeLOD : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material red;
    private Material blue;
    private Material white;

    public float highLimit = 0.2f;
    public float standardLimit = 0.5f;

    private enum LOD
    {
        High,
        Standard,
        Low
    }

    public void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        red = (Material)Resources.Load("Red");
        blue = (Material)Resources.Load("Blue");
        white = (Material)Resources.Load("White");
        meshRenderer.material = white;
    }

    public void Update()
    {
        float distance = GazeDistance.Instance.CalculateDistance(transform.position);
        switch (GetLOD(distance))
        {
            case LOD.High:
                meshRenderer.material = red;
                break;
            case LOD.Standard:
                meshRenderer.material = blue;
                break;
            case LOD.Low:
                meshRenderer.material = white;
                break;
        }

    }

    private LOD GetLOD(float distance)
    {
        if (distance < highLimit)
        {
            return LOD.High;
        }
        else if (distance < standardLimit)
        {
            return LOD.Standard;
        }
        return LOD.Low;
    }
}
