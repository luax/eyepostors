using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureCharacter : MonoBehaviour
{
	public Texture2D charTexture;
	public Texture2D normalMap;
	public int textureSize = 1024;
	public int numberOfAngles = 15;
	public int numberOfFrames = 15;
	public bool saveTextures = true;

	public bool CaptureFinished { get; set; }

	public Texture2D[] Textures { get; set; }

	public Texture2D[] NormalMaps { get; set; }
    
	private Color normalBGColor;
	private int frameSize;

	private SkinnedMeshRenderer myRenderer;

	private int frame = 0;
	private int index = 0;
	private int state;
	private const int TEXTURE = 0;
	private const int NORMAL = 1;
	private Animator animator;
	private Transform myTransform;
	private float currentRotation = 0f;
	private float currentNormalizedTime = 0f;
	private float startTime;

	void Start ()
	{   	
		myRenderer = GetComponent<SkinnedMeshRenderer> ();
		
		animator = gameObject.GetComponent<Animator> ();
		animator.speed = 0f;
		myTransform = gameObject.transform;
		frameSize = textureSize / 4;
		Textures = new Texture2D[numberOfAngles];
		NormalMaps = new Texture2D[numberOfAngles];
		for (int i = 0; i < Textures.Length; i++) {
			Textures [i] = new Texture2D (textureSize, textureSize, TextureFormat.ARGB32, false);
			NormalMaps [i] = new Texture2D (textureSize, textureSize, TextureFormat.ARGB32, false);
		}
		normalBGColor = new Color (127f, 127f, 254f, 255f) / 255f;
		Camera.main.backgroundColor = Color.clear;
		state = TEXTURE;
		myRenderer.material.mainTexture = charTexture;
		StartCoroutine (CaptureFrames ());
	}
    
	private IEnumerator CaptureFrames ()
	{
		startTime = Time.time;
		for (index = 0; index < numberOfAngles; index++) {
			while (frame < numberOfFrames) {
				yield return new WaitForEndOfFrame ();
				Capture ((state == TEXTURE) ? Textures : NormalMaps);
				if (state == NORMAL) {
					UpdateAnimation ();
					frame++;
				}
				SwitchState ();
			}
			RotateObject ();
			if (saveTextures) {
				SaveToPNG ("texture" + index + ".png", Textures [index]);
				SaveToPNG ("normal" + index + ".png", NormalMaps [index]);
			}
			frame = 0;
		}
		CaptureFinished = true;
		Debug.Log ("Time to generate imposter: " + (Time.time - startTime) + " seconds");
	}
	
	private void SwitchState ()
	{
		state = (state + 1) % 2;
		myRenderer.material.mainTexture = (state == TEXTURE) ? charTexture : normalMap;
		Camera.main.backgroundColor = (state == TEXTURE) ? Color.clear : normalBGColor;
	}
    
	// Return part of the screen in a texture.
	private void Capture (Texture2D[] tex)
	{
		int rows = textureSize / frameSize;
		int x = frameSize * (frame % rows);
		int y = textureSize - frameSize * ((frame / rows) + 1);
		Texture2D newTexture = new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);
		newTexture.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0, false);
		TextureScale.Bilinear (newTexture, frameSize, frameSize);   
		tex [index].SetPixels (x, y, frameSize, frameSize, newTexture.GetPixels ());
		tex [index].Apply (false);
	}
    
	public static void SaveToPNG (string name, Texture2D tex)
	{
		File.WriteAllBytes (Application.dataPath + "/Resources/GeneratedTextures/" + name, tex.EncodeToPNG ());
	}

	private void UpdateAnimation ()
	{
		currentNormalizedTime += (float)(1f / numberOfFrames);
		animator.ForceStateNormalizedTime (currentNormalizedTime);
		myTransform.position = new Vector3 (0f, 0f, -2f);
	}

	private void RotateObject ()
	{
		currentRotation += (float)(180f / numberOfAngles);
		gameObject.transform.eulerAngles = new Vector3 (0, currentRotation, 0);
		currentNormalizedTime = 0f;
	}
}