using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unit;

public class AttackSequenceAnimation : MonoBehaviour
{
    [SerializeField] private int frameSkip;
    private int frame;
    [SerializeField] private GameObject enemy, player;
    private bool playerTurn, init = false;
    [SerializeField] float distance;
    private bool moveBack;
    private Vector3 playerPos, enemyPos;
    private int count;
    private int playerAttacks, enemyAttacks;
    private GameObject UI;
    private bool playerDead, enemyDead;
    private GameObject playerUnit, enemyUnit;
    private bool hit;
    private SpriteRenderer playerRend, enemyRend;
    private bool start = false;

    void Start()
    {
        moveBack = false;
        playerPos = player.transform.position;
        enemyPos = enemy.transform.position;
        count = 0;
        playerDead = false;
        enemyDead = false;
        playerRend = player.GetComponent<SpriteRenderer>();
        enemyRend = enemy.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            UI.GetComponent<AttackSequenceUI>().Reset();
        }

        if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            UI.GetComponent<AttackSequenceUI>().Reset();
            //setEnemyAttacks(0);
            //setPlayerAttacks(0);
        }

        if (UI.GetComponent<AttackSequenceUI>().getHpPlayer() < 1 && !playerDead)
        {
            playerDeath();
        }
        else if (UI.GetComponent<AttackSequenceUI>().getHpEnemy() < 1 && !enemyDead)
        {
            enemyDeath();
        }
    }

    private void FixedUpdate()
    {
        if (init)
        {
            frame++;

            if (frame%frameSkip == 0 && playerTurn && playerAttacks > 0)
            {
                playerMove();
            }
            else if (frame%frameSkip == 0 && !playerTurn && enemyAttacks > 0)
            {
                enemyMove();
            }
        }
    }

    private void LateUpdate()
    {
        if (start)
        {
            if (playerUnit.CompareTag("Player"))
            {
                playerRend.color = new Color(255, 255, 255);
                enemyRend.color = new Color(255, 0, 0);
            }
            else if (playerUnit.CompareTag("Enemy"))
            {
                playerRend.color = new Color(255, 0, 0);
                enemyRend.color = new Color(255, 255, 255);
            }

            start = false;
        }
    }

    public void animationInit(bool playerTurn, GameObject UI, GameObject playerUnit, GameObject enemyUnit)
    {
        this.playerTurn = playerTurn;
        init = true;
        this.UI = UI;
        this.playerUnit = playerUnit;
        this.enemyUnit = enemyUnit;
        start = true;
    }

    private void playerMove()
    {
        if (!moveBack)
        {
            
            if (getDistance() < distance)
            {
                hit = playerUnit.GetComponent<Unit>().hitCheck(enemyUnit);
                player.GetComponent<Animator>().SetBool("isRunning", false);
                moveBack = true;

                if (hit)
                {
                    player.GetComponent<Animator>().SetTrigger("isAttacking");
                    UI.GetComponent<AttackSequenceUI>().setEnemyHp(enemyUnit.GetComponent<Unit>().getHp());
                }
                else
                {
                    UI.GetComponent<AttackSequenceUI>().setMiss("MISS");
                }
            }
            else
            {
                UI.GetComponent<AttackSequenceUI>().setMiss("");
                player.GetComponent<Animator>().SetBool("isRunning", true);
                player.transform.Translate(Vector3.right);
            }
        }

        if (player.transform.position == playerPos)
        {
            moveBack = true;
            player.GetComponent<Animator>().SetBool("isRunning", false);
        }

        if (moveBack && player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            player.transform.Rotate(0, 180, 0);
            player.transform.Translate(Vector3.right);
            moveBack = false;
            count++;
        }

        if (count == 2)
        {
            count = 0;
            playerTurn = false;
            playerAttacks--;

            if (playerAttacks > enemyAttacks)
            {
                playerTurn = true;
            }
        }
    }

    private void enemyMove()
    {
        if (!moveBack)
        {

            if (getDistance() < distance)
            {
                hit = enemyUnit.GetComponent<Unit>().hitCheck(playerUnit);
                enemy.GetComponent<Animator>().SetBool("isRunning", false);
                moveBack = true;

                if (hit)
                {
                    enemy.GetComponent<Animator>().SetTrigger("isAttacking");
                    UI.GetComponent<AttackSequenceUI>().setPlayerHp(playerUnit.GetComponent<Unit>().getHp());
                }
                else
                {
                    UI.GetComponent<AttackSequenceUI>().setMiss("MISS");
                }
            }
            else
            {
                UI.GetComponent<AttackSequenceUI>().setMiss("");
                enemy.GetComponent<Animator>().SetBool("isRunning", true);
                enemy.transform.Translate(Vector3.left);
            }
        }

        if (enemy.transform.position == enemyPos)
        {
            moveBack = true;
            enemy.GetComponent<Animator>().SetBool("isRunning", false);
        }

        if (moveBack && enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            enemy.transform.Rotate(0, 180, 0);
            enemy.transform.Translate(Vector3.left);
            moveBack = false;
            count++;
 
        }

        if (count == 2)
        {
            count = 0;
            playerTurn = true;
            enemyAttacks--;

            if (enemyAttacks > playerAttacks)
            {
                playerTurn = false;
            }
        }
    }

    private void playerDeath()
    {
        player.GetComponent<Animator>().SetTrigger("death");
        playerDead = true;

    }

    private void enemyDeath()
    {
        enemy.GetComponent<Animator>().SetTrigger("death");
        enemyDead = true;
    }

    private float getDistance()
    {
        float distance;
        distance = Vector3.Distance(player.transform.position, enemy.transform.position);
        return (distance);
    }

    public int getPlayerAttacks()
    {
        return (playerAttacks);
    }

    public void setPlayerAttacks(int playerAttacks)
    {
        this.playerAttacks = playerAttacks;
    }

    public int getEnemyAttacks()
    {
        return (enemyAttacks);
    }

    public void setEnemyAttacks(int enemyAttacks)
    {
        this.enemyAttacks = enemyAttacks;
    }

    public bool getInit()
    {
        return (init);
    }

    private void setInit(bool init)
    {
        this.init = init;
    }
}
