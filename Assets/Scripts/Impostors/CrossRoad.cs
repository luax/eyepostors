using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossRoad : MonoBehaviour
{
    public int numberOfImpostors = 100;
    public float respawnWidth = 2.5f;
    public GameObject LeftToRightRoad;
    public GameObject TopToBottomRoad;

    private GameObject[] characters;
    private float characterY;
    private Dictionary<GameObject, Mission> missions;
    private Vector3 leftPosition;
    private Vector3 rightPosition;
    private Vector3 topPosition;
    private Vector3 bottomPosition;

    public enum StartPosition
    {
        Left,
        Top,
        Right,
        Bottom
    }

    private class Mission
    {

        private GameObject character;
        private CrossRoad cr;
        private Vector3 startPos;
        private Vector3 missionPos;
        private float travelingTime;
        private float time;
        private float speed = 3;
        private StartPosition start;

        public Mission(GameObject g, CrossRoad r, StartPosition s)
        {
            character = g;
            cr = r;
            start = s;

            startPos = g.transform.position;
            GetMission();
            Reset();
        }

        public void Handle()
        {
            if (Vector3.Distance(character.transform.position, missionPos) > 1) {
                time += Time.deltaTime;
                float fraction = time / travelingTime;
                character.transform.position = Vector3.Lerp(startPos, missionPos, fraction);
                character.transform.rotation = Quaternion.LookRotation(missionPos - character.transform.position);
            } else {
                Respawn();
                GetMission();
                Reset();
            }
        }

        private void Reset()
        {
            time = 0;
            startPos.y = cr.characterY;
            missionPos.y = cr.characterY;
            travelingTime = Vector3.Distance(startPos, missionPos) / speed;
        }

        private void Respawn()
        {
            SetPosition(true);
        }

        private void GetMission()
        {
            SetPosition(false);
        }

        private void SetPosition(bool s)
        {
            Vector3 p = Vector3.zero;
            switch (start) {
                case StartPosition.Left:
                    p = s ? cr.leftPosition : cr.rightPosition;
                    cr.AddOffset(ref p, true);
                    break;
                case StartPosition.Top:
                    p = s ? cr.topPosition : cr.bottomPosition;
                    cr.AddOffset(ref p, false);
                    break;
                case StartPosition.Right:
                    p = s ? cr.rightPosition : cr.leftPosition;
                    cr.AddOffset(ref p, true);
                    break;
                case StartPosition.Bottom:
                    p = s ? cr.bottomPosition : cr.topPosition;
                    cr.AddOffset(ref p, false);
                    break;
            }

            if (s) {
                startPos = p;
            } else {
                missionPos = p;
            }
        }
    }

    public void Start()
    {
        GameObject character = (GameObject)Resources.Load("Character");
        Initialize();

        for (int i = 0; i < numberOfImpostors; i++) {
            GameObject g = (GameObject)Instantiate(character);
            characters[i] = g;
            characterY = characters[0].transform.position.y; // TODO

            StartPosition s = GetStart(i);
            g.transform.position = GetRandomPosition(g.transform.position, s); // 
            missions.Add(g, new Mission(g, this, s));
        }
    }

    private void Initialize()
    {
        leftPosition = LeftToRightRoad.transform.FindChild("StartPoint").transform.position;
        rightPosition = LeftToRightRoad.transform.FindChild("EndPoint").transform.position;
        topPosition = TopToBottomRoad.transform.transform.FindChild("StartPoint").transform.position;
        bottomPosition = TopToBottomRoad.transform.transform.FindChild("EndPoint").transform.position;
        missions = new Dictionary<GameObject, Mission>();
        characters = new GameObject[numberOfImpostors];
    }

    public void Update()
    {
        foreach (GameObject g in characters) {
            missions[g].Handle();
        }
    }

    private Vector3 GetRandomPosition(Vector3 pos, StartPosition s)
    {
        Vector3 p = pos;
        switch (s) {
            case StartPosition.Left:
                p = leftPosition;
                p.z = Random.Range(rightPosition.z, leftPosition.z);
                AddOffset(ref p, true);
                break;
            case StartPosition.Top:
                p = topPosition;
                p.x = Random.Range(bottomPosition.x, topPosition.x);
                AddOffset(ref p, false);
                break;
            case StartPosition.Right:
                p = rightPosition;
                p.z = Random.Range(rightPosition.z, leftPosition.z);
                AddOffset(ref p, true);;
                break;
            case StartPosition.Bottom:
                p = bottomPosition;
                p.x = Random.Range(bottomPosition.x, topPosition.x);
                AddOffset(ref p, false);
                break;
        }
        p.y = characterY;
        return p;
    }

    private void AddOffset(ref Vector3 point, bool x)
    {
        float value = Random.Range(-respawnWidth, respawnWidth);
        if (x) {
            point.x += value;
        } else {
            point.z += value;
        }
    }

    private Vector3 GetOffset(ref Vector3 point, bool x)
    {
        float value = Random.Range(-respawnWidth, respawnWidth);
        if (x) {
            point.x += value;
        } else {
            point.z += value;
        }
        return point;
    }

    private StartPosition GetStart(int index)
    {
        if (index < numberOfImpostors / 4) {
            return StartPosition.Left;
        } else if (index < numberOfImpostors / 2) {
            return StartPosition.Top;
        } else if (index < (3 * numberOfImpostors) / 4) {
            return StartPosition.Right;
        } else {
            return StartPosition.Bottom;
        }
    }
}
