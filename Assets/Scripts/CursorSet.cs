using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unit;

public class CursorSet : MonoBehaviour
{
    //Variables
    public GameObject mainPlayer;
    public Vector3 offset;
    private bool isSelected = false;
    private GameObject tempCharacter;
    private bool isRange = false;
    private bool hasTarget = false;
    public GameObject commandController;
    private bool isCommanding = false;
    private bool enemySelected = false;
    private bool isAttacking = false;
    private bool isEnemy = false;
    [SerializeField] private int multiple;
    private int frame;

    //Called on initiation
    //Sets the default cursor position to the GameObject that is set as the "main player"
    private void Start()
    {
        transform.position = mainPlayer.transform.position + offset;
    }

    //Called every frame
    //Cursor movement calculated here
    //Cursor moves for every frame that is a multiple of the multiple variable (currently 60, so the cursor moves 1 square per second if 60fps)
    //cursor can select allies, then a destination, or an enemy to attack if the attack command was chosen from the command prompt
    //Script activates the unit's move range when initially selected
    //if cursor moves to invalid square, and presses space, move range will go away
    //Logic Bug: if cursor hovers over red square when selecting a move destination, and then goes back to select valid blue square, move range will disappear and no movement will occur
    private void Update()
    {
        frame++;

        if (Input.GetKey(KeyCode.A) && (!isCommanding || isAttacking) && frame%multiple == 0)
        {
            transform.Translate(Vector3.left);
        }

        if (Input.GetKey(KeyCode.D) && (!isCommanding || isAttacking) && frame % multiple == 0)
        {
            transform.Translate(Vector3.right);
        }

        if (Input.GetKey(KeyCode.W) && (!isCommanding || isAttacking) && frame % multiple == 0)
        {
            transform.Translate(Vector3.up);
        }

        if (Input.GetKey(KeyCode.S) && (!isCommanding || isAttacking) && frame % multiple == 0)
        {
            transform.Translate(Vector3.down);
        }

        if (isSelected && Input.GetKeyDown(KeyCode.Space))
        {
            tempCharacter.GetComponent<Unit>().getMoveVision();
            isRange = true;
            isSelected = false;
        }

        else if (!isRange && !isAttacking && Input.GetKeyDown(KeyCode.Space))
        {
            tempCharacter.GetComponent<Unit>().killAll();
        }

        else if (hasTarget && Input.GetKeyDown(KeyCode.Space) && !isCommanding)
        {
            hasTarget = false;
            isCommanding = true;
            commandController.GetComponent<CommandControl>().setUpCommand(new Vector2(transform.position.x - offset.x, transform.position.y - offset.y), this.gameObject);
        }
        else if (commandController.GetComponent<CommandControl>().getSelected() && !isAttacking)
        {
            Debug.Log("moved");
            //commandController.GetComponent<CommandControl>().Reset();
            //tempCharacter.GetComponent<Unit>().move(transform.position - offset, this.gameObject, false);
            isRange = false;
            isSelected = false;
            isCommanding = false;
            commandController.GetComponent<CommandControl>().setSelected(false);
        }
        else if (isAttacking && isEnemy && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("attacked");
            tempCharacter.GetComponent<Unit>().setTarget(selectTarget("Enemy"));
        }
    }

    //Called the frame the cursor encounters a collision
    //Checks what the cursor is standing on
    //If the cursor is standing on a valid player, that isn't already selected, and no other player is currently selected, set the tempCharacter to the GameObject connected to the collision and check off selected bool
    //If the cursor leaves the bounds, run Start() to reset the cursor position; this is a temporary fix, need to find better long term bounds check
    //If an Attack square is collided with, set enemy bool
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRange && collision.gameObject.GetComponent<Unit>().getActive())
        {
            isSelected = true;
            tempCharacter = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("Bounds"))
        {
            Start();
        }
        else if (collision.gameObject.CompareTag("Attack"))
        {
            isEnemy = true;
        }
        else if (collision.gameObject.CompareTag("Move"))
        {
            isRange = true;
        }
    }

    //Called the frame the cursor leaves a collider
    //If the player is no longer selected, set the selected bool accordingly
    //If player leaves attack squre, set range and enemy bools
    //If the player moves off of move square, set target bool
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isSelected = false;
        }

        else if (collision.gameObject.CompareTag("Attack"))
        {
            isRange = false;
            isEnemy = false;
        }
        else if (collision.gameObject.CompareTag("Move"))
        {
            hasTarget = false;
        }
    }

    //Called every frame the cursor is on a collider
    //While the cursor stands on a move square, target bool will be true
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRange && collision.gameObject.CompareTag("Move"))
        {
            hasTarget = true;
        }
    }

    //Getter for isCommanding
    private bool getIsCommanding()
    {
        return (isCommanding);
    }

    //Setter for isCommanding
    public void setIsCommanding(bool isCommanding)
    {
        this.isCommanding = isCommanding;
    }

    //Getter for tempCharacter
    public GameObject getTempcharacter()
    {
        return (tempCharacter);
    }

    //Setter for tempCharacter
    private void setTempCharacter(GameObject tempCharacter)
    {
        this.tempCharacter = tempCharacter;
    }

    //Setter for enemySelected
    private void setEnemySelected(bool enemySelected)
    {
        this.enemySelected = enemySelected;
    }

    //Getter for enemySelected
    public bool getEnemySelected()
    {
        return (enemySelected);
    }

    //Getter for attacking
    private bool getAttacking()
    {
        return (isAttacking);
    }

    //Setter for attacking
    public void setAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    //Method used for selecting an enemy to attack
    //Run from CursorSet Update method to determine valid selected enemy so the player can attack the enemy
    //Takes a string parameter that identifies what type of enemy the game is searching for
    //looks for all entities with the tag equal to type and puts them into a GameObject array
    //checks if the enemy currently being hovered by the cursor is in range of the current player (currently only deals with one range attacks
    //If true sets that GameObject in the array equal to the target
    //Finally, returns the target
    public GameObject selectTarget(string type)
    {
        GameObject[] victims = GameObject.FindGameObjectsWithTag(type);
        GameObject target = null;

        for (int bogus = 0; bogus < victims.Length; bogus++)
        {
            if (transform.position - offset == victims[bogus].transform.position && Input.GetKeyDown(KeyCode.Space))
            {
                target = victims[bogus];
                isAttacking = false;
                break;
            }
        }

        return (target);
    }
}