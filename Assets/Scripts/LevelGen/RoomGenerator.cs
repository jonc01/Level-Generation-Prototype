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

    /*[Header("Direction Bias")] //Must be 100% total, can adjust to bias different map shapes
    [SerializeField] float botChance = .25f;
    [SerializeField] float topChance = .25f;
    [SerializeField] float leftChance = .25f;
    [SerializeField] float rightChance = .25f;*/

    [Space(10)]
    [Header("Generator Components")]
    [SerializeField] GameObject[] StartRooms;
    [SerializeField] GameObject[] Rooms; //Platforms and Room Collider
    [SerializeField] GameObject[] Shops;

    [Header("----------")]
    public GameObject[] GeneratedRooms; //Starting room at [0]

    [Header("Variables")]
    [SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items

    private bool roomGenRunning;
    //
    RaycastHit2D checkUp;
    RaycastHit2D checkLeft;
    RaycastHit2D checkDown;
    RaycastHit2D checkRight;
    //
    [SerializeField] private bool roomFoundUp;
    [SerializeField] private bool roomFoundLeft;
    [SerializeField] private bool roomFoundDown;
    [SerializeField] private bool roomFoundRight;

    private int currRoom;


    private void Awake()
    {
    }

    private void Start()
    {
        shopAdded = false; //Example
        GeneratedRooms = new GameObject[totalRooms];
        GenerateRooms();
    }

    void Update()
    {
        if (DEBUGGING) DebugRaycast();
        if (DEBUGGING)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                DeleteRooms();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                GetNewRandomRoom();
                //ExistingRoomCheck();
                /*int i = Random.Range(0, GeneratedRooms.Length);
                Debug.Log("Random Room: " + GeneratedRooms[i].name);
                Debug.Log("Room Transform: " + GeneratedRooms[i].transform.position);*/
                //float x = GeneratedRooms[i].gameObject.transform.position.x;
                //float y = GeneratedRooms[i].gameObject.transform.position.y;
            }
        }
        //if (!roomGenRunning) return;
        RoomConnectCheck();
    }

    private void DeleteRooms() //DEBUGGING
    {
        transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < GeneratedRooms.Length; i++)
        {
            Destroy(GeneratedRooms[i]);
        }
        Invoke("GenerateRooms", .5f);
    }

    void GenerateRooms()
    {
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
                yield return new WaitForSeconds(.5f);
                int randRoom = Random.Range(0, 4);
                GeneratedRooms[i] = Instantiate(Rooms[randRoom], transform.position, Quaternion.identity);
            }
            else
            {
                int startRoom = Random.Range(0, StartRooms.Length);
                GeneratedRooms[i] = Instantiate(StartRooms[startRoom], transform.position, Quaternion.identity);
                yield return new WaitForSeconds(.5f); //0.001
            }
        }
        roomGenRunning = false;
    }

    private void Move()
    {
        StartCoroutine(MoveCO());
    }

    private int TESTNUM() //TODO: TEMP
    {
        int i = 0;
        while (GeneratedRooms[i] == null)
            i++;

        return i;
    }

    IEnumerator MoveCO()
    {
        //int direction = Random.Range(0, 4); //0, 1, 2, 3
        int direction = ExistingRoomCheck();
        yield return new WaitForSeconds(.1f);

        //if(direction == -1 /*|| direction == 0*/) //-1 is being returned when no openings are found
        //{

        //currRoom = 0;
        while (direction == -1)// || direction == 0)
        {
            Debug.Log("1 - Picking different room ...");
            yield return new WaitForSeconds(.25f);

            int currRoom = TESTNUM(); //TODO: TEMP 

            Debug.Log("Random Room: " + GeneratedRooms[currRoom].name + " at " + GeneratedRooms[currRoom].transform.position);
            Vector3 newPos = GeneratedRooms[currRoom].transform.position;

            transform.position = newPos;
            Debug.Log("2 - Trying again...");

            yield return new WaitForSeconds(0.25f);
            direction = ExistingRoomCheck();

            yield return null;
        }


        //   0 +Y
        //1 -X  3 +X
        //   2 -Y
        float x = transform.position.x;
        float y = transform.position.y;

        switch (direction)
        {
            case 0: //Up
                Debug.Log("0: Up");
                transform.position = new Vector3(x, y += 3, 0);
                break;
            case 1: //Left
                Debug.Log("1: Left");
                transform.position = new Vector3(x -= 5, y, 0);
                break;
            case 2: //Down
                Debug.Log("2: Down");
                transform.position = new Vector3(x, y -= 3, 0);
                break;
            case 3: //Right
                Debug.Log("3: Right");
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

        /*for(int i=0; i<openDirections.Count; i++)
        {
            Debug.Log("Direction: " + openDirections[i]);
        }*/

        if (openDirections.Count > 0)
        {
            int j = Random.Range(0, openDirections.Count); //
            int dir = openDirections[j];
            Debug.Log("Selected Direction: " + dir);
            openDirections.Clear();
            return dir;
        }
        Debug.Log("No open directions. " + openDirections.Count);
        //GetNewRandomRoom();
        return -1;
    }

    private void GetNewRandomRoom()
    {
        currRoom++;
        StartCoroutine(MoveToRandomRoomCO());
    }

    IEnumerator MoveToRandomRoomCO()
    {
        //Pick new room, move to that position, call ExistingRoomcheck again
        int i = Random.Range(0, GeneratedRooms.Length);
        Debug.Log("Random Room: " + GeneratedRooms[i].name);
        Debug.Log("Room Transform: " + GeneratedRooms[i].transform.position);
        float x = GeneratedRooms[i].transform.position.x;
        float y = GeneratedRooms[i].transform.position.y;

        transform.position = new Vector3(x, y, 0);

        yield return new WaitForSeconds(0.5f);
        ExistingRoomCheck();
    }

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
}