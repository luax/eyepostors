using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    private bool showMenu;
    private GUIStyle textStyle;

    private int topDefault = 10;
    private int top = 10;
    private int left = 20;
    private int width = 200;
    private int height = 20;
    private int menuOffset = 30;
    private int items = 5;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            showMenu = !showMenu;
            Utils.Instance.Pause(showMenu);
        }
    }

    protected virtual void OnGUI()
    {
        if (showMenu) {
            DrawMenu();
        }
        Event e = Event.current;
    }

    protected virtual void DrawMenu()
    {
        Event e = Event.current;
        if (e.keyCode == KeyCode.Return) {
            showMenu = false;
            Utils.Instance.Pause(showMenu);
            return;
        }

        GUI.depth = -1;

        // Background box
        GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 200, Screen.width, Screen.height));
        GUI.Box(new Rect(10, 10, 220, items * menuOffset), "Menu");

        if (GUI.Button(new Rect(left, (top += menuOffset), width, height), "Display gaze point: " + Utils.Instance.GetGazePointStatus())) {
            Utils.Instance.ToggleGazePoint();
        }

        if (GUI.Button(new Rect(left, (top += menuOffset), width, height), "Exit game")) {
            Application.Quit();
        }

        GUI.Label(new Rect(left, (top += menuOffset), width, height), "Number of impostors");

        string text = GUI.TextField(new Rect(left, (top += menuOffset), width, height), Settings.numberOfImpostors.ToString());
        int number;
        if (int.TryParse(text, out number) && number >= Settings.minImpostors && number <= Settings.maxImpostors) {
            Settings.numberOfImpostors = number;
        }

        GUI.EndGroup();
        top = topDefault;
    }
}
