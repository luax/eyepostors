using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour
{
    public float NormalizedTime { get; set; }
    public GameObject impostor;
    public GameObject characterMesh;

    private Animator animator;
    private SkinnedMeshRenderer characterRenderer;
    private LOD currentLOD;
    private Impostor impostorScript;
    private CharacterGazeLOD charGazeLOD;

    private bool meshActivated;
    private bool impostorActivated;

    public void Start()
    {
        animator = transform.FindChild("CharacterMesh").GetComponent<Animator>();
        characterRenderer = characterMesh.GetComponent<SkinnedMeshRenderer>();
        impostorScript = impostor.GetComponent<Impostor>();
        charGazeLOD = gameObject.GetComponent<CharacterGazeLOD>();
        DefaultSettings();
    }

    private void DefaultSettings()
    {
        currentLOD = LOD.Minimal;
        meshActivated = false;

        characterMesh.SetActive(meshActivated);
        impostor.SetActive(impostorActivated);
    }

    public void Update()
    {
        NormalizedTime += Time.deltaTime;
        NormalizedTime = NormalizedTime % 1.0f;
    }

    private void Impostor(LOD lod)
    {
        UpdateNormalizedTime();
        characterMesh.SetActive(false);

        if (lod == LOD.Medium) {
            impostorScript.UpdateMaterial(Materials.MediumQuality);
        } else {
            impostorScript.UpdateMaterial(Materials.LowQuality);
            if (lod == LOD.Minimal) {
                // TODO
            }
        }

        impostor.SetActive(true);
        impostorScript.ForcedUpdate();
    }

    private void Mesh()
    {
        impostor.SetActive(false);
        characterMesh.SetActive(true);
        animator.Play("WalkForward", 0, NormalizedTime);
    }

    public void SetLOD(LOD lod)
    {
        if (lod == LOD.High) {
            Mesh();
        } else {
            Impostor(lod);
        }
        currentLOD = lod;
    }

    private void UpdateNormalizedTime()
    {
        NormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime -
            Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

}