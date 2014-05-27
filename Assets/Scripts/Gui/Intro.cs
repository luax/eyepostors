﻿using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour
{

    private string introText =
@"
Press escape to start ...
";

    private GUIStyle introStyle;
    private GameObject startCamera;
    private LockCursor[] lockCursor;
    private bool displayIntroText = true;

    public void Awake()
    {
        // Disable all lock cursor scripts
        LockCursor(false);

        // Use the start camera
        startCamera = transform.FindChild("StartCamera").gameObject;
        startCamera.SetActive(true);
        Settings.camera = startCamera.GetComponent<Camera>();
        Settings.cameraTransform = Settings.camera.transform;

        // GUI style
        introStyle = new GUIStyle();
        introStyle.alignment = TextAnchor.MiddleCenter;
        introStyle.fontSize = 32;
        introStyle.normal.textColor = Color.white;
        introStyle.fontStyle = FontStyle.Bold;
        //introStyle.font = (Font)Resources.Load("fonts/HelveticaCY");

        Utils.Instance.SetGazePoint(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            displayIntroText = !displayIntroText;
        }

    }

    protected virtual void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            displayIntroText = true;
        }
        if (displayIntroText) {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), introText, introStyle);
        }
    }

    private void LockCursor(bool enabled)
    {
        lockCursor = GameObject.FindObjectsOfType(typeof(LockCursor)) as LockCursor[];
        foreach (LockCursor l in lockCursor) {
            l.enabled = enabled;
        }
    }

}
