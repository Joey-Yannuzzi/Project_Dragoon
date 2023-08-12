using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Variables
    public GameObject attackSquare;

    //Method used to distinguish between different terrains
    //Runs whenever GameObject touches a collider
    //Checks if the collided GameObject has the "Impassable" tag
    //If true, destroy this GameObject (since it is impassable, nothing can exist in this space, therefore, a unit cannot move/attack on this space)
    //Then checks if the collided GameObject is a mountain, enemy, and if this GameObject is a move square
    //If true place an attack square down where this GameObject is and then destroy this GameObject
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Impassable"))
        {
            Destroy(this.gameObject);
        }
        else if ((collision.gameObject.CompareTag("Mountain") || collision.gameObject.CompareTag("Enemy")) && this.gameObject.CompareTag("Move"))
        {
            Instantiate(attackSquare, transform.position, new Quaternion(0, 0, 0, 0), transform.parent.transform);
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Player") && collision.gameObject != transform.parent.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
}
