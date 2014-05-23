﻿using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureCharacter : MonoBehaviour
{
	public Texture2D charTexture;
	public int textureSizeHorizontal = 1024;
	public int numberOfAngles = 15;
	public int numberOfFrames = 15;
	public bool saveTextures = true;

	public bool CaptureFinished { get; set; }

	public Texture2D[,] Textures { get; set; }

	public Texture2D[,] NormalMaps { get; set; }
    
	private Transform cameraPivot;
	private Animator animator;
	private Transform myTransform;
	private Transform midGeoTransform;
	private GenerateNormals normalGenerator;
	private SkinnedMeshRenderer myRenderer;
	private int textureSizeVertical;
	private int frameSizeVertical;
	private int frameSizeHorizontal;
	private int totalFrames;
	private int frame = 0;
	private int indexY = 0;
	private int indexX = 0;
	private float currentRotation = 0f;
	private float currentNormalizedTime = 0f;
	private float startTime;
	private Color[] blankFrame;
	private Color[] blankNormalFrame;

	void Start ()
	{   	
		myTransform = gameObject.transform;
		midGeoTransform = transform.FindChild ("CarlMidGeo");
		myRenderer = midGeoTransform.GetComponent<SkinnedMeshRenderer> ();
		cameraPivot = GameObject.Find ("CameraPivot").transform;
		normalGenerator = GetComponent<GenerateNormals> ();
		animator = GetComponent<Animator> ();
		animator.speed = 0f;
		
		textureSizeVertical = textureSizeHorizontal / 2;
		frameSizeHorizontal = textureSizeHorizontal / 8;
		frameSizeVertical = textureSizeVertical / 2;
		int framesInRow = textureSizeHorizontal / frameSizeHorizontal;
		int framesInColumn = textureSizeVertical / frameSizeVertical;
		totalFrames = framesInRow * framesInColumn;
		
		Textures = new Texture2D[numberOfAngles, numberOfAngles];
		NormalMaps = new Texture2D[numberOfAngles, numberOfAngles];
		
		int numPixels = frameSizeHorizontal * frameSizeVertical;
		blankFrame = new Color[numPixels];
		blankNormalFrame = new Color[numPixels];
		Color normalColor = new Color (0.5f, 0.5f, 1f, 1f);
		for (int i = 0; i < numPixels; i++) {
			blankFrame [i] = Color.clear;
			blankNormalFrame [i] = normalColor;
		}
		
		for (int x = 0; x < numberOfAngles; x++) {
			for (int y = 0; y < numberOfAngles; y++) {
				Textures [x, y] = new Texture2D (textureSizeHorizontal, textureSizeVertical, TextureFormat.ARGB32, false);
				NormalMaps [x, y] = new Texture2D (textureSizeHorizontal, textureSizeVertical, TextureFormat.ARGB32, false);
			}
		}
		Camera.main.backgroundColor = Color.clear;
		myRenderer.material.mainTexture = charTexture;
		StartCoroutine (CaptureFrames ());
	}
    
	private IEnumerator CaptureFrames ()
	{
		startTime = Time.time;
		Vector3 yAxis = new Vector3 (0f, 1f, 0f);
		Vector3 xAxis = new Vector3 (1f, 0f, 0f);
		for (indexX = 0; indexX < numberOfAngles; indexX++) {
			for (indexY = 0; indexY < numberOfAngles; indexY++) {
				for (frame = 0; frame < numberOfFrames; frame++) {
					yield return new WaitForEndOfFrame ();
					Capture ();
					UpdateAnimation ();
				}
				FillTextures ();
				SaveTextures ();
				RotateCamera (yAxis, 360f);
				frame = 0;
				Debug.Log ("x: " + indexX + ", y: " + indexY);
			}
			ResetYRotation ();
			currentRotation = 0;
			indexY = 0;
			RotateCamera (xAxis, 90f);
		}
		CaptureFinished = true;
		Debug.Log ("Time to generate imposter: " + (Time.time - startTime) + " seconds");
	}
	
	private void FillTextures ()
	{
		for (int i = frame; i < totalFrames; i++) {
			FillBlankFrame (i, Textures [indexX, indexY], blankFrame);
			FillBlankFrame (i, NormalMaps [indexX, indexY], blankNormalFrame);
		}
	}
	
	private void SaveTextures ()
	{
		if (saveTextures) {
			SaveToPNG ("texture" + indexX + "_" + indexY + ".png", Textures [indexX, indexY]);
			SaveToPNG ("normal" + indexX + "_" + indexY + ".png", NormalMaps [indexX, indexY]);
		}
	}
    
	// Return part of the screen in a texture.
	private void Capture ()
	{
		Texture2D textureFrame = new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);
		textureFrame.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0, false);
		Texture2D normalFrame = normalGenerator.GetNormal ();
		SetFrameInTexture (Textures [indexX, indexY], textureFrame);
		SetFrameInTexture (NormalMaps [indexX, indexY], normalFrame);
	}
	
	private void FillBlankFrame (int frame, Texture2D texture, Color[] colors)
	{
		Vector2 px = GetXY (frame);
		int x = (int)px.x;
		int y = (int)px.y;
		texture.SetPixels (x, y, frameSizeHorizontal, frameSizeVertical, colors);
		texture.Apply (false);
	}
    
	private void SetFrameInTexture (Texture2D texture, Texture2D textureFrame)
	{
		Vector2 px = GetXY (frame);
		int x = (int)px.x;
		int y = (int)px.y;
		TextureScale.Bilinear (textureFrame, frameSizeHorizontal, frameSizeVertical); 
		texture.SetPixels (x, y, frameSizeHorizontal, frameSizeVertical, textureFrame.GetPixels ());
		texture.Apply (false);
	}
	
	private Vector2 GetXY (int frame)
	{
		int columns = textureSizeHorizontal / frameSizeHorizontal;		
		int x = frameSizeHorizontal * (frame % columns);
		int y = textureSizeVertical - frameSizeVertical * ((frame / columns) + 1);
		return new Vector2 (x, y);
	}

	private void UpdateAnimation ()
	{
		currentNormalizedTime += (float)(1f / numberOfFrames);
		animator.ForceStateNormalizedTime (currentNormalizedTime);
		midGeoTransform.position = new Vector3 (0f, 0f, 0f);
		myTransform.position = new Vector3 (0f, 0f, 0f);
	}

	private void RotateCamera (Vector3 axis, float totalAngles)
	{
		cameraPivot.eulerAngles -= (float)(totalAngles / (numberOfAngles - 1)) * axis;
		currentNormalizedTime = 0f;
	}
	
	private void ResetYRotation ()
	{
		Vector3 e = cameraPivot.eulerAngles;
		cameraPivot.eulerAngles = new Vector3 (e.x, 0, 0);
	}
	
	public static void SaveToPNG (string name, Texture2D tex)
	{
		File.WriteAllBytes (Application.dataPath + "/Resources/GeneratedTextures/" + name, tex.EncodeToPNG ());
	}
}