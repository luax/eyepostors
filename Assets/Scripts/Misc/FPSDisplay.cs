using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    public Font font;
    private float deltaTime = 0.0f;
    private GUIStyle style;
    private Rect pos;
    private string text;
    
    void Start()
    {
        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.font = font;
        style.normal.textColor = new Color(1, 1, 1, 1.0f);
        pos = new Rect(5, 5, w, h * 2 / 100);
    }
    
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        text = string.Format("FPS: {0:0.0}", fps);
    }
    
    void OnGUI()
    {
        GUI.Label(pos, text, style);
    }
}