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
    [SerializeField] bool DEBUGGING = true;
    [SerializeField] WallGenerator WallGen;
    [SerializeField] RoomGenerator RoomGen;
    [SerializeField] public LayerMask buildLayer;
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
        RoomGen = GetComponent<RoomGenerator>();
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
        Invoke("StartLevelGen", .1f);
    }

    void StartLevelGen()
    {
        StartCoroutine(LevelGenCO());
    }

    IEnumerator LevelGenCO()
    {
        Time.timeScale = 0; //Pause all game function
        //*All coroutines related to level generation need to use WaitForSecondsRealtime if timeScale is 0

        //Generate Origins first
        yield return StartCoroutine(GenerateOriginsCO());

        //Start Wall generation
        WallGen.GenerateWallDoors();
        while (!WallGen.wallGenDone) yield return null;
        
        //Start Room generation
        RoomGen.GenerateRooms();
        while(!RoomGen.roomGenDone) yield return null;

        Time.timeScale = 1;
    }

    IEnumerator GenerateOriginsCO()
    {
        Debug.Log("Generating Origins...");
        builderRunning = true;
        for (int i = 0; i < totalRooms; i++)
        {
            if (i > 0)
            {
                Move();
                while (!openDirFound) yield return null; //Wait for space to be found in Move()
                
                yield return new WaitForSecondsRealtime(.01f); //.01
                GeneratedOrigins[i] = Instantiate(originObj, transform.position, Quaternion.identity, Level);
                currOrigin = 0;
                openDirFound = false;
            }
            else
            {
                GeneratedOrigins[i] = Instantiate(originObj, transform.position, Quaternion.identity, Level);
                startingRoom = transform.position;
                yield return new WaitForSecondsRealtime(.01f); //0.001 //.01
            }
        }
        endRoom = transform.position;

        builderRunning = false;

        yield return new WaitForSecondsRealtime(.1f);
        Debug.Log("Origins Generated");
    }

    private void Move()
    {
        StartCoroutine(MoveCO());
    }

    IEnumerator MoveCO()
    {
        int direction = ExistingOriginCheck();
        yield return new WaitForSecondsRealtime(.01f); //Delay needed for raycasts to update

        while (direction == -1)
        {
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