using UnityEngine;
using System.Collections;

public class Impostor : MonoBehaviour
{
    public int ShirtColor { get; set; }
    public int updateAnimationFrameCount = 2;
    public int updateRotationFrameCount = 5;

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
    private int currentDirection;
    private Vector3 quadRotation;
    private const int RIGHT = 0;
    private const int LEFT = 1;
    
    
    void Start()
    {
        impostorTransform = transform;
        parentTransform = impostorTransform.parent;
        impostorRenderer = renderer;
        characterAnimation = transform.parent.GetComponent<CharacterAnimation>();
        quadRotation = new Vector3(0, 180f, 0);
        quad = gameObject.GetComponent<MeshFilter>().mesh;
        quad.uv = new Vector2[] {
            new Vector2(0.125f, 0.5f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0.5f),
            new Vector2(0.125f, 1f)
        };
        int[] triangles = new int[]{2, 1, 0, 3, 0, 1}; // rotate quad faces
        quad.triangles = triangles;
        quality = Materials.LowQuality;
        SetMaterial(0, 0);
    }
    
    public void Update()
    {
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
        float angle = Vector3.Angle(cameraToObject, levelCameraToObject);
        return Mathf.RoundToInt((angle / 90f) * ((Settings.numberOfAngles / 4) - 1));
    }
    
    private int GetIndexY(Vector3 cameraToObject)
    {
        Vector3 parentRight = parentTransform.right;
        Vector3 parentForward = parentTransform.forward;
        parentRight.y = cameraToObject.y = parentForward.y = 0;
        float angle = Vector3.Angle(cameraToObject, parentForward);
        int index = Mathf.RoundToInt((angle / 180f) * ((Settings.numberOfAngles / 2) - 1));
        if (Vector3.Dot(cameraToObject, parentRight) > 0f) {
            index = Settings.numberOfAngles - index - 1;
        }
        return index;
    }
    
    public void SetFrame(int frameNumber)
    {
        float x = (0.125f * (frameNumber % 8));
        float y = 0.5f - (0.5f * (frameNumber / 8));
        
        quad.uv = new Vector2[] { 
            new Vector2(x + 0.125f, y),
            new Vector2(x, 0.5f + y),
            new Vector2(x, y),
            new Vector2(x + 0.125f, 0.5f + y)}; 
    }
    
    public float GetAnimationPercentage()
    {
        return ((float)frameIndex / (float)Settings.numberOfFrames);
    }
    
    public void SetAnimationPercentage(float percent)
    {
        frameIndex = Mathf.RoundToInt(percent * Settings.numberOfFrames);
        frameIndex = (frameIndex == Settings.numberOfFrames) ? 0 : frameIndex;
        SetFrame(frameIndex);
    }
    
    private void SetMaterial(int indexX, int indexY)
    {
        impostorRenderer.sharedMaterial = Materials.GetMaterial(ShirtColor, indexX, indexY, quality);
    }
}
