using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    private bool showMenu;
    private GUIStyle textStyle;

    private int topDefault = 10;
    private int top = 10;
    private int left = 20;
    private int width = 200;
    private int height = 20;
    private int menuOffset = 30;
    private int items = 2;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu = !showMenu;
            Utils.Instance.Pause(showMenu);
        }
    }

    protected virtual void OnGUI()
    {
        if (showMenu)
        {
            DrawMenu();
        }
    }

    protected virtual void DrawMenu()
    {
        GUI.depth = -1;

        // Background box
        GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 200, Screen.width, Screen.height));
        GUI.Box(new Rect(10, 10, 220, items * menuOffset), "Menu");

        if (GUI.Button(new Rect(left, (top += menuOffset), width, height), "Display gaze point: " + Utils.Instance.GetGazePointStatus()))
        {
            Utils.Instance.ToggleGazePoint();
        }
        GUI.EndGroup();

        top = topDefault;

    }
}
