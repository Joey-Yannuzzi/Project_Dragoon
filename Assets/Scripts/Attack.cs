using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unit;

public class Attack : MonoBehaviour
{
    //Variables
    private GameObject player;
    public GameObject cursor;
    public Vector3 offset;

    //Runs every frame
    //Sets player equal to the currently selected character if not null
    void Update()
    {
        if (cursor.GetComponent<CursorSet>().getTempcharacter())
        {
            player = cursor.GetComponent<CursorSet>().getTempcharacter();
        }
    }

    //Runs the attack() method in the unit class for the specified unit
    //Activates when 'Attack' button selected in command prompt
    public void onClick()
    {
        //cursor.GetComponent<CursorSet>().setAttacking(true);
        transform.GetComponentInParent<CommandControl>().setSelected(true);
        transform.GetComponentInParent<CommandControl>().Reset();
        player.GetComponent<Unit>().attackInit("Enemy", cursor.transform.position - offset, cursor);
    }
}
