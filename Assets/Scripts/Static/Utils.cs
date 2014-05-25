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
        gazePoint = !gazePoint;
        SetGazePoint(gazePoint);
    }

    public void SetGazePoint(bool enabled) {
        if (gazePoint)
        {
            gazePointVisualiser = Instantiate(Resources.Load("GazePointVisualizer")) as GameObject;
        }
        else
        {
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

}
