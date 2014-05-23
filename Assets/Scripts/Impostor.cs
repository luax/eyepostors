using UnityEngine;
using System.Collections;

public class Impostor : MonoBehaviour
{
	public int numberOfFrames = 15;
	public int numberOfAngles = 15;

	// Component references
	private Transform impostorTransform;
	private Transform parentTransform;
	private Transform cameraTransform;
	private Renderer impostorRenderer;
	private CharacterGazeLOD characterGazeLOD;
	private Mesh quad;
	private int frameIndex;
	private int currentAngleIndex;
	private int currentDirection;
	private const int RIGHT = 0;
	private const int LEFT = 1;

	void Start ()
	{
		impostorTransform = transform;
		cameraTransform = Camera.main.transform;
		parentTransform = impostorTransform.parent;
		impostorRenderer = renderer;
		characterGazeLOD = transform.parent.GetComponent<CharacterGazeLOD> ();
		quad = gameObject.GetComponent<MeshFilter> ().mesh;
		SetUVs ();
		SetMaterial (0);
	}
    
	public void Update ()
	{
		UpdateRotation ();
		UpdateAnimation ();
		LookAtCamera ();
	}

	private void LookAtCamera ()
	{
		Vector3 cPos = cameraTransform.position;
		Vector3 tPos = impostorTransform.position;
		impostorTransform.LookAt (new Vector3 (cPos.x, tPos.y, cPos.z));
		impostorTransform.Rotate (new Vector3 (0, 180f, 0));
	}

	public void UpdateAnimation ()
	{
		SetAnimationPercentage (characterGazeLOD.NormalizedTime);
	}

	public void UpdateRotation ()
	{
		Vector3 parentForward = parentTransform.forward;
		Vector3 cameraToObject = parentTransform.position - cameraTransform.position;
		cameraToObject.y = parentForward.y = 0;
		float angle = 180f - Vector3.Angle (cameraToObject, parentForward);
		int index = Mathf.RoundToInt ((angle / 180f) * (numberOfAngles - 1));
		if (index != currentAngleIndex) {
			// Only update when we need to
			SetMaterial (index);
			currentAngleIndex = index;
			Vector3 parentRight = parentTransform.right;
			parentRight.y = 0;
			float dot = Vector3.Dot (cameraToObject, parentRight);
			if (((dot > 0f) && currentDirection == LEFT) ||
				((dot < 0f) && currentDirection == RIGHT)) {
				// If the dot product is positive the camera is to the right of the player
				// and vice versa. If thats true and current direction is the other way we
				// flip the UVs.
				FlipUVs ();
				currentDirection = (currentDirection + 1) % 2;
			}
		}

	}

	public void FlipUVs ()
	{
		int n = quad.uv.Length;
		Vector2[] uvs = new Vector2[n];
		for (int i = 0; i < n; i++) {
			uvs [i] = quad.uv [(i + 2) % n];
		}
		quad.uv = uvs;
	}

	public void SetFrame (int frameNumber)
	{
		float x = (0.125f * (frameNumber % 8));
		float y = 0.5f - (0.5f * (frameNumber / 8));
		Vector2 offset = new Vector2 (x, y);
		quad.uv = new Vector2[] {
			new Vector2 (x, y),
			new Vector2 (x + 0.125f, 0.5f + y),
			new Vector2 (x + 0.125f, y),
			new Vector2 (x, 0.5f + y)
		};
	}

	public float GetAnimationPercentage ()
	{
		return ((float)frameIndex / (float)numberOfFrames);
	}

	public void SetAnimationPercentage (float percent)
	{
		frameIndex = Mathf.RoundToInt (percent * numberOfFrames);
		frameIndex = (frameIndex == numberOfFrames) ? 0 : frameIndex;
		SetFrame (frameIndex);
	}

	private void SetUVs ()
	{
		quad.uv = new Vector2[] {
            new Vector2 (0f, 0.5f),
            new Vector2 (0.125f, 1f),
            new Vector2 (0.125f, 0.5f),
            new Vector2 (0f, 1f)
        };
	}

	private void SetMaterial (int frameIndex)
	{
		impostorRenderer.sharedMaterial = Materials.Instance.GetMaterial (frameIndex);
		SetFrame (frameIndex);
	}
}
