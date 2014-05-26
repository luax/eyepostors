﻿using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour
{
    public float NormalizedTime { get; set; }
    public GameObject impostor;
    public GameObject characterMesh;

    private Animator animator;
    private SkinnedMeshRenderer characterRenderer;
    private LOD oldLOD;
    private Impostor impostorScript;
    private CharacterGazeLOD charGazeLOD;

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
        oldLOD = LOD.Minimal;

        characterMesh.SetActive(false);
        impostor.SetActive(true);
    }

    public void Update()
    {
        NormalizedTime += Time.deltaTime;
        NormalizedTime = NormalizedTime - Mathf.Floor(NormalizedTime);
    }

    private void Impostor(LOD newLOD)
    {
        UpdateNormalizedTime();
        characterMesh.SetActive(false);

        if (newLOD == LOD.Medium) {
            impostorScript.UpdateMaterial(Materials.MediumQuality);
        } else {
            impostorScript.UpdateMaterial(Materials.LowQuality);
            if (newLOD == LOD.Minimal) {
                impostorScript.SetMinimalLOD(true);
            }
        }
        if (oldLOD == LOD.Minimal) {
            impostorScript.SetMinimalLOD(false);
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

    public void SetLOD(LOD newLOD)
    {
        if (newLOD == LOD.High) {
            Mesh();
        } else {
            Impostor(newLOD);
        }
        oldLOD = newLOD;
    }

    private void UpdateNormalizedTime()
    {
        NormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime -
            Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

}