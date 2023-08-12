using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace unit
{
    public class Unit : MonoBehaviour
    {
        //Variables
        public int lvl, exp, hp, str, skl, spd, lck, def, res, con, mov;
        private int attack, defense;
        private int accuracy, avoid, critRate, critAvoid;
        private int maxHp;
        private int atkSpd;
        public GameObject moveSquare;
        public GameObject attackSquare;
        public float squareOffsetY;
        public float squareOffsetX;
        public float speed;
        private bool isActive;
        private SpriteRenderer spriteRender;
        private GameObject target;
        private bool isSearching = false;
        [SerializeField] private bool counterAttack;
        [SerializeField] private GameObject attackSequence, attackUI;
        private bool isAttackSequence;
        private GameObject currentAttackSequence;
        [SerializeField] private GameObject rnControl;
        private int attacks = 0;
        private bool dead = false;
        private GameObject parent;

        //Run on initiation
        //Set up the sprite renderer
        //set isActive to true
        //set the maxHP to the hp stat (used for healing when implemented)
        //set the target to null
        //set isAttackSequence to false
        void Start()
        {
            spriteRender = GetComponent<SpriteRenderer>();
            isActive = true;
            maxHp = hp;
            target = null;
            isAttackSequence = false;
            parent = transform.parent.gameObject;
        }

        //Run every frame
        //Changes the color scheme of the players and enemies as appropriate
        //Red is for active enemies
        //All color is for active players
        //Black is for inactive players and enemies
        //runs setAttack, setDefense, setAtkSpd, setAccuracy, setAvoid (used for attack calculations later)
        //Checks if searching is true, and target is valid (not null)
        //If true checks if attackSequence is true
        //If true runs the attackSequence code (not fully implemented and breaks game currently)
        //Then runs hitCheck() and sets isSearching to false
        void Update()
        {
            if (isActive)
            {
                spriteRender.color = new Color(255, 255, 255);
            }
            else
            {
                spriteRender.color = new Color(0, 0, 0);
            }

            setAttack();
            setDefense();
            setAtkSpd();
            setAccuracy();
            setAvoid();
            setCritRate();
            setCritAvoid();

            if (isSearching && target)
            {
                if (!isAttackSequence)
                {
                    doubleOpp(target);
                    target.GetComponent<Unit>().doubleOpp(this.gameObject);
                    currentAttackSequence = Instantiate(attackSequence);
                    attackUI.SetActive(true);
                    isAttackSequence = true;
                    attackUI.GetComponent<AttackSequenceUI>().sequenceInit(this.gameObject, target, currentAttackSequence, getAttacks(), target.GetComponent<Unit>().getAttacks());
                }

                //hitCheck(target);
                isSearching = false;
            }
        }

        //Method used to calculate the movement vision and intantiate the squares on the space
        //Run in CursorSet script Update method
        //Intantiates an entire row of blue move squares
        //Adds attack squares on the ends
        //Moves to next row and repeats
        //Goes for as long as the value of mov is
        //Calculates move correctly, however, doesn't account for enemies blocking the way or terrain
        //If there is an enemy or inpassable terrain, red sqaure appears in its place to signify invalid square
        public void getMoveVision()
        {
            this.gameObject.GetComponent<Animator>().SetBool("isSelected", true);
            Vector3 tempOffset = new Vector3(0, 0, 0);
            int nonsense = 0;
            for (int bogus = 0; bogus <= mov; bogus ++)
            {
                tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, 0.01f);
                Instantiate(attackSquare, (transform.position + tempOffset) + (new Vector3(1, 0, 0) * (bogus - (mov + 1))), new Quaternion(0, 0, 0, 0), transform);

                if (bogus ==0)
                {

                }
                else
                {
                    tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, -0.01f);
                    Instantiate(attackSquare, (transform.position - (tempOffset - new Vector3(0, 1, 0))) + (new Vector3(1, 0, 0) * (bogus - (mov + 1))), new Quaternion(0, 0, 0, 0), transform);
                }

                for (int bogus2 = (mov * -1) + bogus; bogus2 <= mov - bogus; bogus2++)
                {
                    tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, 0.01f);
                    Instantiate(moveSquare, (transform.position + tempOffset) + (new Vector3(1, 0, 0) * bogus2), new Quaternion(0, 0, 0, 0), transform);

                    if (bogus == 0)
                    {

                    }
                    else
                    {
                        tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, -0.01f);
                        Instantiate(moveSquare, (transform.position - (tempOffset - new Vector3(0, 1, 0))) + (new Vector3(1, 0, 0) * bogus2), new Quaternion(0, 0, 0, 0), transform);
                    }
                }

                tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, 0.01f);
                Instantiate(attackSquare, (transform.position + tempOffset) + (new Vector3(1, 0, 0) * ((mov + 1) - bogus)), new Quaternion(0, 0, 0, 0), transform);

                if (bogus == 0)
                {

                }
                else
                {
                    tempOffset = new Vector3(squareOffsetX, squareOffsetY + bogus, -0.01f);
                    Instantiate(attackSquare, (transform.position - (tempOffset - new Vector3(0, 1, 0))) + (new Vector3(1, 0, 0) * ((mov + 1) - bogus)), new Quaternion(0, 0, 0, 0), transform);
                }

                nonsense = bogus;
            }

            tempOffset = new Vector3(squareOffsetX, squareOffsetY + nonsense, 0.01f);
            Instantiate(attackSquare, transform.position + (tempOffset + new Vector3(0, 1, 0)), new Quaternion(0, 0, 0, 0), transform);
            tempOffset = new Vector3(squareOffsetX, squareOffsetY + nonsense, -0.01f);
            Instantiate(attackSquare, transform.position - tempOffset, new Quaternion(0, 0, 0, 0), transform);
            //worry about terrain later
        }

        //Method used to kill all of the unit's children
        //Loops through all the children of the unit GameObject and destroys them all
        //Used to get rid of movement/attack squares
        public void killAll()
        {
            this.gameObject.GetComponent<Animator>().SetBool("isSelected", false);
            int count = transform.childCount;

            for (int bogus = 0; bogus < count; bogus++)
            {
                Destroy(this.gameObject.transform.GetChild(bogus).gameObject);
            }
        }

        //Method used to move the player unit to a selected space
        //Takes target, cursor, and attack parameters
        //Makes the cursor inactive
        //Sets x, y, and z floats equal to the target vector's x, y, and z
        //Find the x and y movement vectors
        //Find the x and y directions by dividing them by their absolute value
        //Catches a DivideByZero exception and set the move vector to 0
        //Move the unit until they react the target destination
        //Run killAll method
        //Make the cursor active
        //If the attack bool is true, set up for attack selection
        //If not, run Reset method
        public void move(Vector3 target, GameObject cursor, bool isAttack)
        {
            cursor.SetActive(false);
            float x = target.x;
            float y = target.y;
            float z = target.z;
            int loopX = (int) (x - transform.position.x);
            int loopY = (int) (y - transform.position.y);
            Vector3 xMove;
            Vector3 yMove;

            if (loopX != 0)
            {
                xMove = new Vector3(loopX / Math.Abs(loopX), 0, 0);
            }
            else
            {
                xMove = new Vector3(0, 0, 0);
            }

            if (loopY != 0)
            {
                yMove = new Vector3(0, loopY / Math.Abs(loopY), 0);
            }
            else
            {
                yMove = new Vector3(0, 0, 0);
            }

            while (transform.position.x != target.x)
            {
                transform.Translate(xMove * speed);
            }

            while (transform.position.y != target.y)
            {
                transform.Translate(yMove * speed);
            }

            killAll();
            cursor.SetActive(true);

            if (!isAttack)
            {
                Reset(true);
            }
            else
            {
                getAttackVision();
            }
        }

        //Method used to set up attack squares adjacent to the unit after moving, but before attacking
        //Run from Unit script move method if attack bool was true
        //Loops from -1 to 1 and spawns attack squares in 4 spaces where x is -1 and 1 and y is -1 and 1 (tiles are all adjacent to unit)
        private void getAttackVision()
        {
            for (int bogus = -1; bogus <= 1; bogus++)
            {
                if (bogus != 0)
                {
                    Instantiate(attackSquare, new Vector3(transform.position.x + bogus + squareOffsetX, transform.position.y + squareOffsetY, 0.01f), new Quaternion(0, 0, 0, 0), transform);
                    Instantiate(attackSquare, new Vector3(transform.position.x + squareOffsetX, transform.position.y + bogus + squareOffsetY, 0.01f), new Quaternion(0, 0, 0, 0), transform);
                }
            }
        }

        //Method used to reset multiple things
        //Runs killAll method
        //Sets the target to null
        //Sets isActive to false
        public void Reset(bool initiated)
        {
            killAll();
            setTarget(null);
            isAttackSequence = false;
            currentAttackSequence = null;

            if (initiated)
            {
                setActive(false);
            }
        }

        //Enemy only script
        //Activated by EnemyControl.moveInit()
        //Searches for all player objects
        //Runs enemyMoveCalc() with players parameter
        //is returned the player closest to the enemy
        //Sets player's position to the enemy's target and calculates direction of the player
        //runs a while loop to allign the enemy on the x position of the target
        //runs similar while loop to allign to y position of the target
        //while loops stop when the position is met, or if tempMov goes to 0, because this means the enemy used up all their movement for the turn
        //finally, runs setActive() with false parameter which helps the EnemyControl script know what enemies moved, and what enemies did not move
        //Edit: DivideByZeroException still activated even though I thought it would not; need to look more into this
        public void enemyMove()
        {
            GameObject[] players;
            GameObject closest;
            players = GameObject.FindGameObjectsWithTag("Player");
            closest = enemyMoveCalc(players);
            Vector3 target = closest.transform.position;
            float x = target.x;
            float y = target.y;
            int loopX = (int)(x - transform.position.x);
            int loopY = (int)(y - transform.position.y);
            Vector3 xMove;
            Vector3 yMove;

            if (loopX != 0)
            {
                xMove = new Vector3(loopX / Math.Abs(loopX), 0, 0);
            }
            else
            {
                xMove = new Vector3(0, 0, 0);
            }

            if (loopY != 0)
            {
                yMove = new Vector3(0, loopY / Math.Abs(loopY), 0);
            }
            else
            {
                yMove = new Vector3(0, 0, 0);
            }

            if (xMove.x < 0)
            {
                x++;
            }
            else if (xMove.x > 0)
            {
                x--;
            }
            else if (yMove.y < 0)
            {
                y++;
            }
            else if (yMove.y > 0)
            {
                y--;
            }

            int tempMov = mov;

            while (transform.position.x != x && tempMov > 0)
            {
                transform.Translate(xMove * speed);
                tempMov--;
            }

            while (transform.position.y != y && tempMov > 0)
            {
                transform.Translate(yMove * speed);
                tempMov--;
            }

            if (transform.position.x == x && transform.position.y == y)
            {
                Debug.Log(x + ", " + y);
                Debug.Log(transform.position);
                isSearching = true;
                setTarget(closest);
            }
            else
            {
                Reset(true);
            }
        }

        //Enemy only script
        //Calculates closest gameObject to the enemy
        //Used to see which player is closest
        //Sets the return gameObject to second gameObject in array
        //Compares said return value to the object before it in the array
        //Checks to see which is closer, by using the distance formula
        //If the new distance is less than the original distance, overwrite the player and distance to the new one
        //Efficiency Fix: have the for loop run from int = 1 to int = length + 1, instead of starting at 0
        //After loop, return the player variable
        //Called in enemyMove()
        //Bug: will fail if only one player exists; must fix later
        private GameObject enemyMoveCalc(GameObject[] players)
        {
            double distance = 0;
            double tempDistance = 0;
            GameObject player = null;
            GameObject tempPlayer = null;
            bool exists = true;

            for (int bogus = 0; bogus < players.Length + 1; bogus++)
            {
                if (bogus == 1)
                {
                    try
                    {
                        player = players[bogus];
                        distance = Math.Sqrt(Math.Pow(this.gameObject.transform.position.x - player.transform.position.x, 2) + Math.Pow(this.gameObject.transform.position.y - player.transform.position.y, 2));
                    }
                    catch
                    {
                        exists = false;
                    }
                }

                if (bogus > 0)
                {
                    tempPlayer = players[bogus - 1];
                    tempDistance = Math.Sqrt(Math.Pow(this.gameObject.transform.position.x - tempPlayer.transform.position.x, 2) + Math.Pow(this.gameObject.transform.position.y - tempPlayer.transform.position.y, 2));

                    if (tempDistance < distance || !exists)
                    {
                        distance = tempDistance;
                        player = tempPlayer;
                    }
                }
            }

            return (player);
        }

        //Getter for isActive
        public bool getActive()
        {
            return (isActive);
        }

        //Setter for isActive
        public void setActive(bool active)
        {
            isActive = active;
        }

        //Initiates attack sequence
        //Run from Attack script onClick method
        //Runs move method
        //Tells CursorSet script's attacking bool to true
        //Sets isSearching to true
        public void attackInit(String type, Vector3 cursorPos, GameObject cursor)
        {
            move(cursorPos, cursor, true);
            cursor.GetComponent<CursorSet>().setAttacking(true);
            isSearching = true;
        }

        //Checks the hit rate of the incoming attack
        //calculates true accuraxy by subtracting unit's accuracy from the victims avoid
        //Then generates two random float values, and averages them
        //If the average is less than trueAcc, the attack hits, and damage() runs
        //If not, the attack misses
        //Visual Bug: attack sequence does not play, as it has not been implemented or created yet, therefore, animations are immidiate (or as fast as the computer can calculate)
        public bool hitCheck(GameObject victim)
        {
            int trueAcc = getHit(victim);
            Debug.Log(trueAcc);
            int ave = rnControl.GetComponent<RandomNumberGenerator>().randomNumberGenerator(new int[2]);
            bool crit;
            Debug.Log(ave);
            attacks--;
            //Reset(true);

            if (ave < trueAcc)
            {
                crit = critCheck(victim);
                damage(victim, crit);
                return (true);
            }
            else
            {
                Debug.Log("miss");
                //victim.GetComponent<Unit>().checkDouble(this.gameObject);
                return (false);
            }
        }

        private bool critCheck(GameObject target)
        {
            int trueAcc = getCrit(target);
            int ave = rnControl.GetComponent<RandomNumberGenerator>().randomNumberGenerator(new int[2]);

            if (ave < trueAcc)
            {
                return (true);
            }

            return (false);

        }

        //Calculates damage done to the victim
        //less than 0 damage is not possible, and is changed back to 0 damage
        //Sends damage number to the victim so it can subtract the damage number
        private void damage(GameObject victim, bool crit)
        {
            int damage = getDamage(victim);

            if (damage < 0)
            {
                damage = 0;
            }

            victim.GetComponent<Unit>().takeDamage(damage, this.gameObject, crit);
        }

        //Subtracts damage from current hp
        //If hp drops below 1, unit dies
        //If not unit is still alive
        //Checks if a counter attack can happen and runs it if possible
        public void takeDamage(int damage, GameObject opponent, bool crit)
        {
            if (crit)
            {
                damage *= 3;
            }
            hp = hp - damage;

            if (damage == 0 && opponent.CompareTag("Player"))
            {
                opponent.GetComponent<Unit>().setExp(0);
            }
            else if (hp < 1)
            {
                if (opponent.CompareTag("Player"))
                {
                    opponent.GetComponent<Unit>().setExp(2);
                }
                Destroy(this.gameObject);
            }
            else
            {
                if (opponent.CompareTag("Player"))
                {
                    opponent.GetComponent<Unit>().setExp(1);
                }
                Debug.Log(this.gameObject.name + " took " + damage + " damage");
            }
        }

        private void checkDouble(GameObject opponent)
        {
            if (getAttacks() > 0)
            {
                isAttackSequence = true;
                this.isSearching = true;
                setTarget(opponent);
            }
        }

        private void doubleOpp(GameObject target)
        {
            int targetAtkSpd = target.GetComponent<Unit>().getAtkSpd();

            if ((atkSpd - targetAtkSpd) >= 4)
            {
                attacks = 2;
            }
            else
            {
                attacks = 1;
            }

            Debug.Log(attacks);
        }

        //Getter for entities
        private GameObject[] getEntities(String type)
        {
            GameObject[] entities = GameObject.FindGameObjectsWithTag(type);
            return (entities);
        }

        //Getter for attack
        private int getAttack()
        {
            return (attack);
        }

        //Setter for attack
        //Subject to change when weapons/advantage/effectiveness is implemented
        private void setAttack()
        {
            attack = str;
        }

        //Getter for defense
        private int getDefense()
        {
            return (defense);
        }

        //Setter for defense
        //Subject to change when terrain
        private void setDefense()
        {
            defense = def;
        }

        //Getter for accuracy
        private int getAccuracy()
        {
            return (accuracy);
        }

        //Setter for accuracy
        private void setAccuracy()
        {
            accuracy = 75 + (skl * 2) + (lck / 2);
        }

        //Getter for avoid
        public int getAvoid()
        {
            return (avoid);
        }

        //Setter for avoid
        private void setAvoid()
        {
            avoid = (atkSpd * 2) + lck;
        }

        //Getter for atkSpd
        private int getAtkSpd()
        {
            return (atkSpd);
        }

        //Setter for atkSpd
        private void setAtkSpd()
        {
            atkSpd = spd;
        }

        //Getter for target
        public GameObject getTarget()
        {
            return (target);
        }

        //Setter for target
        public void setTarget(GameObject target)
        {
            this.target = target;
        }

        //Method for calculating true accuracy
        //Takes the unit's accuracy and subtracts the victim's avoid
        public int getHit(GameObject victim)
        {
            int trueAcc = accuracy - victim.GetComponent<Unit>().getAvoid();
            return (trueAcc);
        }

        //Method for calculating damage
        //Takes the unit's attack and subtracts by the victim's defense
        //Subject to change as more is implemented
        public int getDamage(GameObject victim)
        {
            int damage = attack - victim.GetComponent<Unit>().getDefense();

            if (damage < 0)
            {
                damage = 0;
            }
            return (damage);
        }

        private void levelUp()
        {
            hp++;
            maxHp++;
            str++;
            skl++;
            spd++;
            lck++;
            def++;
            res++;
            Debug.Log("Leveled up");
        }

        //Getter for hp
        public int getHp()
        {
            if (hp < 0)
            {
                hp = 0;
            }

            return (hp);
        }

        //Setter for hp
        private void setHp(int hp)
        {
            this.hp = hp;
        }

        public int getAttacks()
        {
            return (attacks);
        }

        private void setAttacks(int attacks)
        {
            this.attacks = attacks;
        }

        private int getCritRate()
        {
            return (critRate);
        }

        private void setCritRate()
        {
            critRate = (skl + lck)/ 2;
        }

        public int getCritAvoid()
        {
            return (critAvoid);
        }

        private void setCritAvoid()
        {
            critAvoid = lck;
        }

        public int getCrit(GameObject target)
        {
            int crit = critRate - target.GetComponent<Unit>().getCritAvoid();

            if (crit < 0)
            {
                crit = 0;
            }

            return (crit);
        }

        public bool getDead()
        {
            return (dead);
        }

        private void setDead(bool dead)
        {
            this.dead = dead;
        }

        private int getExp()
        {
            return (exp);
        }

        public void setExp(int modifier)
        {
            exp += 50 * modifier;

            if (exp > 99)
            {
                exp -= 100;
                levelUp();
            }

            Debug.Log(exp);
        }
    }
}
