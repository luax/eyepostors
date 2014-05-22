using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureCharacter : MonoBehaviour
{
	public Texture2D charTexture;
	public int textureSize = 1024;
	public int numberOfAngles = 15;
	public int numberOfFrames = 15;
	public bool saveTextures = true;

	public bool CaptureFinished { get; set; }
	public Texture2D[] Textures { get; set; }
	public Texture2D[] NormalMaps { get; set; }
    
	private Animator animator;
	private Transform myTransform;
	private Transform midGeoTransform;
	private GenerateNormals normalGenerator;
	private SkinnedMeshRenderer myRenderer;
	
	private int frameSize;
	private int frame = 0;
	private int index = 0;
	private float currentRotation = 0f;
	private float currentNormalizedTime = 0f;
	private float startTime;

	void Start ()
	{   	
		myTransform = gameObject.transform;
		midGeoTransform = myTransform.FindChild ("CarlMidGeo");
		myRenderer = midGeoTransform.gameObject.GetComponent<SkinnedMeshRenderer> ();
		normalGenerator = GetComponent<GenerateNormals> ();
		animator = GetComponent<Animator> ();
		animator.speed = 0f;
		
		frameSize = textureSize / 4;
		Textures = new Texture2D[numberOfAngles];
		NormalMaps = new Texture2D[numberOfAngles];
		for (int i = 0; i < Textures.Length; i++) {
			Textures [i] = new Texture2D (textureSize, textureSize, TextureFormat.ARGB32, false);
			NormalMaps [i] = new Texture2D (textureSize, textureSize, TextureFormat.ARGB32, false);
		}
		Camera.main.backgroundColor = Color.clear;
		myRenderer.material.mainTexture = charTexture;
		StartCoroutine (CaptureFrames ());
	}
    
	private IEnumerator CaptureFrames ()
	{
		startTime = Time.time;
		for (index = 0; index < numberOfAngles; index++) {
			for (frame = 0; frame < numberOfFrames; frame++) {
				yield return new WaitForEndOfFrame ();
				Capture ();
				UpdateAnimation ();
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
    
	// Return part of the screen in a texture.
	private void Capture ()
	{
		Texture2D textureFrame = new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);
		textureFrame.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0, false);
		Texture2D normalFrame = normalGenerator.GetNormal ();
		SetFameInTexture (Textures [index], textureFrame);
		SetFameInTexture (NormalMaps [index], normalFrame);
	}
	
	private void SetFameInTexture (Texture2D texture, Texture2D textureFrame)
	{
		int rows = textureSize / frameSize;
		int x = frameSize * (frame % rows);
		int y = textureSize - frameSize * ((frame / rows) + 1);
		TextureScale.Bilinear (textureFrame, frameSize, frameSize); 
		texture.SetPixels (x, y, frameSize, frameSize, textureFrame.GetPixels ());
		texture.Apply (false);
	}

	private void UpdateAnimation ()
	{
		currentNormalizedTime += (float)(1f / numberOfFrames);
		animator.ForceStateNormalizedTime (currentNormalizedTime);
		midGeoTransform.position = new Vector3 (0f, 0f, 0f);
		myTransform.position = new Vector3 (0f, 0f, 0f);
	}

	private void RotateObject ()
	{
		currentRotation += (float)(180f / numberOfAngles);
		midGeoTransform.localEulerAngles = -new Vector3 (0, currentRotation, 0);
		myTransform.eulerAngles = new Vector3 (0, currentRotation, 0);
		currentNormalizedTime = 0f;
	}
	
	public static void SaveToPNG (string name, Texture2D tex)
	{
		File.WriteAllBytes (Application.dataPath + "/Resources/GeneratedTextures/" + name, tex.EncodeToPNG ());
	}
}