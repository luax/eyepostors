using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Materials {
	private static int numberOfAngles = Settings.numberOfAngles;
	private static int numberOfColors = Settings.numberOfColors;

	private const int numberOfQualities = 2;
	public const int MediumQuality = 1;
	public const int LowQuality = 0;

	private static Material[,,,] impostorMaterials;
	private static Material[] meshMaterials;
	private static Color[] colors;
	private static Dictionary<int, Vector2[]> uvs;

	static Materials () {
		uvs = new Dictionary<int, Vector2[]> ();
		for (int i = 0; i < Settings.numberOfFrames; i++) {
			uvs.Add (i, CalculateUV (i));
		}
        
		impostorMaterials = new Material[numberOfColors, numberOfAngles, numberOfAngles, numberOfQualities];
		meshMaterials = new Material[numberOfColors];
		colors = new Color[numberOfColors];
		for (int i = 0; i < numberOfColors; i++) {
			colors [i] = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1f);
		}
		
		for (int color = 0; color < Settings.numberOfColors; color++) {
			for (int x = 0; x < numberOfAngles; x++) {
				for (int y = 0; y < numberOfAngles; y++) {
					impostorMaterials [color, x, y, 0] = MakeMaterial (color, x, y, 0);
					impostorMaterials [color, x, y, 1] = MakeMaterial (color, x, y, 1);
				}
			}
			meshMaterials [color] = MakeMaterial (color);
		}
	}
	
	public static Material GetMaterial (int color) {
		return meshMaterials [color];
	}

	public static Material GetMaterial (int color, int x, int y, int quality) {
		return impostorMaterials [color, x, y, quality];
	}
    
	public static Vector2[] GetUV (int i) {
		return uvs [i];
	}

	private static Material MakeMaterial (int color) {
		Material mat = new Material (Shader.Find ("ShirtColor/Diffuse"));
		mat.SetColor ("_ShirtColor", colors [color]);
		mat.SetTexture ("_MainTex", (Texture)Resources.Load ("ff"));
		return mat;
	}

	private static Material MakeMaterial (int color, int indexX, int indexY, int quality) {
		Material mat = new Material (Shader.Find ("ShirtColor/Transparent"));
		mat.SetColor ("_ShirtColor", colors [color]);
		if (quality == MediumQuality) {
			mat.SetTexture ("_MainTex", (Texture)Resources.Load ("GeneratedTextures/Medium/texture" + indexX + "_" + indexY));
			mat.SetTexture ("_BumpMap", (Texture)Resources.Load ("GeneratedTextures/Medium/normal" + indexX + "_" + indexY));
		} else {
			mat.SetTexture ("_MainTex", (Texture)Resources.Load ("GeneratedTextures/Low/texture" + indexX + "_" + indexY));
			mat.SetTexture ("_BumpMap", (Texture)Resources.Load ("GeneratedTextures/Low/normal" + indexX + "_" + indexY));
		}
		return mat;
	}
    
	private static Vector2[] CalculateUV (int i) {
		float x = (0.125f * (i % 8));
		float y = 0.5f - (0.5f * (i / 8));
        
		return new Vector2[] { 
            new Vector2 (x + 0.125f, y),
            new Vector2 (x, 0.5f + y),
            new Vector2 (x, y),
            new Vector2 (x + 0.125f, 0.5f + y)}; 
	}
}