using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement movement;
    public PlayerCombat combat;

    void Update()
    {
        //MOVEMENT
        if (movement != null && movement.allowInput)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                movement.Move(false);
            }
            if (Input.GetButtonDown("Horizontal"))
            {
                movement.Move(true);
            }

            /*if (Input.GetButtonDown("Dodge"))
            {
                if (movement.canDash) movement.StartDash();
            }*/
        }

        //COMBAT
        if (combat != null && combat.allowInput)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (movement.IsGrounded()) combat.Attack1(); //isGrounded
                //else combat.Attack2();
            }
        }
    }

}
