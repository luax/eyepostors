#define DEBUG
#undef DEBUG

using UnityEngine;
using System.Collections;

public class GazeDistance : Singleton<GazeDistance>
{
    private EyeXGazePoint gazePoint;
    private EyeXGazePointProvider gazePointProvider;

    private Vector3 cPos;
    private Vector3 gazePoint3D;
    private Vector3 gazePoint2D;

    private Rect rect;

    private GameObject latestObject;

    public void Awake()
    {
        gazePointProvider = EyeXGazePointProvider.GetInstance();
        rect = new Rect();
    }

    public void OnEnable()
    {
        gazePointProvider.StartStreaming(Settings.gazePointType);
    }

    public void OnDisable()
    {
        gazePointProvider.StopStreaming(Settings.gazePointType);
    }

    public void Update()
    {

        if (Settings.triggerOption == TriggerOption.Mouse) {
            gazePoint3D = Input.mousePosition;
            gazePoint2D = Input.mousePosition;
        } else {
            gazePoint = gazePointProvider.GetLastGazePoint(Settings.gazePointType);
            gazePoint3D = gazePoint.Screen;
            gazePoint2D = gazePoint.GUI;
        }

        cPos = Camera.main.transform.position;
    }

    public float CalculateDistance(GameObject gameObject)
    {
        if (!gameObject.renderer.isVisible) {
            return float.MaxValue;
        }

        if (Settings.triggerOption == TriggerOption.Gaze) {
            if (!gazePoint.IsValid || !gazePoint.IsWithinScreenBounds) {
                return float.MaxValue;
            }
        }

#if DEBUG
        latestObject = gameObject;
#endif

        Vector3 tPos = gameObject.transform.position;
        cPos.y = tPos.y = 0;

        if (MuchClose(ref tPos)) {
            return 0f;
        }

        if (MaxDistance(ref tPos)) {
            return float.MaxValue;
        }

        BoundsToScreenRect(gameObject.renderer.bounds);
        //Rect rect = ProjectedRect.GetProjectedRect(gameObject, Camera.main).rect;
        return DistanceToRectangle();
    }

    private bool MuchClose(ref Vector3 objectPosition)
    {
        return Vector3.Distance(cPos, objectPosition) < Settings.worldMinDistance;
    }

    private bool MaxDistance(ref Vector3 objectPosition)
    {
        return Vector3.Distance(cPos, objectPosition) > Settings.worldMaxDistance;
    }

    private float DistanceToRectangle()
    {
        float dx = Mathf.Max(rect.xMin - gazePoint2D.x, 0, gazePoint2D.x - rect.xMax);
        float dy = Mathf.Max(rect.yMin - gazePoint2D.y, 0, gazePoint2D.y - rect.yMax);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

#if DEBUG
    public void OnGUI()
    {
        if (latestObject.renderer != null && latestObject.renderer.isVisible)
        {
            //GUI.Box(ProjectedRect.GetProjectedRect(latestObject, Camera.main).rect, "Distance from trigger option " + CalculateDistance(latestObject));
            BoundsToScreenRect(latestObject.renderer.bounds);
            GUI.Box(rect, "Distance from trigger option " + CalculateDistance(latestObject));
        }
    }
#endif

    public void BoundsToScreenRect(Bounds b)
    {
        Vector3[] vertices = new Vector3[8];
        float h = Screen.height;
        Vector3 c = b.center;
        Vector3 e = b.extents;

        // Counter clockwise starting at top most left vertex
        vertices[0] = c + e;
        vertices[1] = new Vector3(c.x + e.x, c.y - e.y, c.z + e.z);
        vertices[2] = new Vector3(c.x - e.x, c.y - e.y, c.z + e.z);
        vertices[3] = new Vector3(c.x - e.x, c.y + e.y, c.z + e.z);
        vertices[4] = new Vector3(c.x + e.x, c.y + e.y, c.z - e.z);
        vertices[5] = new Vector3(c.x + e.x, c.y - e.y, c.z - e.z);
        vertices[6] = new Vector3(c.x - e.x, c.y - e.y, c.z - e.z);
        vertices[7] = new Vector3(c.x - e.x, c.y + e.y, c.z - e.z);

        Vector3 min = ConvertToScreenSpace(vertices[0]);
        Vector3 max = ConvertToScreenSpace(vertices[0]);
        for (int i = 1; i < vertices.Length; i++) {
            vertices[i] = ConvertToScreenSpace(vertices[i]);
            min = Vector3.Min(min, vertices[i]);
            max = Vector3.Max(max, vertices[i]);
        }

        rect.xMin = min.x;
        rect.yMin = min.y;
        rect.xMax = max.x;
        rect.yMax = max.y;

        //return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private Vector3 ConvertToScreenSpace(Vector3 p)
    {
        Vector3 r = Camera.main.WorldToScreenPoint(p);
        r.y = Screen.height - r.y;
        return r;
    }

    public Rect BoundsToScreenRect2(Bounds bounds)
    {
        Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
        Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));

        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }
}
