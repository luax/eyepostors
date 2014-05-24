using UnityEngine;
using System.Collections;

public static class Materials
{
	private static int numberOfAngles = Settings.numberOfAngles;
	private static int numberOfColors = Settings.numberOfColors;
	private static Material[,,] impostorMaterials;
	private static Material[] meshMaterials;
	private static string shader = "ShirtColor/Transparent";
	private static Color[] colors;

	static Materials()
	{
		impostorMaterials = new Material[numberOfColors, numberOfAngles, numberOfAngles];
		meshMaterials = new Material[numberOfColors];
		colors = new Color[numberOfColors];
		for (int i = 0; i < numberOfColors; i++) {
			colors [i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		}
		
		for (int color = 0; color < Settings.numberOfColors; color++) {
			for (int x = 0; x < numberOfAngles; x++) {
				for (int y = 0; y < numberOfAngles; y++) {
					impostorMaterials [color, x, y] = MakeMaterial(color, x, y);
				}
			}
			meshMaterials [color] = MakeMaterial(color);
		}
	}
	
	public static Material GetMaterial(int color)
	{
		return meshMaterials [color];
	}

	public static Material GetMaterial(int color, int x, int y)
	{
		return impostorMaterials [color, x, y];
	}

	private static Material MakeMaterial(int color)
	{
		Material mat = new Material(Shader.Find("ShirtColor/Diffuse"));
		mat.SetColor("_ShirtColor", colors [color]);
		mat.SetTexture("_MainTex", (Texture)Resources.Load("ff"));
		return mat;
	}

	private static Material MakeMaterial(int color, int indexX, int indexY)
	{
		Material mat = new Material(Shader.Find("ShirtColor/Transparent"));
		mat.SetColor("_ShirtColor", colors [color]);
		mat.SetTexture("_MainTex", (Texture)Resources.Load("GeneratedTextures/texture" + indexX + "_" + indexY));
		mat.SetTexture("_BumpMap", (Texture)Resources.Load("GeneratedTextures/normal" + indexX + "_" + indexY));
		return mat;
	}
}