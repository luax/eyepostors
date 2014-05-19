using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureCharacter : MonoBehaviour
{
    public int textureSize = 1024;
    public int frameSize = 256;
    public int numberOfTextures = 1;
    private int numberOfFrames;
    private int frame = 0;
    private int index;
    private Texture2D[] textures;
    
    void Start()
    {
        textures = new Texture2D[numberOfTextures];
        for (int i = 0; i < textures.Length; i++)
        {
            textures [i] = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        }
        numberOfFrames = (int)(Mathf.Pow(textureSize, 2) / Mathf.Pow(frameSize, 2));
    }
    
    // Update is called once per frame
    void Update()
    {
        
        if (frame < numberOfFrames)
        {
            StartCoroutine(GetCapture(frame));
            frame++;     
        }
    }
    
    IEnumerator GetCapture(int frame)
    {
        yield return new WaitForEndOfFrame();
        Capture(frame);
        if (frame == numberOfFrames - 1)
        {
            renderer.material.mainTexture = textures [index];
            SaveToPNG(textures [index], index);
            index++;
        }
    }
    
    // Return part of the screen in a texture.
    public void Capture(int frame)
    {
        int rows = textureSize / frameSize;
        int destX = frameSize * (frame % rows);
        int destY = frameSize * ((frame / rows));
        Texture2D newTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        newTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
        TextureScale.Bilinear(newTexture, frameSize, frameSize);
        textures [index].SetPixels(destX, destY, frameSize, frameSize, newTexture.GetPixels());
        textures [index].Apply(false);
    }
    
    public static void SaveToPNG(Texture2D tex, int index)
    {
        File.WriteAllBytes("texture" + index + ".png", tex.EncodeToPNG());
    }
}