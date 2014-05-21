using UnityEngine;
using System.Collections;

public class Materials : Singleton<Materials>
{
    public int numberOfAngles = 15;
    private Material[] materials;

    void Awake()
    {
        materials = new Material[numberOfAngles];
        for (int i = 0; i < numberOfAngles; i++)
        {
            materials[i] = MakeMaterial(i);
        }
    }

    public Material GetMaterial(int index)
    {
        return materials[index];
    }

    private Material MakeMaterial(int index)
    {
        Material mat = new Material(Shader.Find("Transparent/Cutout/Bumped Diffuse"));
        mat.SetTexture("_MainTex", (Texture)Resources.Load("GeneratedTextures/texture" + index));
        mat.SetTexture("_BumpMap", (Texture)Resources.Load("GeneratedTextures/normal" + index));
        return mat;
    }
}