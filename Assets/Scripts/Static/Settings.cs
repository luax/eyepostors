using UnityEngine;
using System.Collections;

public enum TriggerOption
{
    Mouse,
    Gaze
}

public static class Settings
{
    // Eye tracking options
    public const TriggerOption triggerOption = TriggerOption.Mouse;
    public const EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;

    // Character gaze LOD
    public const float gazeDistanceHigh = 0.045f;
    public const float gazeDistanceMedium = 1f;
    public const float cooldownTime = 0.5f;

    // Gaze distance
    public const float worldMaxDistance = 25f;
    public const float worldMinDistance = 6f;

    // Impostors
    public const int numberOfAngles = 16;
    public const int numberOfFrames = 16;
    public const int numberOfColors = 15;

    public const int minImpostors = 1;
    public const int defImpostors = 10;
    public const int maxImpostors = 50000;
    public static int numberOfImpostors;

    // Camera
    public static Transform cameraTransform;


    // Screen
    public static float diagonalLength;
    static Settings() {
        if (Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }
        numberOfImpostors = defImpostors;
        int w = Screen.width;
        int h = Screen.height;
        diagonalLength = Mathf.Sqrt(w * w + h * h);
    }

}

