using UnityEngine;
using System.Collections;

public class Impostor : MonoBehaviour
{
	public int numberOfFrames = 15;
	public int numberOfAngles = 15;
	
	// Component references
	private Animator animator;
	private Transform impostorTransform;
	private Transform parentTransform;
	private Transform cameraTransform;
	private Renderer impostorRenderer;
	private Mesh quad;

	private int frame;
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
		animator = impostorTransform.parent.gameObject.GetComponent<Animator> ();
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
		float time = animator.GetCurrentAnimatorStateInfo (0).normalizedTime;
		time -= (int)time;
		SetAnimationPercentage (time);
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

	public int GetFrame ()
	{
		Vector2 offset = impostorRenderer.material.GetTextureOffset ("_MainTex");
		return (int)((offset.x / 0.25f) + (4f * offset.y / -0.25f));
	}

	public void SetFrame (int frameNumber)
	{
		float x = (0.25f * (frameNumber % 4));
		float y = (-0.25f * (frameNumber / 4));
		Vector2 offset = new Vector2 (x, y);
		impostorRenderer.material.SetTextureOffset ("_MainTex", offset);
		impostorRenderer.material.SetTextureOffset ("_BumpMap", offset);
	}

	public float GetAnimationPercentage ()
	{
		return ((float)frame / (float)numberOfFrames);
	}

	public void SetAnimationPercentage (float percent)
	{
		int index = Mathf.RoundToInt (percent * numberOfFrames);
		index = (index == numberOfFrames) ? 0 : index;
		SetFrame (index);
	}

	private void SetUVs ()
	{
		quad = gameObject.GetComponent<MeshFilter> ().mesh;
		quad.uv = new Vector2[] {
			new Vector2 (0f, 0.75f),
			new Vector2 (0.25f, 1f),
			new Vector2 (0.25f, 0.75f),
			new Vector2 (0f, 1f)
		};
	}

	private void SetMaterial (int index)
	{
		impostorRenderer.material = Materials.Instance.GetMaterial (index);
		SetFrame (frame);
	}
}
