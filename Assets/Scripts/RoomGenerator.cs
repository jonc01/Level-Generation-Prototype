using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] int totalRooms;

    [Header("Direction Bias")] //Must be 100% total
    [SerializeField] float topChance = .25f;
    [SerializeField] float botChance = .25f;
    [SerializeField] float leftChance = .25f;
    [SerializeField] float rightChance = .25f;

    [Space(10)]

    [SerializeField] GameObject[] Walls;
    [SerializeField] GameObject[] PlatformGroups;
    [SerializeField] GameObject[] Shops;

    [SerializeField] bool shopAdded; //Shop Types: General, Attack Items, Defense Items, Healing Items
    

    private void Start()
    {
        shopAdded = false;

    }

    void Update()
    {
        
    }

    private void Move()
    {
        //
    }
}
