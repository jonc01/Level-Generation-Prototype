using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    // Level Generation v0.3
    //Step 1) LevelBuilder.cs
    //  a) This script instantiates and creates an array of Colliders
    //      b) Room origins (Colliders/Transform)

    //Step 2) WallGenerator.cs
    //  a) Loop through RoomOrigins[]
    //    a.1) For each room, Check each direction for an existing object 
    //    a.2) Generate a Door if there is a bordering room
    //    a.3) else generate a Wall 

    //Step 3) RoomGenerator.cs
    //  a) Loop through RoomOrigins[], place rooms at random
    //    a.1) Place Start at [0], and Boss at last [RoomOrigins.Length]
    //    a.2) Make sure certain rooms are spawned as required (Start, Boss, Shops, Trials, etc)

    [Header("Builder Setup")]
    [SerializeField] WallGenerator WallGen;
    [SerializeField] LayerMask buildLayer;
    [SerializeField] bool DEBUGGING = true;
    [SerializeField] private Transform Level; //Must be separate object

    [Space(10)]
    [Header("Generator Components")]
    public int totalRooms;
    [SerializeField] GameObject originObj;

    [Header("--- Populated at Generation ---")]
    public GameObject[] GeneratedOrigins; //Starting room at [0]
    [SerializeField] public Vector3 startingRoom; //First room transform added, Start room
    [SerializeField] public Vector3 endRoom; //Last room transform added, should be Boss room

    //
    [SerializeField] public bool originFoundUp;
    [SerializeField] public bool originFoundLeft;
    [SerializeField] public bool originFoundDown;
    [SerializeField] public bool originFoundRight;

    public bool builderRunning;
    private int currOrigin;
    private bool openDirFound;

    private void Start()
    {
        WallGen = GetComponent<WallGenerator>();
        GeneratedOrigins = new GameObject[totalRooms];
    }

    void Update()
    {
        if (DEBUGGING)
        {
            if (!builderRunning)
                if (Input.GetKeyDown(KeyCode.U)) DeleteOrigins();
        }
        if (WallGen.wallGenDone && !builderRunning) return; //Stop updating raycasts if not needed
        OriginConnectCheck(); //Raycasts
        DebugRaycast();
    }

    private void DeleteOrigins() //DEBUGGING
    {
        transform.position = new Vector3(0, 0, 0);

        for (int i = 0; i < GeneratedOrigins.Length; i++)
        {
            Destroy(GeneratedOrigins[i]);
        }
        Invoke("GenerateOrigins", .1f);
    }

    void GenerateOrigins()
    {
        Debug.Log("Generating Origins...");
        Time.timeScale = 0;
        StartCoroutine(GenerateOriginsCO());
    }

    IEnumerator GenerateOriginsCO()
    {
        builderRunning = true;
        for (int i = 0; i < totalRooms; i++)
        {
            if (i > 0)
            {
                Move();
                while (!openDirFound)
                {
                    //Wait for space to be found
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(.01f);
                GeneratedOrigins[i] = Instantiate(originObj, transform.position, Quaternion.identity, Level);
                currOrigin = 0;
                openDirFound = false;
            }
            else
            {
                GeneratedOrigins[i] = Instantiate(originObj, transform.position, Quaternion.identity, Level);
                startingRoom = transform.position;
                yield return new WaitForSecondsRealtime(.01f); //0.001
            }
        }
        endRoom = transform.position;
        builderRunning = false;

        ///////////////////////////////////TODO:
        //WallGen.GenerateWallDoors();
        //Wait for walls to be generated
        //while (!WallGen.wallGenDone) yield return null; //TODO: needs to allow starting

        Debug.Log("Origins Generated");
        Time.timeScale = 1;
    }

    private void Move()
    {
        StartCoroutine(MoveCO());
    }

    IEnumerator MoveCO()
    {
        //int direction = Random.Range(0, 4); //0, 1, 2, 3
        int direction = ExistingOriginCheck();
        yield return new WaitForSecondsRealtime(.01f); //Delay needed for raycasts to update

        while (direction == -1)
        {
            //yield return new WaitForSecondsRealtime(.01f);
            Vector3 newPos = GeneratedOrigins[currOrigin].transform.position;

            transform.position = newPos;

            yield return new WaitForSecondsRealtime(.01f); //Delay needed for raycasts to update
            direction = ExistingOriginCheck();

            if (direction != -1) break;
            currOrigin++;

            yield return null;
        }
        yield return new WaitForSecondsRealtime(.01f);
        openDirFound = true;

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
    }

    private int ExistingOriginCheck()
    {
        //Compile list of open directions
        List<int> openDirections = new List<int>();
        if (!originFoundUp) openDirections.Add(0);
        if (!originFoundLeft) openDirections.Add(1);
        if (!originFoundDown) openDirections.Add(2);
        if (!originFoundRight) openDirections.Add(3);

        //Debug.Log("Origins Found: " + originFoundUp + originFoundLeft + originFoundDown + originFoundRight);

        //Of all open directions, return random direction.
        if (openDirections.Count > 0)
        {
            int j = Random.Range(0, openDirections.Count);
            int dir = openDirections[j];
            openDirections.Clear();
            return dir;
        }
        //If a origin is surrounded, then move Builder to a previous origin transform, and check for opening.
        //Debug.Log("No open directions.");
        return -1;
    }

    #region Raycasts

    private void OriginConnectCheck()
    {
        //Bools to check if origins exist in each direction of current position
        originFoundUp = Physics2D.Raycast(transform.position, Vector3.up, 3f, buildLayer);
        originFoundLeft = Physics2D.Raycast(transform.position, Vector3.left, 5f, buildLayer);
        originFoundDown = Physics2D.Raycast(transform.position, Vector3.down, 3f, buildLayer);
        originFoundRight = Physics2D.Raycast(transform.position, Vector3.right, 5f, buildLayer);
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