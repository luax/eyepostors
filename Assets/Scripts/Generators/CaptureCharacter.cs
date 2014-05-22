using UnityEngine;
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

	public Texture2D[] Textures { get; set; }

	public Texture2D[] NormalMaps { get; set; }
    
	private Animator animator;
	private Transform myTransform;
	private Transform midGeoTransform;
	private GenerateNormals normalGenerator;
	private SkinnedMeshRenderer myRenderer;
	private int textureSizeVertical;
	private int frameSizeVertical;
	private int frameSizeHorizontal;
	private int frame = 0;
	private int index = 0;
	private float currentRotation = 0f;
	private float currentNormalizedTime = 0f;
	private float startTime;
	private Color[] blankFrame;
	private Color[] blankNormalFrame;

	void Start()
	{   	
		myTransform = gameObject.transform;
		midGeoTransform = myTransform.FindChild("CarlMidGeo");
		myRenderer = midGeoTransform.gameObject.GetComponent<SkinnedMeshRenderer>();
		normalGenerator = GetComponent<GenerateNormals>();
		animator = GetComponent<Animator>();
		animator.speed = 0f;
		
		textureSizeVertical = textureSizeHorizontal / 2;
		frameSizeHorizontal = textureSizeHorizontal / 8;
		frameSizeVertical = textureSizeVertical / 2;
		Textures = new Texture2D[numberOfAngles];
		NormalMaps = new Texture2D[numberOfAngles];
		
		int numPixels = frameSizeHorizontal * frameSizeVertical;
		blankFrame = new Color[numPixels];
		blankNormalFrame = new Color[numPixels];
		Color normalColor = new Color(0.5f, 0.5f, 1f, 1f);
		for (int i = 0; i < numPixels; i++)
		{
			blankFrame [i] = Color.clear;
			blankNormalFrame [i] = normalColor;
		}
		
		for (int i = 0; i < Textures.Length; i++)
		{
			Textures [i] = new Texture2D(textureSizeHorizontal, textureSizeVertical, TextureFormat.ARGB32, false);
			NormalMaps [i] = new Texture2D(textureSizeHorizontal, textureSizeVertical, TextureFormat.ARGB32, false);
		}
		Camera.main.backgroundColor = Color.clear;
		myRenderer.material.mainTexture = charTexture;
		StartCoroutine(CaptureFrames());
	}
    
	private IEnumerator CaptureFrames()
	{
		startTime = Time.time;
		int framesInRow = textureSizeHorizontal / frameSizeHorizontal;
		int framesInColumn = textureSizeVertical / frameSizeVertical;
		int totalFrames = framesInRow * framesInColumn;
		for (index = 0; index < numberOfAngles; index++)
		{
			for (frame = 0; frame < numberOfFrames; frame++)
			{
				yield return new WaitForEndOfFrame();
				Capture();
				UpdateAnimation();
			}
			
			//Fill rest of image with blank pixels
			for (int i = frame; i < totalFrames; i++)
			{
				FillBlankFrame(i, Textures [index], blankFrame);
				FillBlankFrame(i, NormalMaps [index], blankNormalFrame);
			}
			
			RotateObject();
			if (saveTextures)
			{
				SaveToPNG("texture" + index + ".png", Textures [index]);
				SaveToPNG("normal" + index + ".png", NormalMaps [index]);
			}
			frame = 0;
		}
		CaptureFinished = true;
		Debug.Log("Time to generate imposter: " + (Time.time - startTime) + " seconds");
	}
    
	// Return part of the screen in a texture.
	private void Capture()
	{
		Texture2D textureFrame = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
		textureFrame.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
		Texture2D normalFrame = normalGenerator.GetNormal();
		SetFrameInTexture(Textures [index], textureFrame);
		SetFrameInTexture(NormalMaps [index], normalFrame);
	}
	
	private void FillBlankFrame(int frame, Texture2D texture, Color[] colors)
	{
		Vector2 px = GetXY(frame);
		int x = (int)px.x;
		int y = (int)px.y;
		texture.SetPixels(x, y, frameSizeHorizontal, frameSizeVertical, colors);
		texture.Apply(false);
	}
    
	private void SetFrameInTexture(Texture2D texture, Texture2D textureFrame)
	{
		Vector2 px = GetXY(frame);
		int x = (int)px.x;
		int y = (int)px.y;
		TextureScale.Bilinear(textureFrame, frameSizeHorizontal, frameSizeVertical); 
		texture.SetPixels(x, y, frameSizeHorizontal, frameSizeVertical, textureFrame.GetPixels());
		texture.Apply(false);
	}
	
	private Vector2 GetXY(int frame)
	{
		int columns = textureSizeHorizontal / frameSizeHorizontal;		
		int x = frameSizeHorizontal * (frame % columns);
		int y = textureSizeVertical - frameSizeVertical * ((frame / columns) + 1);
		return new Vector2(x, y);
	}

	private void UpdateAnimation()
	{
		currentNormalizedTime += (float)(1f / numberOfFrames);
		animator.ForceStateNormalizedTime(currentNormalizedTime);
		midGeoTransform.position = new Vector3(0f, 0f, 0f);
		myTransform.position = new Vector3(0f, 0f, 0f);
	}

	private void RotateObject()
	{
		currentRotation += (float)(180f / (numberOfAngles - 1));
		midGeoTransform.localEulerAngles = -new Vector3(0, currentRotation, 0);
		myTransform.eulerAngles = new Vector3(0, currentRotation, 0);
		currentNormalizedTime = 0f;
	}
	
	public static void SaveToPNG(string name, Texture2D tex)
	{
		File.WriteAllBytes(Application.dataPath + "/Resources/GeneratedTextures/" + name, tex.EncodeToPNG());
	}
}