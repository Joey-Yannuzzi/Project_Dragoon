using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unit;

public class Wait : MonoBehaviour
{
    //Variables
    private GameObject player;
    [SerializeField] private GameObject cursor;

    //Runs every frame
    //Checks if the cursor has a valid player selected
    //If true set the player equal to the cursor's player
    void Update()
    {
        if (cursor.GetComponent<CursorSet>().getTempcharacter())
        {
            player = cursor.GetComponent<CursorSet>().getTempcharacter();
        }
    }

    //Method for initiating movement that ends in the unit waiting in the selected spot
    //Run when the "Wait" GameObject is clicked
    //Sets the CommandControl script's selected bool to true
    //Runs CommandControl script's Reset method
    //Run's the player's Unit script's move method and adds the false parameter to indicate not to attack
    public void onClick()
    {
        transform.GetComponentInParent<CommandControl>().setSelected(true);
        transform.GetComponentInParent<CommandControl>().Reset();
        player.GetComponent<Unit>().move(cursor.transform.position - cursor.GetComponent<CursorSet>().offset, cursor, false);
    }
}
