﻿using UnityEngine;
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
	
    void Awake()
    {
        animator = transform.FindChild("CharacterMesh").GetComponent<Animator>();
        characterRenderer = characterMesh.GetComponent<SkinnedMeshRenderer>();
        impostorScript = impostor.GetComponent<Impostor>();
        currentLOD = LOD.Low;
        characterMesh.SetActive(false);
        impostor.SetActive(true);
        charGazeLOD = gameObject.GetComponent<CharacterGazeLOD>();
    }
	
    // Update is called once per frame
    void Update()
    {
        NormalizedTime += Time.deltaTime;
        NormalizedTime = NormalizedTime % 1.0f;
    }

    public void DisableAnimation()
    {
        impostorScript.OutOfRange = true;
    }
    public void EnableAnimation()
    {
        impostorScript.OutOfRange = false;
    }

    public void SetQuality(LOD quality)
    {
        if (quality == LOD.High) {
            SetCharacterMesh(true);
            SetImpostor(false);
        } else {
            SetImpostor(true, quality);
        }
        currentLOD = quality;
    }

    private void UpdateNormalizedTime()
    {
        NormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 
            Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    private void SetCharacterMesh(bool activate)
    {
        characterMesh.SetActive(activate);
        if (activate) {   
            animator.Play("WalkForward", 0, NormalizedTime);
        }
        
    }
    
    private void SetImpostor(bool activate, LOD quality = LOD.Minimal)
    {
        if (currentLOD == LOD.High) {
            UpdateNormalizedTime();
            SetCharacterMesh(false);
        }
        impostor.SetActive(activate);
        if (activate && quality == LOD.Low) {
            impostorScript.Quality = 0;
            impostorScript.Update();
        } else {
            impostorScript.Quality = 1;
            impostorScript.Update();
        }
    }
}