using UnityEngine;
using System.Collections;

public class GazeDistance : Singleton<GazeDistance>
{
    public EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;
    public TriggerOption triggerOption = TriggerOption.Mouse;

    private EyeXGazePointProvider gazePointProvider;
    private EyeXGazePoint gazePoint;

    // For debugging
    public const bool DEBUG = false;
    private Material red;
    private Material blue;
    private GameObject latestObject;


    public void Awake()
    {
        gazePointProvider = EyeXGazePointProvider.GetInstance();
    }

    public void Start()
    {
        if (DEBUG)
        {
            latestObject = gameObject;
        }
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

    public float CalculateDistance(GameObject gameObject, TriggerOption option)
    {
        if (!gazePoint.IsValid || !gazePoint.IsWithinScreenBounds)
        {
            return float.MaxValue;
        }

        if (DEBUG)
        {
            latestObject = gameObject;
        }

        Rect rect = ProjectedRect.GetProjectedRect(gameObject, Camera.main).rect;

        if (option == TriggerOption.Mouse)
        {
            return DistanceToRectangle(rect, Input.mousePosition);
        }
        else
        {
            return DistanceToRectangle(rect, gazePoint.GUI);
        }
    }


    private float DistanceToRectangle(Rect rect, Vector2 gaze)
    {
        float dx = Mathf.Max(rect.xMin - gaze.x, 0, gaze.x - rect.xMax);
        float dy = Mathf.Max(rect.yMin - gaze.y, 0, gaze.y - rect.yMax);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public void OnGUI()
    {
        if (DEBUG && latestObject.renderer != null)
        {
            GUI.Box(ProjectedRect.GetProjectedRect(latestObject, Camera.main).rect, "Distance from trigger option " + CalculateDistance(latestObject, triggerOption));
        }
    }

    // Tobii version is probably better than this
    public Rect BoundsToScreenRect(Bounds bounds)
    {
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
}
