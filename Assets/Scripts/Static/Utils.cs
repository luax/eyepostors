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
        LockPlayer(b);
    }

    public void LockPlayer(bool b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }
        MouseLook mX = (MouseLook)player.GetComponent(typeof(MouseLook));
        MouseLook mY = (MouseLook)Camera.main.gameObject.GetComponent(typeof(MouseLook));

        if (mX != null)
        {
            mX.enabled = !b;
        }
        if (mY != null)
        {
            mY.enabled = !b;
        }
    }
}
