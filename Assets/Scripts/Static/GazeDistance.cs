using UnityEngine;
using System.Collections;

public class GazeDistance : Singleton<GazeDistance>
{
    //public const EyeXGazePointType gazePointType = EyeXGazePointType.GazeLightlyFiltered;
    //public const TriggerOption triggerOption = TriggerOption.Mouse;

    private EyeXGazePointType gazePointType;
    private TriggerOption triggerOption;
    private EyeXGazePointProvider gazePointProvider;
    private EyeXGazePoint gazePoint;

    public const float maxDistance = 20f;
    public const float minDistance = 5f;

    private Vector3 cPos;
    private Vector3 gazePoint3D;
    private Vector3 gazePoint2D;

    public void Awake()
    {
        gazePointProvider = EyeXGazePointProvider.GetInstance();
    }

    public void Start()
    {
        CharacterGazeLOD gazeLOD = GameObject.FindObjectOfType<CharacterGazeLOD>();
        if (gazeLOD != null)
        {
            triggerOption = gazeLOD.option;
            gazePointType = gazeLOD.gazePointType;
        }
        else
        {
            triggerOption = TriggerOption.Mouse;
            gazePointType = EyeXGazePointType.GazeLightlyFiltered;
            Debug.LogError("No CharacterGazeLOD script was found and could not get any trigger or gaze point option");
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

        if (triggerOption == TriggerOption.Mouse)
        {
            gazePoint3D = Input.mousePosition;
            gazePoint2D = Input.mousePosition;
        }
        else
        {
            gazePoint = gazePointProvider.GetLastGazePoint(gazePointType);
            gazePoint3D = gazePoint.Screen;
            gazePoint2D = gazePoint.GUI;
        }

        cPos = Camera.main.transform.position;
    }

    public float CalculateDistance(GameObject gameObject)
    {

        if (triggerOption == TriggerOption.Gaze && (!gazePoint.IsValid || !gazePoint.IsWithinScreenBounds))
        {
            return float.MaxValue;
        }

        Vector3 tPos = gameObject.transform.position;
        cPos.y = tPos.y = 0;

        if (MuchClose(gameObject, ref tPos))
        {
            return 0f;
        }

        if (MaxDistance(gameObject, ref tPos))
        {
            return float.MaxValue;
        }

        //Rect rect = BoundsToScreenRect(gameObject.renderer.bounds);
        Rect rect = ProjectedRect.GetProjectedRect(gameObject, Camera.main).rect;
        return DistanceToRectangle(rect, gazePoint2D);
    }

    private bool MuchClose(GameObject gameObject, ref Vector3 objectPosition)
    {
        return Vector3.Distance(cPos, objectPosition) < minDistance;
    }

    private bool MaxDistance(GameObject gameObject, ref Vector3 objectPosition)
    {
        return Vector3.Distance(cPos, objectPosition) > maxDistance;
    }

    private float DistanceToRectangle(Rect rect, Vector2 gaze)
    {
        float dx = Mathf.Max(rect.xMin - gaze.x, 0, gaze.x - rect.xMax);
        float dy = Mathf.Max(rect.yMin - gaze.y, 0, gaze.y - rect.yMax);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    //public void OnGUI()
    //{
    //    if (latestObject.renderer != null)
    //    {
    //        //GUI.Box(ProjectedRect.GetProjectedRect(latestObject, Camera.main).rect, "Distance from trigger option " + CalculateDistance(latestObject));

    //        GUI.Box(BoundsToScreenRect(latestObject.renderer.bounds), "Distance from trigger option " + CalculateDistance(latestObject));
    //    }
    //}

    // Tobii version is probably better than this
    public Rect BoundsToScreenRect(Bounds bounds)
    {
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));

        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
}
