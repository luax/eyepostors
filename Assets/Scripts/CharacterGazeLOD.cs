using UnityEngine;
using System.Collections;

public enum LOD
{
    High,
    Medium,
    Low,
    Minimal
}

public class CharacterGazeLOD : MonoBehaviour
{

    public GameObject impostor;
    public GameObject characterMesh;
    private CharacterAnimation characterAnimation;
    private LOD currentLOD;
    private float coolDown;

    public void Awake()
    {
        characterAnimation = gameObject.GetComponent<CharacterAnimation>();
        characterMesh.SetActive(false);
        impostor.SetActive(true);
    }

    public void Update()
    {
        if (CoolDown()) {
            return;
        }

        float distance = 0;

        if (impostor.activeSelf) {
            distance = GazeDistance.Instance.CalculateDistance(impostor);
        } else {
            distance = GazeDistance.Instance.CalculateDistance(characterMesh);
        }

        SetLOD(GetLOD(distance));
    }

    private void SetLOD(LOD lod)
    {
        if (currentLOD == lod) {
            return;
        }
        if (lod == LOD.High) {
            SetCoolDown();
        }
        characterAnimation.SetQuality(lod);
        currentLOD = lod;
    }

    private void SetCoolDown()
    {
        coolDown = Settings.cooldownTime;
    }

    private bool CoolDown()
    {
        return (coolDown -= Time.deltaTime) > 0;
    }

    private LOD GetLOD(float distance)
    {   
        if (distance < Settings.gazeDistanceHigh) {
            return LOD.High;
        } else if (distance < Settings.gazeDistanceMedium) {
            return LOD.Medium;
        }
        return LOD.Low;
    }
}
