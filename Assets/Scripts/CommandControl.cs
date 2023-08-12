using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommandControl : MonoBehaviour
{
    //Variables
    private GameObject[] enemies;
    private bool selected;
    private EventSystem eventSystem;
    public GameObject system;

    //Runs on initiation
    //Set the event system to the game's event system
    //Populate the enemies GameObject with all GameObjects with the "Enemy" tag
    //Run Reset method
    void Start()
    {
        eventSystem = system.GetComponent<EventSystem>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Reset();
    }

    //Method used to determine which commands are available and only show valid commands
    //Populates enemies with all enemy GameObjects
    //Checks if there is an enemy in any of the spaces next to the target location
    //If true, set the "Attack" GameObject to true and break out of the loop
    //After the loop, set the "Item" and "Wait" GameObjects to true
    public void setUpCommand(Vector2 target, GameObject cursor)
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int bogus = 0; bogus < enemies.Length; bogus++)
        {
            if ((target.x + 1 == enemies[bogus].transform.position.x || target.x - 1 == enemies[bogus].transform.position.x) && (target.y == enemies[bogus].transform.position.y))
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                break;
            }
            else if ((target.y + 1 == enemies[bogus].transform.position.y || target.y - 1 == enemies[bogus].transform.position.y) && (target.x == enemies[bogus].transform.position.x))
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                break;
            }
        }

        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(true);
    }

    //Method used to reset the event system's selected GameObject and the command GameObjects
    //Sets the event system's selected GameObject to the "Wait" GameObject (this is so the player can use WASD to select commands instead of the mouse, which will be invisible during gameplay)
    //Sets all the command GameObjects to false
    public void Reset()
    {
        //setSelected(false);
        eventSystem.SetSelectedGameObject(transform.GetChild(3).gameObject);

        for (int bogus = 0; bogus < transform.childCount; bogus++)
        {
            transform.GetChild(bogus).gameObject.SetActive(false);
        }
    }

    //Getter for selected
    public bool getSelected()
    {
        return (selected);
    }

    //Setter for selected
    public void setSelected(bool selected)
    {
        this.selected = selected;
    }
}
