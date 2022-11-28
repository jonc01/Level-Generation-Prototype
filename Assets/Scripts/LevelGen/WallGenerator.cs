using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask roomLayer;
    [SerializeField] Transform[] GeneratedRooms;

    [Header("Components")]
    [SerializeField] GameObject[] Walls; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] Doors; //0: Bot, 1: Top, 2: Left, 3: Right

    [Header("Raycasts")]
    RaycastHit2D checkUp;
    RaycastHit2D checkLeft;
    RaycastHit2D checkDown;
    RaycastHit2D checkRight;
    
    bool roomFoundUp;
    bool roomFoundLeft;
    bool roomFoundDown;
    bool roomFoundRight;

    private void Start()
    {
        //GeneratedRooms = GameObject.FindGameObjectWithTag("GeneratedRooms").transform;
    }

    public void GenerateWallDoors()
    {
        for(int i = 0; i < GeneratedRooms.Length; i++)
        {
            WallDoorCheck(GeneratedRooms[i].position);
        }
        Debug.Log("Walls done");
    }

    private void Move(Vector3 roomPos)
    {
        //Move Builder to position at GeneratedRooms index
        transform.position = roomPos;
    }

    void WallDoorCheck(Vector3 roomPos)
    {
        //TODO: This should be run after all rooms/platforms are done being added first
        //TODO: Add all rooms into an array of transform positions, iterate through positions
        //      to run WallDoorCheck()
        //TODO: Add bool Raycast check then run this vv RaycastHit2D object
        if (roomFoundUp)
        {
            checkUp = Physics2D.Raycast(transform.position, Vector3.up, 3f, roomLayer);
            var RoomUp = checkUp.transform.gameObject.GetComponent<RoomManager>();
            if (RoomUp != null) GenerateDoor(0);
        }
        else GenerateWall(0);

        if (roomFoundLeft)
        {
            checkLeft = Physics2D.Raycast(transform.position, Vector3.left, 5f, roomLayer);
            var RoomLeft = checkLeft.transform.gameObject.GetComponent<RoomManager>();
            if (RoomLeft != null) GenerateDoor(1);
        }
        else GenerateWall(1);

        if (roomFoundDown)
        {
            checkDown = Physics2D.Raycast(transform.position, Vector3.down, 3f, roomLayer);
            var RoomDown = checkDown.transform.gameObject.GetComponent<RoomManager>();
            if (RoomDown != null) GenerateDoor(2);
        }
        else GenerateWall(2);

        if (roomFoundRight)
        {
            checkRight = Physics2D.Raycast(transform.position, Vector3.right, 5f, roomLayer);
            var RoomRight = checkRight.transform.gameObject.GetComponent<RoomManager>();
            if (RoomRight != null) GenerateDoor(3);
        }
        else GenerateWall(3);
    }

    void GenerateWall(int direction)
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
}
