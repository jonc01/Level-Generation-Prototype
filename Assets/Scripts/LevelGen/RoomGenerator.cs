using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Vector3 startingRoom;
    [SerializeField] int totalRooms;
    [SerializeField] LayerMask roomLayer;
    [SerializeField] bool DEBUGGING = true;
    [SerializeField] WallGenerator WallGen;

    [Space(10)]
    [Header("Generator Components")]
    [SerializeField] GameObject[] StartRooms;
    [SerializeField] GameObject[] Rooms; //Platforms and Room Collider
    [SerializeField] GameObject[] Shops;

    [Header("--- Populated at Generation ---")]
    public GameObject[] GeneratedRooms; //Starting room at [0]

    [Header("Variables")]
    [SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items

    public bool roomGenRunning;
    //
    [SerializeField] public bool roomFoundUp;
    [SerializeField] public bool roomFoundLeft;
    [SerializeField] public bool roomFoundDown;
    [SerializeField] public bool roomFoundRight;

    private int currRoom;
    private bool openSpaceFound;

    private void Awake()
    {

    }

    private void Start()
    {
        WallGen = GetComponent<WallGenerator>();
        shopAdded = false; //Example
        GeneratedRooms = new GameObject[totalRooms];
        GenerateRooms();
    }

    void Update()
    {
        if (DEBUGGING) {
            DebugRaycast();
            if (Input.GetKeyDown(KeyCode.J)) DeleteRooms();
        }
        //if (!roomGenRunning) return;
        RoomConnectCheck(); //Raycasts
    }

    private void DeleteRooms() //DEBUGGING
    {
        transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < GeneratedRooms.Length; i++)
        {
            Destroy(GeneratedRooms[i]);
        }
        Invoke("GenerateRooms", .1f);
    }

    void GenerateRooms()
    {
        Time.timeScale = 0;
        StartCoroutine(GenerateRoomsCO());
    }

    IEnumerator GenerateRoomsCO()
    {
        roomGenRunning = true;
        for (int i = 0; i < totalRooms; i++)
        {
            if (i > 0)
            {
                Move();
                while (!openSpaceFound)
                {
                    //Wait for space to be found
                    yield return null;
                }
                //Debug.Log("Open space found!");
                yield return new WaitForSecondsRealtime(.01f);
                int randRoom = Random.Range(0, 4);
                GeneratedRooms[i] = Instantiate(Rooms[randRoom], transform.position, Quaternion.identity);
                currRoom = 0;
                openSpaceFound = false;
            }
            else
            {
                int startRoom = Random.Range(0, StartRooms.Length);
                GeneratedRooms[i] = Instantiate(StartRooms[startRoom], transform.position, Quaternion.identity);
                yield return new WaitForSecondsRealtime(.01f); //0.001
            }
        }
        roomGenRunning = false;

        ///////////////////////////////////TODO:
        //WallGen.GenerateWallDoors();
        //Wait for walls to be generated
        //while (!WallGen.wallGenDone) yield return null; //TODO: needs to allow starting

        Time.timeScale = 1;
    }

    private void Move()
    {
        StartCoroutine(MoveCO());
    }

    IEnumerator MoveCO()
    {
        //int direction = Random.Range(0, 4); //0, 1, 2, 3
        int direction = ExistingRoomCheck();
        yield return new WaitForSecondsRealtime(.01f); //Delay needed for raycasts to update

        while (direction == -1)
        {
            Debug.Log("1 - Picking different room at: [" + currRoom + "]");
            //yield return new WaitForSecondsRealtime(.01f);

            Debug.Log("Random Room: " + GeneratedRooms[currRoom].name + " at " + GeneratedRooms[currRoom].transform.position);
            Vector3 newPos = GeneratedRooms[currRoom].transform.position;

            transform.position = newPos;
            
            yield return new WaitForSecondsRealtime(.01f); //Delay needed for raycasts to update
            direction = ExistingRoomCheck();

            if (direction != -1) break;
            currRoom++;

            yield return null;
        }
        yield return new WaitForSecondsRealtime(.01f);
        openSpaceFound = true;

        //   0 +Y
        //1 -X  3 +X
        //   2 -Y
        float x = transform.position.x;
        float y = transform.position.y;
        //Update Builder transform
        switch (direction)
        {
            case 0: //Up
                transform.position = new Vector3(x, y += 3, 0);
                break;
            case 1: //Left
                transform.position = new Vector3(x -= 5, y, 0);
                break;
            case 2: //Down
                transform.position = new Vector3(x, y -= 3, 0);
                break;
            case 3: //Right
                transform.position = new Vector3(x += 5, y, 0);
                break;
            default:
                Debug.Log("Error in Move() Random.Range");
                break;
        }
        //WallDoorCheck();
    }

    private int ExistingRoomCheck()
    {
        //Builder picks a random direction, if room exists, pick another direction.
        //Return all open directions, random.range from remaining directions.
        //If a room is surrounded, then move Builder to a previous room, and check for opening.

        List<int> openDirections = new List<int>();
        if (!roomFoundUp) openDirections.Add(0);
        if (!roomFoundLeft) openDirections.Add(1);
        if (!roomFoundDown) openDirections.Add(2);
        if (!roomFoundRight) openDirections.Add(3);

        Debug.Log("Rooms Found: " + roomFoundUp + roomFoundLeft + roomFoundDown + roomFoundRight);

        if (openDirections.Count > 0)
        {
            int j = Random.Range(0, openDirections.Count);
            int dir = openDirections[j];
            Debug.Log("Selected Direction: " + dir);
            openDirections.Clear();
            return dir;
        }
        Debug.Log("No open directions.");
        return -1;
    }

    #region Raycasts

    private void RoomConnectCheck()
    {
        //bools
        roomFoundUp = Physics2D.Raycast(transform.position, Vector3.up, 3f, roomLayer);
        roomFoundLeft = Physics2D.Raycast(transform.position, Vector3.left, 5f, roomLayer);
        roomFoundDown = Physics2D.Raycast(transform.position, Vector3.down, 3f, roomLayer);
        roomFoundRight = Physics2D.Raycast(transform.position, Vector3.right, 5f, roomLayer);
    }

    private void DebugRaycast()
    {
        Vector3 up = transform.TransformDirection(Vector3.up) * 3f;
        Vector3 left = transform.TransformDirection(Vector3.left) * 5f;
        Vector3 down = transform.TransformDirection(Vector3.down) * 3f;
        Vector3 right = transform.TransformDirection(Vector3.right) * 5f;

        Debug.DrawRay(transform.position, up, Color.green);
        Debug.DrawRay(transform.position, left, Color.green);
        Debug.DrawRay(transform.position, down, Color.green);
        Debug.DrawRay(transform.position, right, Color.green);
    }
    #endregion
}