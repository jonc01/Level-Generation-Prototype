using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] bool DEBUGGING = true;
    public LevelBuilder Builder;

    [Space(10)]
    [Header("Generator Components")]
    [SerializeField] GameObject BossRoom;
    [Header("Generator Component Arrays")]
    [SerializeField] GameObject[] StartRooms;
    [SerializeField] GameObject[] Rooms; //Platforms and Room Collider
    [SerializeField] GameObject[] Shops;
    [SerializeField] GameObject[] Trials;

    [Header("--- Populated at Generation ---")]
    //TODO: remove

    [Header("Variables")]
    //[SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items
    public int numShops;

    public bool roomGenRunning;
    public bool roomGenDone;
    //
    private int currRoom;


    private void Start()
    {
        Builder = GetComponent<LevelBuilder>();
        roomGenDone = false;

        //shopAdded = false; //Example
    }

    void Update()
    {
        if (DEBUGGING) {
            if (Input.GetKeyDown(KeyCode.I)) GenerateRooms();
        }
    }

    public void GenerateRooms()
    {
        StartCoroutine(GenerateRoomsCO());
    }

    IEnumerator GenerateRoomsCO()
    {
        roomGenDone = false;
        roomGenRunning = true;
        for (int i = 0; i < Builder.totalRooms; i++)
        {
            Transform currOrigin = Builder.GeneratedOrigins[i].transform;
            if (i == 0) //Starting Room
            {
                int startRoom = Random.Range(0, StartRooms.Length);
                //Transform currOrigin = Builder.GeneratedOrigins[i].transform;
                Instantiate(StartRooms[startRoom], currOrigin.position, Quaternion.identity, currOrigin);
                yield return new WaitForSecondsRealtime(.01f); //0.001
            }
            else if (i == Builder.GeneratedOrigins.Length-1) //Ending/Boss Room
            {
                Instantiate(BossRoom, currOrigin.position, Quaternion.identity, currOrigin);
            }
            else
            {
                int rand = Random.Range(0, Rooms.Length);
                Instantiate(Rooms[rand], currOrigin.position, Quaternion.identity, currOrigin);
                //Debug.Log("Open space found!");
                /*yield return new WaitForSecondsRealtime(.01f);
                int randRoom = Random.Range(0, 4);
                Instantiate(Rooms[randRoom], transform.position, Quaternion.identity);
                currRoom = 0;*/
            }
        }
        roomGenRunning = false;
        roomGenDone = true;
    }
}