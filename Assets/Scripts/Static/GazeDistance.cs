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

    // For debugging
    public const bool DEBUG = true;
    private Material red;
    private Material blue;
    private GameObject latestObject;

    public const float maxDistance = 10f;
    public const float maxGUIDistance = 1000f; // TODO

    private Vector3 cPos;
    private Vector3 gazePoint3D;
    private Vector3 gazePoint2D;

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

    public float CalculateDistance(ref GameObject gameObject)
    {
        if (DEBUG)
        {
            latestObject = gameObject;
        }

        if (triggerOption == TriggerOption.
        Gaze && (!gazePoint.IsValid || !gazePoint.IsWithinScreenBounds))
        {
            return float.MaxValue;
        }

        if (MaxDistance(ref gameObject) || MaxGUIDistance(ref gameObject))
        {
            return float.MaxValue;
        }

        //Vector2 gaze = new Vector2(gazePoint.Screen.x / Screen.width, gazePoint.Screen.y / Screen.height);
        //Vector2 pos = new Vector2(position.x, position.y / Screen.height);

        Rect rect = ProjectedRect.GetProjectedRect(gameObject, Camera.main).rect;

        return DistanceToRectangle(rect, gazePoint2D);
    }

    private bool MaxDistance(ref GameObject gameObject)
    {
        Vector3 tPos = gameObject.transform.position;
        cPos.y = tPos.y = 0;
        return Vector3.Distance(cPos, tPos) > maxDistance;
    }


    private bool MaxGUIDistance(ref GameObject gameObject)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        float test = Vector3.Distance(position, gazePoint2D); // GUI
        // Debug.Log("GUI dist: " + test);
        return test > maxGUIDistance;
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
            GUI.Box(ProjectedRect.GetProjectedRect(latestObject, Camera.main).rect, "Distance from trigger option " + CalculateDistance(ref latestObject));
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
