using UnityEngine;
using System.Collections;

public class CharacterGazeLOD : MonoBehaviour
{
    public float NormalizedTime { get; set; }

    public GameObject impostor;
    public GameObject characterMesh;

    private Transform myTransform;
    private SkinnedMeshRenderer characterRenderer;
    private LOD currentLOD;
    private Impostor impostorScript;
    private float coolDown;
    private Animator animator;

    private enum LOD
    {
        High,
        Medium,
        Low
    }

    public void Awake()
    {
        myTransform = transform;
        characterRenderer = characterMesh.GetComponent<SkinnedMeshRenderer>();
        impostorScript = impostor.GetComponent<Impostor>();
        animator = characterMesh.GetComponent<Animator>();
        characterMesh.SetActive(false);
        impostor.SetActive(true);
    }

    public void Update()
    {
        NormalizedTime += Time.deltaTime;
        NormalizedTime = NormalizedTime % 1.0f;

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

        SetCoolDown();

        switch (lod) {
            case LOD.High:
                SetImpostor(false, LOD.Medium);
                SetCharacterMesh(true);
                break;
            case LOD.Medium:
                SetCharacterMesh(false);
                SetImpostor(true, LOD.Medium);
                break;
            case LOD.Low:
                SetCharacterMesh(false);
                SetImpostor(true, LOD.Low);
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
        Debug.Log(distance);
        if (distance < Settings.gazeDistanceHigh) {
            Debug.Log("HIGH");
            return LOD.High;
        } else if (distance < Settings.gazeDistanceMedium) {
            Debug.Log("MEDIUM");
            return LOD.Medium;
        }
        Debug.Log("LOW");
        return LOD.Low;
    }

    private void SetCharacterMesh(bool activate)
    {
        if (!activate) {
            NormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 
                Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        characterMesh.SetActive(activate);
        if (activate) {   
            animator.Play("WalkForward", 0, NormalizedTime);
        }

    }

    private void SetImpostor(bool activate, LOD quality)
    {
        impostor.SetActive(activate);
        if (activate && quality == LOD.Low) {
            impostorScript.Quality = 0;
            impostorScript.Update();
        } else if (activate) {
            impostorScript.Quality = 1;
            impostorScript.Update();
        }
    }

}
