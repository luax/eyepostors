using UnityEngine;
using System.Collections;

public class lol : MonoBehaviour {

    public Texture texture;
    public Texture normalMap;
    private int frame;
    private Mesh mesh;

    // Use this for initialization
    void Start () {
        mesh = gameObject.GetComponent<MeshFilter> ().mesh;
        foreach (Vector2 v in mesh.uv) {
            Debug.Log (v);
        }
        Debug.Log (mesh.uv);
        mesh.uv = new Vector2[] {
            new Vector2 (0f, 0.75f),
            new Vector2 (0.25f, 1f),
            new Vector2 (0.25f, 0.75f),
            new Vector2 (0f, 1f)
        };
        renderer.material.SetTexture ("_MainTex", texture);
        renderer.material.SetTexture ("_BumpMap", normalMap);
    }
	
    // Update is called once per frame
    void Update () {
        int frames = 5;
        if (frame % frames == 0) {
            SetFrame ((frame / frames) % 15);
        }
        frame++;
    }

    void SetFrame (int frameNumber) {
        float x = (0.25f * (frameNumber % 4));
        float y = (-0.25f * (frameNumber / 4));
        Vector2 offset = new Vector2 (x, y);
        Debug.Log ("Framenumber " + frameNumber + " (x: " + offset.x + " ,y: " + offset.y + ")");
        renderer.material.SetTextureOffset ("_MainTex", offset);
        renderer.material.SetTextureOffset ("_BumpMap", offset);
    }
}
