using UnityEngine;
using System.Collections;

public enum TriggerOption
{
    Mouse,
    Gaze
}

public class CharacterGazeLOD : MonoBehaviour
{
    public TriggerOption option;
    public EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;
    public float highLimit = 50f; // TODO
    public float standardLimit = 180f; // TODO
    public Mesh high;
    public Mesh medium;
    public GameObject impostor;
    public GameObject characterMesh;
    public float coolDownTime = 0.5f;
    private Transform myTransform;
    private Transform cameraTransform;
    private SkinnedMeshRenderer characterRenderer;
    private LOD currentLOD;
    private Impostor impostorScript;
    private float coolDown;


    private enum LOD
    {
        High,
        Standard,
        Low
    }

    public void Awake()
    {
        myTransform = transform;
        cameraTransform = Camera.main.transform;
        characterRenderer = characterMesh.GetComponent<SkinnedMeshRenderer>();
        impostorScript = impostor.GetComponent<Impostor>();
        characterMesh.SetActive(false);
        impostor.SetActive(true);
    }

    public void Update()
    {
        float distance = float.MaxValue;

        if (impostor.activeSelf)
        {
            distance = GazeDistance.Instance.CalculateDistance(ref impostor);
        } else
        {
            distance = GazeDistance.Instance.CalculateDistance(ref characterMesh);
        }

        if (!CoolDown())
        {
            SetLOD(GetLOD(distance));
        }
    }

    private void SetLOD(LOD lod)
    {
        if (currentLOD == lod)
        {
            return;
        } else
        {
            SetCoolDown();
        }

        switch (lod)
        {
            case LOD.High:
                SetImpostor(false);
                SetCharacterMesh(true, high);
                break;
            case LOD.Standard:
                SetImpostor(false);
                SetCharacterMesh(true, medium);
                break;
            case LOD.Low:
                SetImpostor(true);
                SetCharacterMesh(false);
                break;
        }

        currentLOD = lod;
    }

    private void SetCoolDown()
    {
        coolDown = coolDownTime;
    }

    private bool CoolDown()
    {
        return (coolDown -= Time.deltaTime) > 0;
    }

    private LOD GetLOD(float distance)
    {
        if (distance < highLimit)
        {
            return LOD.High;
        } else if (distance < standardLimit)
        {
            return LOD.Standard;
        }
        return LOD.Low;
    }

    private void SetImpostor(bool active)
    {
        impostor.SetActive(active);
        if (active)
        {
            impostorScript.Update();
        }
    }

    private void SetCharacterMesh(bool active, Mesh mesh = null)
    {
        characterMesh.SetActive(active);
        if (active && mesh)
        {
            characterRenderer.sharedMesh = mesh;
            characterMesh.GetComponent<Animator>().ForceStateNormalizedTime(impostorScript.normalizedTime);
        }
    }
}
