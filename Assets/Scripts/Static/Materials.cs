﻿using UnityEngine;
using System.Collections;

public class Materials : Singleton<Materials>
{
	private int numberOfAngles = Settings.numberOfAngles;
	private Material[,] materials;
	private string shader = "ShirtColor/Transparent";

	void Awake ()
	{
		materials = new Material[numberOfAngles, numberOfAngles];
		for (int x = 0; x < numberOfAngles; x++) {
			for (int y = 0; y < numberOfAngles; y++) {
				materials [x, y] = MakeMaterial (x, y);
			}
		}
	}

	public Material GetMaterial (int x, int y)
	{
		return materials [x, y];
	}
    
	private Material MakeMaterial (int indexX, int indexY)
	{
		Material mat = new Material (Shader.Find (shader));
		mat.SetTexture ("_MainTex", (Texture)Resources.Load ("GeneratedTextures/texture" + indexX + "_" + indexY));
		mat.SetTexture ("_BumpMap", (Texture)Resources.Load ("GeneratedTextures/normal" + indexX + "_" + indexY));

		return mat;
	}
}