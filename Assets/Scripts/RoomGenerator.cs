using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Vector3 startingRoom;
    [SerializeField] int totalRooms;
    [SerializeField] Transform Builder;

    [Header("Direction Bias")] //Must be 100% total, can adjust to bias different map shapes
    [SerializeField] float botChance = .25f;
    [SerializeField] float topChance = .25f;
    [SerializeField] float leftChance = .25f;
    [SerializeField] float rightChance = .25f;

    [Space(10)]

    [SerializeField] GameObject[] Walls; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] Doors; //0: Bot, 1: Top, 2: Left, 3: Right
    [SerializeField] GameObject[] PlatformGroups;
    [SerializeField] GameObject[] Shops;

    [SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items
    

    private void Start()
    {
        shopAdded = false; //Example
        GenerateRooms();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            int i = Random.Range(0, 4);
            Debug.Log(i);
        }
    }

    void GenerateRooms()
    {
        for(int i = 0; i < totalRooms; i++)
        {
            int randPlatforms = Random.Range(0, 4);
            Instantiate(PlatformGroups[randPlatforms], Builder.position, Quaternion.identity);
            Move();
        }
    }

    private void Move()
    {
        int j = Random.Range(0, 4); //excludes 4, only 0, 1, 2, 3
        //0: -X, 1: +X
        //2: -Y, 3: +Y
        //X += 6, Y += 4
        float x = Builder.position.x;
        float y = Builder.position.y;

        switch (j)
        {
            case 0:
                Builder.position = new Vector3(x -= 6, y, 0);
                break;
            case 1:
                Builder.position = new Vector3(x += 6, y, 0);
                break;
            case 2:
                Builder.position = new Vector3(x, y -= 4, 0);
                break;
            case 3:
                Builder.position = new Vector3(x, y += 4, 0);
                break;
            default:
                break;
        }
    }
}
