using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour
{

    private string introText =
@"
Tex text text";

    private GUIStyle introStyle;
    private GameObject startCamera;
    private LockCursor[] lockCursor;

    public void Awake()
    {
        // Disable all lock cursor scripts
        LockCursor(false);

        // Use the start camera
        startCamera = transform.FindChild("StartCamera").gameObject;
        startCamera.SetActive(true);
        Settings.cameraTransform = startCamera.GetComponent<Camera>().transform;


        // GUI style
        introStyle = new GUIStyle();
        introStyle.alignment = TextAnchor.MiddleCenter;
        introStyle.fontSize = 32;
        introStyle.normal.textColor = Color.black;
        //Font font = new Font();
        //introStyle.font = font;

        Utils.Instance.SetGazePoint(true);
    }

    protected virtual void OnGUI()
    {
        GUI.BeginGroup(new Rect(Screen.width / 2 - 220, Screen.height / 2, Screen.width, Screen.height));
   
        if (GUI.Button(new Rect(0, 10, 200, 20), "First person controller")) {
            Player();
            Disable(true);
        }
        if (GUI.Button(new Rect(220, 10, 200, 20), "Free view")) {
            FreeView();
            Disable(false);
        }
        
        GUI.EndGroup();
        GUI.Label(new Rect(0, -Screen.height * 0.15f, Screen.width, Screen.height), introText, introStyle);
    }

    private void Player()
    {
        GameObject player = Instantiate(Resources.Load("Player")) as GameObject;
        SetPosition(player);

    }

    private void FreeView()
    {
        GameObject freeView = Instantiate(Resources.Load("FreeViewCamera")) as GameObject;
        SetPosition(freeView);
    }

    private void Disable(bool lol)
    {
        startCamera.GetComponent<Camera>().enabled = false;
        startCamera.tag = "";
        Settings.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Settings.cameraTransform = Settings.camera.transform;
        Utils.Instance.SetGazePoint(false);
        LockCursor(true);
        Destroy(startCamera);
        Destroy(gameObject);
    }

    private void SetPosition(GameObject g)
    {
        g.transform.position = startCamera.transform.position;
    }

    private void LockCursor(bool enabled)
    {
        lockCursor = GameObject.FindObjectsOfType(typeof(LockCursor)) as LockCursor[];
        foreach (LockCursor l in lockCursor) {
            l.enabled = enabled;
        }
    }

}
