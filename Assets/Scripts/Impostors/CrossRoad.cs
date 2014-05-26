using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossRoad : MonoBehaviour
{
    public float respawnWidth = 2.5f;
    public GameObject positions;

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
        private CrossRoad cr;
        private Vector3 startPos;
        private Vector3 missionPos;
        private float travelingTime;
        private float time;
        private float speed;
        private StartPosition start;
        private Vector3 direction;
        private Transform transform;

        public Mission(GameObject g, CrossRoad r, StartPosition s)
        {
            cr = r;
            start = s;
            transform = g.transform;
            Respawn(true);
            Values();
        }

        public void Handle()
        {
            if ((time += Time.deltaTime) < travelingTime) {
                float fraction = time / travelingTime;
                transform.position = Vector3.Lerp(startPos, missionPos, fraction);
            } else {
                Respawn(false);
                Values();
            }
        }

        private void Values()
        {
            time = 0;
            speed = Random.Range(1f, 2f);
            startPos.y = cr.characterY;
            missionPos.y = cr.characterY;
            travelingTime = Vector3.Distance(startPos, missionPos) / speed;
            direction = missionPos - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Respawn(bool random)
        {
            // AddOffset true is for x-led
            switch (start) {
                case StartPosition.Left:
                    startPos = cr.leftPosition;
                    missionPos = cr.rightPosition;
                    if (random) {
                        startPos.x = Random.Range(cr.leftPosition.x, cr.rightPosition.x);
                    }

                    AddOffset(ref startPos, false);
                    AddOffset(ref missionPos, false);
                    break;
                case StartPosition.Top:
                    startPos = cr.topPosition;
                    missionPos = cr.bottomPosition;
                    if (random) {
                        startPos.z = Random.Range(cr.bottomPosition.z, cr.topPosition.z);
                    }
                    AddOffset(ref startPos, true);
                    AddOffset(ref missionPos, true);
                    break;
                case StartPosition.Right:
                    startPos = cr.rightPosition;
                    missionPos = cr.leftPosition;
                    if (random) {
                        startPos.x = Random.Range(cr.leftPosition.x, cr.rightPosition.x);
                    }
                    AddOffset(ref startPos, false);
                    AddOffset(ref missionPos, false);
                    break;
                case StartPosition.Bottom:
                    startPos = cr.bottomPosition;
                    missionPos = cr.topPosition;
                    if (random) {
                        startPos.z = Random.Range(cr.bottomPosition.z, cr.topPosition.z);
                    }
                    AddOffset(ref startPos, true);
                    AddOffset(ref missionPos, true);
                    break;
            }
            startPos.y = cr.characterY;
            missionPos.y = cr.characterY;
        }

        private void AddOffset(ref Vector3 point, bool x)
        {
            float value = Random.Range(-cr.respawnWidth, cr.respawnWidth);
            if (x) {
                point.x += value;
            } else {
                point.z += value;
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
        leftPosition = positions.transform.FindChild("LeftPosition").transform.position;
        rightPosition = positions.transform.FindChild("RightPosition").transform.position;
        topPosition = positions.transform.transform.FindChild("TopPosition").transform.position;
        bottomPosition = positions.transform.transform.FindChild("BottomPosition").transform.position;
        missions = new Dictionary<GameObject, Mission>();
        characters = new List<GameObject>(numberOfImpostors);
    }

    private void RemoveImpostors(int num)
    {
        // TODO: remove on each start position even
        for (int i = 0; num > 0 && i < characters.Count; i++) {
            GameObject character = characters[i];
            missions.Remove(character);
            characters.RemoveAt(i);
            Destroy(character);
            num--;
        }
    }

    private void AddImpostors(int num)
    {
        for (int i = 0; i < num; i++) {
            GameObject g = (GameObject)Instantiate(character);
            characters.Add(g);
            StartPosition s = GetStart(i, num);
            missions.Add(g, new Mission(g, this, s));
        }
    }

    public void Update()
    {
        foreach (GameObject g in characters) {
            missions[g].Handle();
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
