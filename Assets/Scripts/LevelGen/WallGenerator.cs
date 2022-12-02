using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool DEBUGGING;
    [SerializeField] public bool wallGenDone;

    [Header("References")]
    [SerializeField] LayerMask wallLayer;
    [SerializeField] RoomGenerator RoomGen;

    [Header("Components")]
    [SerializeField] GameObject[] Walls; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] Doors; //0: Bot, 1: Top, 2: Left, 3: Right

    [Header("Raycasts")]
    RaycastHit2D roomUpHit; //rename roomUp
    RaycastHit2D roomLeftHit;
    RaycastHit2D roomDownHit;
    RaycastHit2D roomRightHit;

    bool buildingWallsDoors;

    private void Start()
    {
        RoomGen = GetComponent<RoomGenerator>();
        wallGenDone = false;
    }

    private void Update()
    {
        if (RoomGen.roomGenRunning) return;
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Generating Walls");
            GenerateWallDoors();
        }

        if(DEBUGGING) DebugRaycast();
        RoomConnectRaycastCheck();
        RaycastGetRoom();
    }

    public void GenerateWallDoors()
    {
        StartCoroutine(GenerateWallDoorsCO());
    }

    IEnumerator GenerateWallDoorsCO()
    {
        for (int i = 0; i < RoomGen.GeneratedRooms.Length; i++)
        {
            Debug.Log(" -------------- ");
            Debug.Log("1) Generating for Room: [" + i + "]");
            wallGenDone = false;
            buildingWallsDoors = true;
            Transform currPos = RoomGen.GeneratedRooms[i].transform;
            var currRoom = currPos.GetComponent<Room>(); //RoomGen.GeneratedRooms[i].GetComponent<Room>();
            Move(currPos); //Move to room position at index
            yield return new WaitForSecondsRealtime(.2f);
            WallDoorCheck(); //Check for rooms and generate walls or doors

            if (currRoom == null) wallGenDone = true;
            //if(currRoom != null) while (!currRoom.wallsGenerated) yield return null;

            while (buildingWallsDoors) yield return null;
            //while (!wallGenDone) yield return null;
            Debug.Log("3) buildingWallsDoors , True == " + buildingWallsDoors);
            Debug.Log("3) WallGenDone , True == " + wallGenDone);


            //while (!currRoom.wallsGenerated) yield return null;

        }
        Debug.Log("Walls done");
        yield return new WaitForSecondsRealtime(.1f);

        wallGenDone = true;
    }

    private void Move(Transform roomPos)
    {
        //Move Builder to position at GeneratedRooms index
        transform.position = roomPos.position;

        Debug.Log("Moving Builder to " + roomPos.position + " == " + transform.position);
    }

    void WallDoorCheck()
    {
        //StartCoroutine(WallDoorCheckCO());
        StartCoroutine(WallDoorCO());
    }

    IEnumerator WallDoorCheckCO()
    {
        buildingWallsDoors = true;
        //TODO: Tag check won't work,
        //

        //TODO: This should be run after all rooms/platforms are done being added first
        //TODO: Add all rooms into an array of transform positions, iterate through positions
        //      to run WallDoorCheck()
        //TODO: Add bool Raycast check then run this vv RaycastHit2D object

        Debug.Log("2) Checking for bordering rooms...");

        if (RoomGen.roomFoundUp) //Bool check
        {
            Debug.Log("2a) found room up");
            //Check for Room Component
            Room currRoom = roomUpHit.transform.GetComponent<Room>();
            if (currRoom != null) {
                Debug.Log("2b) Room script found");
                //Bordering room exists, if no walls are generated, add a Door
                /*if (!currRoom.wallsGenerated) GenerateDoor(0);
                Debug.Log("2c) wallsGenerated: " + currRoom.wallsGenerated);*/
            }
            GenerateWall(0);
        }
        else GenerateWall(0); //No room, generate a wall
        Debug.Log("3) No Room up, there should be a wall ...");

        yield return new WaitForSecondsRealtime(.5f);

        if (RoomGen.roomFoundLeft)
        {
            Room currRoom = roomLeftHit.transform.GetComponent<Room>();
            if (currRoom != null)
                //GenerateDoor(1);
            
            GenerateWall(1);
        }
        else GenerateWall(1);

        yield return new WaitForSecondsRealtime(.5f);

        if (RoomGen.roomFoundDown)
        {
            Room currRoom = roomDownHit.transform.GetComponent<Room>();
            /*if (currRoom != null)
                if (!currRoom.wallsGenerated) GenerateDoor(2);*/

            GenerateWall(2);
        }
        else GenerateWall(2);

        yield return new WaitForSecondsRealtime(.5f);

        if (RoomGen.roomFoundRight)
        {
            Room currRoom = roomRightHit.transform.GetComponent<Room>();
            /*if (currRoom != null)
                if (!currRoom.wallsGenerated) GenerateDoor(3);
*/
            GenerateWall(3);
        }
        else GenerateWall(3);

        yield return new WaitForSecondsRealtime(.5f);
        wallGenDone = true;
        buildingWallsDoors = false;
    }

    IEnumerator WallDoorCO()
    {
        Debug.Log("1) Checking...");
        if (RoomGen.roomFoundUp)
        {
            if (roomUpHit.collider != null)
            {
                if (roomUpHit.collider.CompareTag("WallDoor")) Debug.Log("WallDoor found UP"); //do nothing
                else
                {
                    //Room found, but no wall/door
                    Debug.Log("No WallDoor UP");
                    //Add Door, because room
                }
            }
            //roomRight.transform.GetComponent<Room>();
            //Debug.Log(currRoom + " currRoom");
        }
        else //No Room
        {
            //Build Wall
        }

        yield return new WaitForSecondsRealtime(.2f);

        if (RoomGen.roomFoundLeft) //LEFT
        {
            if (roomLeftHit.collider != null)
            {
                if (roomLeftHit.collider.CompareTag("WallDoor"))  Debug.Log("WallDoor found LEFT"); //do nothing
                else
                {
                    Debug.Log("No WallDoor LEFT");
                }
            }
        }
        else
        {
            //Build Wall
        }

        yield return new WaitForSecondsRealtime(.2f);

        if (RoomGen.roomFoundDown) //DOWN
        {
            if (roomDownHit.collider != null)
            {
                if (roomDownHit.collider.CompareTag("WallDoor")) Debug.Log("WallDoor found DOWN"); //do nothing
            }
            else Debug.Log("No WallDoor DOWN");
        }
        else
        {
            //Build Wall
        }

        yield return new WaitForSecondsRealtime(.2f);

        if (RoomGen.roomFoundRight) //RIGHT
        {
            if (roomRightHit.collider != null) 
            {
                if(roomRightHit.collider.CompareTag("WallDoor")) Debug.Log("WallDoor found RIGHT"); //do nothing
            }
            else Debug.Log("No WallDoor RIGHT"); // Room found, but no wall / door BUILD DOOR
        }
        else
        {
            //Build Wall
        }

    }

    void GenerateWall(int direction)
    {
        //Get current position, adjust offset and Instantiate Wall
        float x = transform.position.x;
        float y = transform.position.y;

        switch (direction) //TODO: Do this or setup walls to be offset, so instantiating at transform.position will be correct
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

    #region Raycasts
    private void RoomConnectRaycastCheck()
    {
        //bools
        roomUpHit = Physics2D.Raycast(transform.position, Vector3.up, 3f, wallLayer);
        roomLeftHit = Physics2D.Raycast(transform.position, Vector3.left, 5f, wallLayer);
        roomDownHit = Physics2D.Raycast(transform.position, Vector3.down, 3f, wallLayer);
        roomRightHit = Physics2D.Raycast(transform.position, Vector3.right, 5f, wallLayer);
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

    void RaycastGetRoom()
    {
        //roomUp
        /*roomLeft;
        roomDown;
        roomRight;*/
    }
    #endregion
}