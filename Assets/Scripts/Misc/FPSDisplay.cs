using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    public int FramesPerSec { get; protected set; }
    public float updateFrequency = 0.5f;
    public Font font;

    private string text;
    private GUIStyle style;
    private Rect pos;

    private void Start()
    {
        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.font = font;
        style.normal.textColor = new Color(1, 1, 1, 1.0f);
        pos = new Rect(5, 5, w, h * 2 / 100);
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (;;) {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(updateFrequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
            
            FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            text = FramesPerSec.ToString() + " fps";
        }
    }
    void OnGUI()
    {
        GUI.Label(pos, text, style);
    }
}