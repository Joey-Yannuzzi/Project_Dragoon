using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextControl : MonoBehaviour
{
    //Variables
    public TextMeshProUGUI phaseText;
    public GameObject timer;
    private bool isTimed;
    private GameObject tempTimer;

    //Called on initiation
    //Sets the phase text blank
    //Sets the temporary timer to null
    void Start()
    {
        phaseText.text = "";
        tempTimer = null;
    }

    //Runs every frame
    //Will catch an exception every frame that tempTimer is null
    //Will successfully run the try statement if tempTimer is valid
    //If the statement if true wipe the phase UI elements in order to resume play
    void Update()
    {
        try
        {
            if (tempTimer.GetComponent<Timer>().getTime() < 0)
            {
                isTimed = false;
                phaseText.text = "";
                phaseText.color = Color.white;
                tempTimer = null;
            }
        }
        catch
        {
            //Debug.Log("tempTimer is null");
        }
    }

    //Displays Player Phase UI elements and sets a timer to to keep them on screen
    //Run in Controller script Update method when switching
    //Sets the timed bool
    //Sets the UI text to blue
    //Sets the text appropriately
    //Creates a timer used to see how long the UI stays up
    public void playerPhaseShow()
    {
        isTimed = true;
        phaseText.color = Color.blue;
        phaseText.text = "PLAYER PHASE";
        tempTimer = Instantiate(timer);
    }

    //Displays Enemy Phase UI elements and sets a timer to keep them on screen
    //Run in Controller script Update method when switching
    //Sets the timed bool
    //Sets the UI text to blue
    //Sets the text appropriately
    //Creates a timer used to see how long the UI stays up
    public void enemyPhaseShow()
    {
        isTimed = true;
        phaseText.color = Color.red;
        phaseText.text = "ENEMY PHASE";
        tempTimer = Instantiate(timer);
    }

    //Getter for isTimed
    public bool getTimed()
    {
        return (isTimed);
    }
}
