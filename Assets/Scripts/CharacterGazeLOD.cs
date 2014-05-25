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

        float distance = float.MaxValue;

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

        characterAnimation.EnableAnimation();
        switch (lod) {
            case LOD.High:
                SetCoolDown();
                characterAnimation.SetQuality(LOD.High);
                break;
            case LOD.Medium:
                characterAnimation.SetQuality(LOD.Medium);
                break;
            case LOD.Low:
                characterAnimation.SetQuality(LOD.Low);
                break;
            case LOD.Minimal:
                characterAnimation.SetQuality(LOD.Minimal);
                //TODO
                break;
        }

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
