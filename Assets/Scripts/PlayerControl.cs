using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unit;

public class PlayerControl : MonoBehaviour
{
    //Variables
    private bool isActive = true;
    private int count;
    private bool enemyWin = false;

    //Run on initiation
    //Sets count equal to the number of children, which is the number of player units
    //Sets isActive to true
    void Start()
    {
        count = transform.childCount;
        setActive(true);
    }

    private void Update()
    {
        if (count < 1)
        {
            enemyWin = true;
        }
    }

    //Runs at the end of every frame
    //Sets the count equal to the number of children, just in case one of the players died during the frame's calculations
    //Runs the checkCount method
    private void LateUpdate()
    {
        count = transform.childCount;
        checkCount();
    }

    //Setter for isActive
    private void setActive(bool active)
    {
        isActive = active;
    }

    //Getter for isActive
    public bool getActive()
    {
        return (isActive);
    }

    //Method used to check if all the player units acted
    //Run in PlayerControl script LateUpdate
    //checks if the children of the player controller GameObject are inactive
    //Incriments a counter for each inactive child
    //If the counter is equal to the total number of children set isActive to false
    private void checkCount()
    {
        int tempCount = 0;
        for (int bogus = 0; bogus < count; bogus++)
        {
            if (!this.gameObject.transform.GetChild(bogus).gameObject.GetComponent<Unit>().getActive())
            {
                tempCount++;
            }
        }

        if (tempCount == count)
        {
            setActive(false);
        }
    }

    //Method used to reset all the children and the controller
    //Run in Controller script setStart method
    //Sets isActive to true
    //Sets each valid child as active
    public void Reset()
    {
        setActive(true);
        for (int bogus = 0; bogus < count; bogus++)
        {
            this.gameObject.transform.GetChild(bogus).gameObject.GetComponent<Unit>().setActive(true);
        }
    }

    public bool getWin()
    {
        return (enemyWin);
    }
}
