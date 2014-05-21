using UnityEngine;
using System.Collections;

public class Imposter : MonoBehaviour
{
	public GameObject parent;
	public float FPS;
	public int numberOfFrames = 15;
	public int numberOfAngles = 8;
	private Transform parentTransform;
	private Texture[] textures;
	private Texture[] normalMaps;
	private float frameTime;
	private int frame;
	private Mesh quad;
	private Transform cameraTransform;
	private int currentAngleIndex;
	private int currentDirection;
	private const int RIGHT = 0;
	private const int LEFT = 1;

	// Use this for initialization
	void Start ()
	{
		SetUVs ();
		GetTextures ();
		cameraTransform = Camera.main.transform;
		parentTransform = parent.transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateRotation ();
		UpdateAnimation ();
		LookAtCamera ();
	}

	private void LookAtCamera ()
	{
		Vector3 cPos = cameraTransform.position;
		Vector3 tPos = transform.position;
		transform.LookAt (new Vector3 (cPos.x, tPos.y, cPos.z));
		transform.Rotate (new Vector3 (0, 180f, 0));
	}

	public void UpdateAnimation ()
	{
		frameTime += Time.deltaTime;
		if (frameTime > (1f / FPS)) {
			frame++;
			if (frame >= numberOfFrames) {
				frame = 0;
			}
			SetFrame (frame);
			frameTime = 0;
		}
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
			renderer.material.SetTexture ("_MainTex", textures [index]);
			renderer.material.SetTexture ("_BumpMap", normalMaps [index]);
			currentAngleIndex = index;
			Vector3 parentRight = parent.transform.right;
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
		Vector2 offset = renderer.material.GetTextureOffset ("_MainTex");
		return (int)((offset.x / 0.25f) + (4f * offset.y / -0.25f));
	}

	public void SetFrame (int frameNumber)
	{
		float x = (0.25f * (frameNumber % 4));
		float y = (-0.25f * (frameNumber / 4));
		Vector2 offset = new Vector2 (x, y);
		renderer.material.SetTextureOffset ("_MainTex", offset);
		renderer.material.SetTextureOffset ("_BumpMap", offset);
	}

	public float GetAnimationPercentage ()
	{
		return ((float)frame / (float)numberOfFrames);
	}

	public void SetAnimationPercentage (float percent)
	{
		SetFrame (Mathf.RoundToInt (percent * numberOfFrames));
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

	private void GetTextures ()
	{
		textures = new Texture[numberOfAngles];
		normalMaps = new Texture[numberOfAngles];
		for (int i = 0; i < numberOfAngles; i++) {
			textures [i] = (Texture)Resources.Load ("GeneratedTextures/texture" + i);
			normalMaps [i] = (Texture)Resources.Load ("GeneratedTextures/normal" + i);
		}
		renderer.material.SetTexture ("_MainTex", textures [0]);
		renderer.material.SetTexture ("_BumpMap", normalMaps [0]);
	}
}
