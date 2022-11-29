using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool DEBUGGING;
    [SerializeField] public bool wallsGenerated;

    [Header("References")]
    [SerializeField] LayerMask roomLayer;
    [SerializeField] Transform[] GeneratedRooms;
    [SerializeField] RoomGenerator RoomGen;

    [Header("Components")]
    [SerializeField] GameObject[] Walls; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] Doors; //0: Bot, 1: Top, 2: Left, 3: Right

    [Header("Raycasts")]
    RaycastHit2D checkUp; //rename roomUp
    RaycastHit2D checkLeft;
    RaycastHit2D checkDown;
    RaycastHit2D checkRight;

    bool roomFoundUp;
    bool roomFoundLeft;
    bool roomFoundDown;
    bool roomFoundRight;

    private void Start()
    {
        RoomGen = GetComponent<RoomGenerator>();
        wallsGenerated = false;
    }

    private void Update()
    {
        if (RoomGen.roomGenRunning) return;
        if (Input.GetKeyDown(KeyCode.I))
        {
            GenerateWallDoors();
        }

        if(DEBUGGING) DebugRaycast();
        RoomConnectCheck();
    }

    public void GenerateWallDoors()
    {
        StartCoroutine(GenerateWallDoorsCO());
    }

    IEnumerator GenerateWallDoorsCO()
    {
        for (int i = 0; i < GeneratedRooms.Length; i++)
        {
            WallDoorCheck(RoomGen.GeneratedRooms[i].transform.position);
        }
        Debug.Log("Walls done");
        yield return new WaitForSecondsRealtime(.1f);

        wallsGenerated = true;
    }

    private void Move(Vector3 roomPos)
    {
        //Move Builder to position at GeneratedRooms index
        transform.position = roomPos;
    }

    void WallDoorCheck(Vector3 roomPos)
    {
        //TODO: Tag check won't work, 
        //



        //TODO: This should be run after all rooms/platforms are done being added first
        //TODO: Add all rooms into an array of transform positions, iterate through positions
        //      to run WallDoorCheck()
        //TODO: Add bool Raycast check then run this vv RaycastHit2D object
        if (roomFoundUp)
        {
            checkUp = Physics2D.Raycast(transform.position, Vector3.up, 3f, roomLayer);
            var RoomUp = checkUp.transform.gameObject.GetComponent<Room>();
            if (RoomUp != null) GenerateWall(0, false);
        }
        else GenerateWall(0);

        if (roomFoundLeft)
        {
            checkLeft = Physics2D.Raycast(transform.position, Vector3.left, 5f, roomLayer);
            var RoomLeft = checkLeft.transform.gameObject.GetComponent<Room>();
            if (RoomLeft != null) GenerateWall(1, false);
        }
        else GenerateWall(1);

        if (roomFoundDown)
        {
            checkDown = Physics2D.Raycast(transform.position, Vector3.down, 3f, roomLayer);
            var RoomDown = checkDown.transform.gameObject.GetComponent<Room>();
            if (RoomDown != null) GenerateWall(2, false);
        }
        else GenerateWall(2);

        if (roomFoundRight)
        {
            checkRight = Physics2D.Raycast(transform.position, Vector3.right, 5f, roomLayer);
            var RoomRight = checkRight.transform.gameObject.GetComponent<Room>();
            if (RoomRight != null) GenerateWall(3, false);
        }
        else GenerateWall(3);
    }

    void GenerateWall(int direction, bool isWall = true)
    {
        //Get current position, adjust offset and Instantiate Wall
        float x = transform.position.x;
        float y = transform.position.y;

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

        if(isWall) Instantiate(Walls[direction], newPos, Quaternion.identity);
        else Instantiate(Doors[direction], newPos, Quaternion.identity);

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

    void GenerateDoor(int direction)
    {
        float x = transform.position.x;
        float y = transform.position.y;

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

        Instantiate(Doors[direction], newPos, Quaternion.identity);
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