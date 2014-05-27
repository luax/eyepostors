using UnityEngine;
using System.Collections;

public class Utils : Singleton<Utils>
{
    private bool gazePoint;
    private GameObject gazePointVisualiser;

    public bool GetGazePointStatus()
    {
        return gazePoint;
    }

    public void ToggleGazePoint()
    {
        SetGazePoint(!gazePoint);
    }

    public void SetGazePoint(bool enabled)
    {
        gazePoint = enabled;
        if (gazePoint) {
            gazePointVisualiser = Instantiate(Resources.Load("GazePointVisualizer")) as GameObject;
        } else {
            Destroy(gazePointVisualiser);
        }
    }

    public void Pause(bool b)
    {
        Time.timeScale = b ? 0 : 1;
        Screen.lockCursor = !b;
        Screen.showCursor = b;
        LockScreen(b);
    }

    public void LockScreen(bool b)
    {
        MouseLook[] mouseLook = GameObject.FindObjectsOfType(typeof(MouseLook)) as MouseLook[];
        foreach (MouseLook m in mouseLook) {
            m.enabled = !b;
        }
    }

    public void CreateFirstPerson()
    {
        GameObject currentCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Settings.camera.tag = "";

        GameObject player = Instantiate(Resources.Load("Player")) as GameObject;
        player.transform.position = Settings.cameraTransform.position;

        UpdateCamera();
        Destroy(currentCamera.transform.root.gameObject);
    }

    public void CreateFreeView()
    {
        GameObject currentCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Settings.camera.tag = "";

        GameObject freeView = Instantiate(Resources.Load("FreeViewCamera")) as GameObject;
        freeView.transform.position = Settings.cameraTransform.position;
        freeView.transform.rotation = Settings.cameraTransform.rotation;

        UpdateCamera();
        Destroy(currentCamera.transform.root.gameObject);
    }

    public void UpdateCamera()
    {
        Settings.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Settings.cameraTransform = Settings.camera.transform;
    }

}
