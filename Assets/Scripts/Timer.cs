using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    //Variables
    public float time;
    private float currentTime;

    //Runs on initiation
    //Sets the current time to the max time
    void Start()
    {
        currentTime = time;
    }

    //Runs every frame
    //Checks if the current time is greater than 0
    //If true, subtract Time.deltaTime from the current time
    //If false destroy this GameObject
    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //Getter for currentTime
    public float getTime()
    {
        return (currentTime);
    }
}
