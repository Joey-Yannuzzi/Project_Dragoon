using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    //Variables
    private int phase; //0 = player, 1 = enemy, 2 = ally, 3 = 3rd army, -1 = inactive
    private bool start;
    private bool isEnemy = false;
    private bool isPlayer = false;
    private bool isAlly = false;
    private bool is3rd = false;
    public GameObject cursor;
    private int count;
    public GameObject playerController;
    public GameObject enemyController;
    private bool enemyStart;
    private int runCount;
    [SerializeField] private GameObject attackUI;

    //Run on initiation
    //Sets runCount to 0
    //Sets phase to -1
    //Sets start to true
    //disables the cursor GameObject
    //sets enemy start to false
    //Makes the cursor disappear
    //Sets the frame rate to 60 FPS (this is done to keep the cursor and other frame dependent features moving at a constant rate on different machines
    void Start()
    {
        runCount = 0;
        phase = -1;
        setStart(true);
        cursor.SetActive(false);
        enemyStart = false;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
    }

    //Runs every frame
    //Handles running phase releated methods and switching phases
    //Checks which phase it currently is and runs appropriate method depending on phase
    //If start is true checks which phase it should be by cycling through the phase variable
    //Sets the appropriate phase bool true and runs the appropriate UI method in the TextControl script
    //Looks for escape key input and makes cursor visable when pressed (used for debug purposes)
    void Update()
    {
        if (attackUI.GetComponent<AttackSequenceUI>().getActive())
        {
            cursor.SetActive(false);
        }
        else if (!attackUI.GetComponent<AttackSequenceUI>().getActive() && isPlayer)
        {
            cursor.SetActive(true);
        }

        if (isPlayer)
        {
            playerPhase();
        }
        else if (isEnemy)
        {
            enemyPhase();
        }
        else if (isAlly)
        {
            allyPhase();
        }
        else if (is3rd)
        {
            armyPhase();
        }
        else if (start)
        {
            switch (phase)
            {
                case 0:
                    isPlayer = true;
                    this.gameObject.GetComponent<TextControl>().playerPhaseShow();
                    break;
                case 1:
                    isEnemy = true;
                    this.gameObject.GetComponent<TextControl>().enemyPhaseShow();
                    break;
                case 2:
                    isAlly = true;
                    break;
                case 3:
                    is3rd = true;
                    break;
            }
            start = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
    }

    private void LateUpdate()
    {
        string winner = "";

        if (playerController.GetComponent<PlayerControl>().getWin())
        {
            winner = "Enemy";
        }
        else if (enemyController.GetComponent<EnemyControl>().getWin())
        {
            winner = "Player";
        }

        endGame(winner);
    }

    private void endGame(string winner)
    {
        if (!attackUI.GetComponent<AttackSequenceUI>().getActive())
        {
            if (winner.Equals("Player"))
            {
                SceneManager.LoadScene("Win", LoadSceneMode.Single);
            }

            else if (winner.Equals("Enemy"))
            {
                SceneManager.LoadScene("Lose", LoadSceneMode.Single);
            }
        }
    }

    // Getter for start
    public bool getStart()
    {
        return (start);
    }

    //Setter for start
    //Also resets the phase so the loop can begin again
    //Runs the Reset methods of the player and enemy controllers
    //Incriments the phase counter
    //Sets runCount to 0
    private void setStart(bool start)
    {
        playerController.GetComponent<PlayerControl>().Reset();
        enemyController.GetComponent<EnemyControl>().Reset();
        this.start = start;
        phase++;
        runCount = 0;
    }

    //Method used to create player phase game loop
    //Run in Controller script Update method every frame that it is player phase
    //Checks if the GameObject's TextControl isTimed bool is false and if runCount is 0
    //Will be true the frame after the timer is done and the UI elements are gone
    //If true activates the cursor and incriments runCount (this is so it doesn't run every frame after the UI is gone)
    //Then checks if the playerController GameObject's PlayerControl script active bool is false
    //If true deactivate the player bool, run setStart method, and deactivate the cursor to prepare for next phase
    private void playerPhase()
    {
        if (!this.gameObject.GetComponent<TextControl>().getTimed() && runCount == 0)
        {
            cursor.SetActive(true);
            runCount++;
        }
        if (!playerController.gameObject.GetComponent<PlayerControl>().getActive() && !attackUI.GetComponent<AttackSequenceUI>().getActive())
        {
            isPlayer = false;
            setStart(true);
            cursor.SetActive(false);
        }
    }

    //Method used to create enemy phase game loop
    //Run in Controller script Update method every frame that it is enemy phase
    //Checks if the GameObject's TextControl isTimed bool is false
    //Will be true every frame after the timer is done and the UI elements are gone (there is no runCount in this script as it is unessasary)
    //If true sets enemyStart to true
    //Then checks if the enemyController GameObject's EnemyControl script active bool is false
    //If true deactivate the enemy bool, set enemyStart to false, and run setStart method to prepare for mext phase
    private void enemyPhase()
    {
        if (!this.gameObject.GetComponent<TextControl>().getTimed())
        {
            setEnemyStart(true);
        }
        if (!enemyController.gameObject.GetComponent<EnemyControl>().getActive())
        {
            isEnemy = false;
            setEnemyStart(false);
            setStart(true);
        }
        //Debug.Log("Enemy Phase start");
    }

    //Method used to create ally phase game loop
    //Allies are not implemented yet, so this method is unused at the moment
    private void allyPhase()
    {
        //Debug.Log("Ally phase start");
        isAlly = false;
        setStart(true);
    }

    //Method used to create a third army phase game loop
    //The third army is not implemented yet, so this method is unused at the moment
    private void armyPhase()
    {
        //Debug.Log("3rd Team phase start");
        is3rd = false;
        phase = -1;
        setStart(true);
    }

    //Setter for enemyStart
    private void setEnemyStart(bool enemyStart)
    {
        this.enemyStart = enemyStart;
    }

    //Getter for enemyStart
    public bool getEnemyStart()
    {
        return (enemyStart);
    }
}
