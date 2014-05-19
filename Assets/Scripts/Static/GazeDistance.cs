using UnityEngine;
using System.Collections;

public class GazeDistance : Singleton<GazeDistance>
{
    public EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;

    private EyeXGazePointProvider gazePointProvider;
    private EyeXGazePoint gazePoint;
    
    public void Awake()
    {
        gazePointProvider = EyeXGazePointProvider.GetInstance();
    }

    public void OnEnable()
    {
        gazePointProvider.StartStreaming(gazePointType);
    }

    public void OnDisable()
    {
        gazePointProvider.StopStreaming(gazePointType);
    }

    public void Update()
    {
        gazePoint = gazePointProvider.GetLastGazePoint(gazePointType);
    }

    public float CalculateDistance(Vector3 position)
    {
        if (!gazePoint.IsValid || !gazePoint.IsWithinScreenBounds)
        {
            return float.MaxValue;
        }
        position = Camera.main.WorldToScreenPoint(position);
        Vector2 gaze = new Vector2(gazePoint.Screen.x / Screen.width, gazePoint.Screen.y / Screen.height);
        Vector2 pos = new Vector2(position.x / Screen.width, position.y / Screen.height);
        return Vector2.Distance(gaze, pos);
    }
}
