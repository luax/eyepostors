using UnityEngine;
using System.Collections;

public class CharacterGazeLOD : MonoBehaviour
{

    public float highLimit = 0.2f;
    public float standardLimit = 0.5f;

    private GameObject impostor;
    private GameObject characterMesh;

    private enum LOD
    {
        High,
        Standard,
        Low
    }

    public void Awake()
    {
        characterMesh = transform.FindChild("CharacterMesh").gameObject;
        impostor = transform.FindChild("Impostor").gameObject;
        characterMesh.SetActive(false);
        impostor.SetActive(true);
    }

    public void Update()
    {
        float distance = GazeDistance.Instance.CalculateDistance(transform.position);
        switch (GetLOD(distance))
        {
            case LOD.High:
                SetImpostor(false);
                SetCharacterMesh(true);
                break;
            case LOD.Standard:
                SetImpostor(true, LOD.Standard);
                SetCharacterMesh(false);
                break;
            case LOD.Low:
                SetImpostor(true, LOD.Low);
                SetCharacterMesh(false);
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


    private void SetImpostor(bool active, LOD lod = LOD.Low)
    {
        impostor.SetActive(active);
        if (!active)
        {
            return;
        }

    }

    private void SetCharacterMesh(bool active)
    {
        characterMesh.SetActive(active);
        if (!active)
        {
            return;
        }
    }
}
