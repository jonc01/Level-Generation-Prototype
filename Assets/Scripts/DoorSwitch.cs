using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField] BoxCollider2D collider;
    [SerializeField] GameObject textPrompt;
    public bool isActive;
    bool playerInRange;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();

        if (textPrompt != null)
            textPrompt.SetActive(false);

        playerInRange = false;
    }

    void Update()
    {
        if (collider == null) return;

        //If Switch is active, enable trigger collider to allow interaction
        if (!isActive)
        {
            collider.enabled = false;
            return;
        }
        else collider.enabled = true;

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Open Doors
            Debug.Log("Open Doors");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Display Prompt
        if (textPrompt == null) return;

        if (isActive)
        {
            textPrompt.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Hide prompt
        if(textPrompt != null) textPrompt.SetActive(false);
        playerInRange = false;
    }
}
