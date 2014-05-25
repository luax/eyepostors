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

    // Distance and level of detail settings
    public static class Distance
    {
        public const float highestLOD = 0.045f;
        public const float mediumLOD = 1f;
        public const float lowestLOD = 2f;
        public const float minimalLOD = 3f;

        public const float worldMaximum = 40f;
        public const float worldMedium = 20f;
        public const float worldMinimum = 6f;
    }

    public const float LODcooldownTime = 0.5f;

    // Impostors
    public const int numberOfAngles = 16;
    public const int numberOfFrames = 16;
    public const int numberOfColors = 15;

    public const int minImpostors = 1;
    public const int defImpostors = 500;
    public const int maxImpostors = 50000;
    public static int numberOfImpostors;

    // Camera
    public static Transform cameraTransform;

    // Screen
    public static float diagonalLength;

    static Settings()
    {
        if (Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }
        numberOfImpostors = defImpostors;
        int w = Screen.width;
        int h = Screen.height;
        diagonalLength = Mathf.Sqrt(w * w + h * h);
    }

}

