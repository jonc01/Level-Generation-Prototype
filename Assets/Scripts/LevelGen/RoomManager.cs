using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("SETUP")]
    public bool isStart;
    
    [Header("Walls")]
    public bool allWallsGenerated;
    [Space(10)]
    public bool topWall;
    public bool leftWall;
    public bool botWall;
    public bool rightWall;

    void Awake()
    {

    }

    void Update()
    {
        
    }
}
