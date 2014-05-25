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
    public const float gazeDistanceHigh = 50f;
    public const float gazeDistanceMedium = 180f;
    public const float cooldownTime = 0.5f;

    // Gaze distance
    public const float worldMaxDistance = 20f;
    public const float worldMinDistance = 0f;

    // Impostors
    public const int numberOfAngles = 16;
    public const int numberOfFrames = 16;
    public const int numberOfColors = 15;

    public static Transform cameraTransform;

    static Settings() {
        if (Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }
    }
}

