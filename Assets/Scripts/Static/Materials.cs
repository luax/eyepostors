using UnityEngine;
using System.Collections;

public static class Materials
{
    private static int numberOfAngles = Settings.numberOfAngles;
    private static Material[,] materials;
    private static string shader = "ShirtColor/Transparent";

    static Materials()
    {
        materials = new Material[numberOfAngles, numberOfAngles];
        for (int x = 0; x < numberOfAngles; x++) {
            for (int y = 0; y < numberOfAngles; y++) {
                materials[x, y] = MakeMaterial(x, y);
            }
        }
    }

    public static Material GetMaterial(int x, int y)
    {
        return materials[x, y];
    }

    private static Material MakeMaterial(int indexX, int indexY)
    {
        Material mat = new Material(Shader.Find(shader));
        mat.SetTexture("_MainTex", (Texture)Resources.Load("GeneratedTextures/texture" + indexX + "_" + indexY));
        mat.SetTexture("_BumpMap", (Texture)Resources.Load("GeneratedTextures/normal" + indexX + "_" + indexY));

        return mat;
    }
}