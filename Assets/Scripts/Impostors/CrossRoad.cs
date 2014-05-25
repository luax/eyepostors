using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossRoad : MonoBehaviour
{
    public float respawnWidth = 2.5f;
    public GameObject LeftToRightRoad;
    public GameObject TopToBottomRoad;

    private int numberOfImpostors;
    private List<GameObject> characters;
    private float characterY;
    private Dictionary<GameObject, Mission> missions;
    private Vector3 leftPosition;
    private Vector3 rightPosition;
    private Vector3 topPosition;
    private Vector3 bottomPosition;
    private GameObject character;

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
        private float speed;
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
            if ((time += Time.deltaTime) < travelingTime) {
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
            speed = Random.Range(1f, 2f);
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

    IEnumerator UpdateImpostors()
    {
        while (true) {
            if (numberOfImpostors != Settings.numberOfImpostors) {
                int num = (Settings.numberOfImpostors - numberOfImpostors);
                if (num < 0) {
                    RemoveImpostors(Mathf.Abs(num));
                } else {
                    AddImpostors(num);
                }
                numberOfImpostors = Settings.numberOfImpostors;
            }
            yield return new WaitForSeconds(1);
        }
    }


    public void Start()
    {
        character = (GameObject)Resources.Load("Character");
        characterY = character.transform.position.y;

        Initialize();

        AddImpostors(numberOfImpostors);

        StartCoroutine(UpdateImpostors());
    }

    private void Initialize()
    {
        numberOfImpostors = Settings.numberOfImpostors;
        leftPosition = LeftToRightRoad.transform.FindChild("StartPoint").transform.position;
        rightPosition = LeftToRightRoad.transform.FindChild("EndPoint").transform.position;
        topPosition = TopToBottomRoad.transform.transform.FindChild("StartPoint").transform.position;
        bottomPosition = TopToBottomRoad.transform.transform.FindChild("EndPoint").transform.position;
        missions = new Dictionary<GameObject, Mission>();
        characters = new List<GameObject>(numberOfImpostors);
    }

    private void RemoveImpostors(int diff)
    {
        // TODO: remove on each start position even
        for (int i = 0; diff > 0 && i < characters.Count; i++) {
            GameObject character = characters[i];
            missions.Remove(character);
            characters.RemoveAt(i);
            Destroy(character);
            diff--;
        }
    }

    private void AddImpostors(int diff)
    {
        for (int i = 0; i < diff; i++) {
            GameObject g = (GameObject)Instantiate(character);
            characters.Add(g);
            StartPosition s = GetStart(i, diff);
            g.transform.position = GetRandomPosition(g.transform.position, s);
            missions.Add(g, new Mission(g, this, s));
        }
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
                AddOffset(ref p, true); ;
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

    private StartPosition GetStart(int index, int condition)
    {
        if (index < condition / 4) {
            return StartPosition.Left;
        } else if (index < condition / 2) {
            return StartPosition.Top;
        } else if (index < (3 * condition) / 4) {
            return StartPosition.Right;
        } else {
            return StartPosition.Bottom;
        }
    }
}
