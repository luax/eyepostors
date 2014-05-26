using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{

    public GameObject camera;
    public Transform lightTarget;
    public Transform cityTarget;
    public float mySpeed;
    public Hashtable myTween;

    private float pathPosition;
    void Start()
    {
        lightTarget = GameObject.Find("_Directional light").transform;
        cityTarget = GameObject.Find("CityBlock").transform;
        Path1();

    }
    private void Path1()
    {
        myTween = new Hashtable();
        mySpeed = 10f;
        myTween.Add("looktarget", lightTarget);
        myTween.Add("time", mySpeed);
        myTween.Add("looktime", 1f);
        myTween.Add("path", iTweenPath.GetPath("Path1"));
        myTween.Add("easetype", iTween.EaseType.linear);
        myTween.Add("looptype", iTween.LoopType.none);
        myTween.Add("oncomplete", "Path2");
        myTween.Add("oncompletetarget", gameObject);
        iTween.MoveTo(camera, myTween);
    }

    private void Path2()
    {
        myTween = new Hashtable();
        mySpeed = 20f;
        myTween.Add("looktarget", lightTarget);
        myTween.Add("time", mySpeed);
        myTween.Add("path", iTweenPath.GetPath("Path2"));
        myTween.Add("easetype", iTween.EaseType.linear);
        myTween.Add("looptype", iTween.LoopType.none);
        myTween.Add("oncomplete", "Path3");
        myTween.Add("oncompletetarget", gameObject);
        iTween.MoveTo(camera, myTween);
    }

    private void Path3()
    {
        myTween = new Hashtable();
        mySpeed = 20f;
        myTween.Add("looktarget", cityTarget);
        myTween.Add("time", mySpeed);
        myTween.Add("path", iTweenPath.GetPath("Path3"));
        myTween.Add("easetype", iTween.EaseType.linear);
        myTween.Add("looptype", iTween.LoopType.none);
        myTween.Add("oncomplete", "Path1");
        myTween.Add("oncompletetarget", gameObject);
        iTween.MoveTo(camera, myTween);
    }
}