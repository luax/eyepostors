using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fun : MonoBehaviour
{
    private GameObject[] characters;
    private int[][] grid;
    private int grids = 10;
    private float characterY;
    private Mission mission;
    private System.Random random;
    private Dictionary<GameObject, Mission> missions;

    private class Mission
    {

        private GameObject character;
        private Fun fun;

        private Vector3 startPos;
        private Vector3 missionPos;

        private float travelingTime;
        private float time;

        private float speed = 3;


        public Mission(GameObject g, Fun r)
        {
            character = g;
            fun = r;
            Start();
        }

        public void Handle()
        {
            if (Vector3.Distance(character.transform.position, missionPos) > 2)
            {
                time += Time.deltaTime;
                float fraction = time / travelingTime;

                character.transform.position = Vector3.Lerp(startPos, missionPos, fraction);
                character.transform.rotation = Quaternion.LookRotation(missionPos - character.transform.position);

                //character.transform.position = Vector3.Lerp(character.transform.position, missionPos, Time.deltaTime*0.1f);
                //character.transform.rotation = Quaternion.LookRotation(missionPos - character.transform.position);
            }
            else
            {
                Start();
            }
        }

        private void Start()
        {
            time = 0;
            startPos = character.transform.position;
            missionPos = fun.GetRandomPosition();

            travelingTime = Vector3.Distance(startPos, missionPos) / speed;
        }

    }


    public void Start()
    {
        ////
        GameObject character = (GameObject)Resources.Load("Character");
        for (int i = 0; i < 1000; i++)
        {
            GameObject g = (GameObject)Instantiate(character);
        }
        Debug.Log("Number of impostors: " + 1000);
        ////

        random = new System.Random();
        missions = new Dictionary<GameObject, Mission>();

        characters = GameObject.FindGameObjectsWithTag("Character");

        characterY = characters[0].transform.position.y;
        grid = new int[grids][];

        foreach (GameObject g in characters)
        {
            g.transform.position = GetRandomPosition();
            missions.Add(g, new Mission(g, this));
        }
    }

    public void Update()
    {
        foreach (GameObject g in characters)
        {
            missions[g].Handle();
        }
    }

    private Vector3 GetRandomPosition()
    {
        return RandomPositionInGrid(random.Next(1, grids), random.Next(1, grids));
    }

    private Vector3 RandomPositionInGrid(int i, int j)
    {
        return new Vector3(i * random.Next(1, grids + 1), characterY, -j * random.Next(1, grids + 1));
    }

}
