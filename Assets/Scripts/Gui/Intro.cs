using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

    private string introText =
@"
Press any key to continue...";

    private GUIStyle introStyle;

    private Camera mainCamera;
    private GameObject startCamera;

	public void Start () {
        // Change camera
        mainCamera = Camera.main;
        //mainCamera.enabled = false; // Only disable main camera
        startCamera = (GameObject)Instantiate(Resources.Load("StartCamera"));
        startCamera.SetActive(true);

        // GUI style
		introStyle = new GUIStyle();
		introStyle.alignment = TextAnchor.MiddleCenter;
		introStyle.fontSize = 32;
		introStyle.normal.textColor = Color.black;
        //Font font = new Font();
        //introStyle.font = font;

        Utils.Instance.SetGazePoint(true);
	}
	
	// Update is called once per frame
    public void Update()
    {
        if (Input.anyKey)
        {
            Utils.Instance.SetGazePoint(false);
            Destroy(startCamera);
            mainCamera.enabled = true;
            enabled = false; // Disable this script
        }
	}

    protected virtual void OnGUI()
	{
		GUI.Label(new Rect(0, -Screen.height * 0.15f, Screen.width, Screen.height), introText, introStyle);
	}
}
