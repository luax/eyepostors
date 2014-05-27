﻿using UnityEngine;
using System.Collections;

public class Impostor : MonoBehaviour
{
    public int ShirtColor { get; set; }

    public int updateAnimationFrameCount = 2;
    public int updateRotationFrameCount = 5;
    private int numberOfAngles;
    private int halfOfAngles;
    private int quarterOfAngles;
    private int numberOfFrames;
    private int quality;
    private int frameRotation;
    private int frameAnimation;
    private Transform impostorTransform;
    private Transform parentTransform;
    private Renderer impostorRenderer;
    private CharacterAnimation characterAnimation;
    private Mesh quad;
    private int frameIndex;
    private int currentAngleIndexX;
    private int currentAngleIndexY;
    private const int RIGHT = 0;
    private const int LEFT = 1;

    private bool minimalLOD;

    void Start()
    {
        numberOfAngles = Settings.numberOfAngles;
        halfOfAngles = Settings.numberOfAngles / 2;
        quarterOfAngles = Settings.numberOfAngles / 4;
        numberOfFrames = Settings.numberOfFrames;
        impostorTransform = transform;
        parentTransform = impostorTransform.parent;
        impostorRenderer = renderer;
        characterAnimation = transform.parent.GetComponent<CharacterAnimation>();
        quad = gameObject.GetComponent<MeshFilter>().mesh;
        int[] triangles = new int[]{2, 1, 0, 3, 0, 1}; // rotate quad faces
        quad.triangles = triangles;
        quad.uv = Materials.GetUV(0);
        quality = Materials.LowQuality;
       
        SetMaterial(0, 0);
    }

    public void Update()
    {
        if (minimalLOD) {
            LookAtCamera();
            return;
        }
        if (frameRotation >= updateRotationFrameCount) {
            frameRotation = 0;
            UpdateRotation();
            LookAtCamera();
        }
        if (frameAnimation >= updateAnimationFrameCount) {
            frameAnimation = 0;
            UpdateAnimation();
        }
        frameRotation++;
        frameAnimation++;
    }

    public void ForcedUpdate()
    {
        UpdateRotation();
        LookAtCamera();
        UpdateAnimation();
    }

    private void LookAtCamera()
    {
        impostorTransform.LookAt(Settings.cameraTransform.position);
    }
    
    public void UpdateAnimation()
    {
        SetAnimationPercentage(characterAnimation.NormalizedTime);
    }
    
    public void UpdateRotation()
    {
        Vector3 cameraToObject = Settings.cameraTransform.position - parentTransform.position;
        int indexX = GetIndexX(cameraToObject);
        int indexY = GetIndexY(cameraToObject);
        if (indexY != currentAngleIndexY || indexX != currentAngleIndexX) {
            SetMaterial(indexX, indexY);
            currentAngleIndexX = indexX;
            currentAngleIndexY = indexY;
        }
    }

    public void UpdateMaterial(int q)
    {
        if (quality == q) {
            return;
        }
        quality = q;
        SetMaterial(currentAngleIndexX, currentAngleIndexY);
    }
    
    private int GetIndexX(Vector3 cameraToObject)
    {
        Vector3 levelCameraToObject = cameraToObject;
        levelCameraToObject.y = 0;
        float angle = Vector3.Angle(cameraToObject, levelCameraToObject) - 10f;
        angle = Mathf.Max(angle, 0);    
        return Mathf.RoundToInt((angle / 90f) * (quarterOfAngles - 1));
    }

    public void SetMinimalLOD(bool enabled)
    {
        minimalLOD = enabled;
        if (enabled) {
            characterAnimation.NormalizedTime = 0;
        }
    }
    
    private int GetIndexY(Vector3 cameraToObject)
    {
        Vector3 parentRight = parentTransform.right;
        Vector3 parentForward = parentTransform.forward;
        parentRight.y = cameraToObject.y = parentForward.y = 0;
        float angle = Vector3.Angle(cameraToObject, parentForward);
        
        int index = Mathf.RoundToInt((angle * numberOfAngles) / 360f);
        if (index != 0 && index != halfOfAngles) {
            if (Vector3.Dot(cameraToObject, parentRight) > 0f) {
                index = numberOfAngles - index;
            }
        }
        return index;
    }
    
    public float GetAnimationPercentage()
    {
        return ((float)frameIndex / (float)numberOfFrames);
    }
    
    public void SetAnimationPercentage(float percent)
    {
        frameIndex = Mathf.RoundToInt(percent * numberOfFrames);
        frameIndex = (frameIndex == numberOfFrames) ? 0 : frameIndex;
        quad.uv = Materials.GetUV(frameIndex);
    }
    
    private void SetMaterial(int indexX, int indexY)
    {
        impostorRenderer.sharedMaterial = Materials.GetMaterial(ShirtColor, indexX, indexY, quality);
    }
}