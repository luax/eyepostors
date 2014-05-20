using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureCharacter : MonoBehaviour
{
    public Texture2D charTexture;
    public Texture2D normalMap;
    public int textureSize = 1024;
    public int numberOfTextures = 1;
    
    public bool CaptureFinished { get; set; }
    public Texture2D[] Textures { get; set; }
    public Texture2D[] NormalMaps { get; set; }
    
    private Color normalBGColor;
    private int frameSize;
    private int numberOfFrames;
    private int frame = 0;
    private int index = 0;
    private int state;
    private const int TEXTURE = 0;
    private const int NORMAL = 1;
    
    void Start() {   
        frameSize = textureSize / 4;
        Textures = new Texture2D[numberOfTextures];
        NormalMaps = new Texture2D[numberOfTextures];
        for(int i = 0; i < Textures.Length; i++) {
            Textures [i] = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            NormalMaps [i] = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        }
        normalBGColor = new Color(127f, 127f, 254f, 255f) / 255f;
        numberOfFrames = (int)(Mathf.Pow(textureSize, 2) / Mathf.Pow(frameSize, 2));
        frame = 0;
        state = TEXTURE;
        renderer.material.mainTexture = charTexture;
        StartCoroutine(CaptureFrames());
    }
    
    IEnumerator CaptureFrames() {
        for(index = 0; index < numberOfTextures; index++) {
            while(frame < numberOfFrames) {
                yield return new WaitForEndOfFrame();
                Capture();
                if(state == NORMAL)
                    frame++;
                state = (state + 1) % 2;
                renderer.material.mainTexture = (state == TEXTURE) ? charTexture : normalMap;
                Camera.main.backgroundColor = (state == TEXTURE) ? Color.clear : normalBGColor;
            }
            
            SaveToPNG("texture" + index + ".png", Textures [index]);
            SaveToPNG("normal" + index + ".png", NormalMaps [index]);
        }
        CaptureFinished = true;
    }
    
    // Return part of the screen in a texture.
    public void Capture() {
        int rows = textureSize / frameSize;
        int x = frameSize * (frame % rows);
        int y = textureSize - frameSize * ((frame / rows) + 1);
        Texture2D newTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        newTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
        TextureScale.Bilinear(newTexture, frameSize, frameSize);   
        if(state == TEXTURE) {
            Textures [index].SetPixels(x, y, frameSize, frameSize, newTexture.GetPixels());
            Textures [index].Apply(false);
        } else {
            NormalMaps [index].SetPixels(x, y, frameSize, frameSize, newTexture.GetPixels());
            NormalMaps [index].Apply(false);
        }
    }
    
    public static void SaveToPNG(string path, Texture2D tex) {
        File.WriteAllBytes(path, tex.EncodeToPNG());
    }
}