using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("SETUP")]
    public bool isStart;

    [Header("Walls")]
    public bool wallsGenerated;
    [Space(10)]
    public bool topWall; //These might not be needed, just use above
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
