using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Vector3 startingRoom;
    [SerializeField] int totalRooms;
    [SerializeField] Transform Builder;
    [SerializeField] LayerMask roomLayer;
    [SerializeField] bool DEBUGGING = true;

    [Header("Direction Bias")] //Must be 100% total, can adjust to bias different map shapes
    [SerializeField] float botChance = .25f;
    [SerializeField] float topChance = .25f;
    [SerializeField] float leftChance = .25f;
    [SerializeField] float rightChance = .25f;

    [Space(10)]
    [Header("Generator Components")]
    [SerializeField] GameObject[] Walls; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] Doors; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] PlatformGroups;
    [SerializeField] GameObject[] Shops;

    [Header("")]
    [SerializeField] GameObject[] GeneratedRooms;

    [Header("Variables")]
    [SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items

    private bool roomGenRunning;
    private bool wallGenRunning;
    //
    RaycastHit2D checkUp;
    RaycastHit2D checkLeft;
    RaycastHit2D checkDown;
    RaycastHit2D checkRight;
    //
    private bool roomFoundUp;
    private bool roomFoundLeft;
    private bool roomFoundDown;
    private bool roomFoundRight;


    private void Start()
    {
        shopAdded = false; //Example
        GenerateRooms();
    }

    void Update()
    {
        if (DEBUGGING) DebugRaycast();
        //if (!roomGenRunning) return;
        RoomConnectCheck();
    }

    void GenerateRooms()
    {
        roomGenRunning = true;
        for(int i = 0; i < totalRooms; i++)
        {

            int randPlatforms = Random.Range(0, 4);
            Instantiate(PlatformGroups[randPlatforms], Builder.position, Quaternion.identity);
            Move();
        }
        roomGenRunning = false;
    }

    private void Move()
    {
        int direction = Random.Range(0, 4); //excludes 4, only 0, 1, 2, 3
        //   0 +Y 
        //1 -X  3 +X
        //   2 -Y
        float x = Builder.position.x;
        float y = Builder.position.y;

        switch (direction)
        {
            case 0: //Up
                Builder.position = new Vector3(x, y += 3, 0);
                break;
            case 1: //Left
                Builder.position = new Vector3(x -= 5, y, 0);
                break;
            case 2: //Down
                Builder.position = new Vector3(x, y -= 3, 0);
                break;
            case 3: //Right
                Builder.position = new Vector3(x += 5, y, 0);
                break;
            default:
                Debug.Log("");
                break;
        }
        WallDoorCheck();
    }

    private int Rotate()
    {
        //     0 up
        //1 left + 3 right
        //    2 down

        /*int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0:
                return 0;
            case 1:
                return 1;
            default:
                break;
        }*/
        return 0;
    }
    
    void WallDoorCheck()
    {
        //TODO: This should be run after all rooms/platforms are done being added first
        //TODO: Add all rooms into an array of transform positions, iterate through positions
        //      to run WallDoorCheck()
        //TODO: Add bool Raycast check then run this vv RaycastHit2D object
        if (roomFoundUp)
        {
            checkUp = Physics2D.Raycast(Builder.position, Vector3.up, 3f, roomLayer);
            var RoomUp = checkUp.transform.gameObject.GetComponent<RoomManager>();
            if (RoomUp != null) GenerateDoor(0);
        }
        else GenerateWall(0);

        if (roomFoundLeft)
        {
            checkLeft = Physics2D.Raycast(Builder.position, Vector3.left, 5f, roomLayer);
            var RoomLeft = checkLeft.transform.gameObject.GetComponent<RoomManager>();
            if (RoomLeft != null) GenerateDoor(1);
        }
        else GenerateWall(1);

        if (roomFoundDown)
        {
            checkDown = Physics2D.Raycast(Builder.position, Vector3.down, 3f, roomLayer);
            var RoomDown = checkDown.transform.gameObject.GetComponent<RoomManager>();
            if (RoomDown != null) GenerateDoor(2);
        }
        else GenerateWall(2);

        if (roomFoundRight)
        {
            checkRight = Physics2D.Raycast(Builder.position, Vector3.right, 5f, roomLayer);
            var RoomRight = checkRight.transform.gameObject.GetComponent<RoomManager>();
            if (RoomRight != null) GenerateDoor(3);
        }
        else GenerateWall(3);
    }

    void GenerateWall(int direction) //TODO: move to new script? WallGenerator.cs
    {
        float x = Builder.position.x;
        float y = Builder.position.y;

        switch (direction)
        {
            case 0: //Up
                y += 1.5f;
                break;
            case 1: //Left
                x -= 2.5f;
                break;
            case 2: //Down
                y -= 1.5f;
                break;
            case 3: //Right
                x += 2.5f;
                break;
            default:
                break;
        }
        Vector3 newPos = new Vector3(x, y, 0);

        Instantiate(Walls[direction], newPos, Quaternion.identity);

        /*
        int randWall = Random.Range(0, 3); //If adding door location variants
        switch (direction)
        {
            case 0: //Up
                Instantiate(UpWalls[randWall], Builder.position, Quaternion.identity);
                break;
            case 1:
                Instantiate(LeftWalls[randWall], Builder.position, Quaternion.identity);
                break; ...
        */
    }

    void GenerateDoor(int direction) //TODO: move to new script?
    {
        float x = Builder.position.x;
        float y = Builder.position.y;

        switch (direction) {
            case 0: //Up
                y += 1.5f;
                break;
            case 1: //Left
                x -= 2.5f;
                break;
            case 2: //Down
                y -= 1.5f;
                break;
            case 3: //Right
                x += 2.5f;
                break;
            default:
                break;
        }
        Vector3 newPos = new Vector3(x, y, 0);

        Instantiate(Doors[direction], newPos, Quaternion.identity);
    }

    private void RoomConnectCheck()
    {
        //bools
        roomFoundUp =  Physics2D.Raycast(Builder.position, Vector3.up, 3f, roomLayer);
        roomFoundLeft = Physics2D.Raycast(Builder.position, Vector3.left, 5f, roomLayer);
        roomFoundDown = Physics2D.Raycast(Builder.position, Vector3.down, 3f, roomLayer);
        roomFoundRight = Physics2D.Raycast(Builder.position, Vector3.right, 5f, roomLayer);
    }

    private void DebugRaycast()
    {
        Vector3 up = transform.TransformDirection(Vector3.up) * 3f;
        Vector3 left = transform.TransformDirection(Vector3.left) * 5f;
        Vector3 down = transform.TransformDirection(Vector3.down) * 3f;
        Vector3 right = transform.TransformDirection(Vector3.right) * 5f;

        Debug.DrawRay(Builder.position, up, Color.green);
        Debug.DrawRay(Builder.position, left, Color.green);
        Debug.DrawRay(Builder.position, down, Color.green);
        Debug.DrawRay(Builder.position, right, Color.green);
    }
}
