using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMaceScript : MonoBehaviour
{
    enum BossState {
        Intro,
        Idle,
        // Patrol,
        Jump,
        COUNT
    }
    BossState currentState;
    private Animator anim;
    public float introDuration = 3f;
    private float currIntroTime;
    public float idleDuration = 2.0f;
    private float currIdleTime;
    public float patrolIntroDuration = 2.0f; 
    private float currPatrolIntroTime;
    public float patrolDuration = 10.0f;
    private float currPatrolTime;
    public Transform[] patrolSpots;
    private Transform currentDestination;
    public float offset = 1.0f;
    public float patrolSpeed = 10.0f;


    public float jumpIntroDuration = 2.0f;
    private float currJumpIntroTime;
    private bool onStage;
    public Transform stageCheck;
    public LayerMask whatIsStage;
    public float stageCheckRadius = 0.5f;
    // public float jumpTimeDuration = 10.0f;
    // private float currJumpTime;
    public int totalJumps = 4;
    private int jumpsLeft;
    public GameObject player;
    public float moveSpeed = 1000.0f;
    public float jumpPower = 1000.0f;
    private bool jumped;

    



    private Rigidbody2D rb2d;
    private SpriteRenderer sr2d;
    public PlayerScript playerScript;
    private CameraScript cam;
    public GameObject maceDeathEffect;

    public Image hpBar;
    public float totalHealth = 100.0f;
    private float health;
    public float damagedFlashTime = .4f;
    public float flashRotations = 2;
    public float InvulnerableCD = 1.0f;
    private float elapsedInvulnerablTime;
    public float damageDealt = 5f;





    // private bool isPatrolling;
    // private int farthestIdx;
    // private float farthest;
    // private bool isPatrolIntro;
    // private bool isJumping;

    // public float speed = 20f;
    // public float delay = 1f;
    // public Transform target;
    // private float posY;

    // private float timeLeft;
    // private bool playerFound;
    // private float offsetX = 2f;


    // public Transform PlayerCheck;
    // public float PlayerCheckX;
    // public float PlayerCheckY;

    // private float colorChangeTime;



    // public float InvulnerableInterval = .1f;

    // public float sawAttackCD = 1f;
    // private float currentSawAttack;
    // public Transform sawAttackPos1;
    // public GameObject sawPrefab;
    // public GameObject sawPrefabIndestructible;
    // public GameObject[] spikePrefabs;



    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentState = BossState.Intro;
        currIntroTime = introDuration;
        currIdleTime = idleDuration;
        currPatrolIntroTime = patrolIntroDuration;
        currPatrolTime = patrolDuration;
        currentDestination = patrolSpots[0];
        currJumpIntroTime = jumpIntroDuration;
        // currJumpTime = jumpTimeDuration;
        jumpsLeft = totalJumps;
        jumped = false;

        rb2d = GetComponent<Rigidbody2D>();
        // rb2d.gravityScale = 0;
        sr2d = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraScript>();
        health = totalHealth;
        elapsedInvulnerablTime = 0.0f;

        // anim = GetComponent<Animator>();
        // currentState = BossState.Intro;

        // currIntroTime = IntroDuration;

        // currPatrolIntroTime = patrolIntroDuration;
        // isPatrolIntro = false;

        // currIdleTime = idleDuration;

        // farthestIdx = 0;
        // farthest = 0.0f;
        // currPatrolTime = patrolDuration;
        // currentDestination = patrolSpots[0];

        // isPatrolling = false;

        // isJumping = false;
        // eyeR.SetActive(false);
        // eyeL.SetActive(false);
        // currentSawAttack = 0.0f;
        // target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        // posY = transform.position.y;
        // timeLeft = delay;
        // GoToNextState();

    }

    // Update is called once per frame
    void Update()
    {
        // CheckPlayerFound();
    }

    void FixedUpdate ()
    {
        if (currentState == BossState.Intro)
        {
            if (currIntroTime <= 0)
            {
                currentState = BossState.Idle;
            }
            currIntroTime -= Time.deltaTime;
            Debug.Log("Intro");
        } else if (currentState == BossState.Idle)
        {
            if (currIdleTime <= 0)
            {
                currentState = (BossState)UnityEngine.Random.Range(2, (int)BossState.COUNT);
                Debug.Log("NEW STATE: " + currentState.ToString());
                currIdleTime = idleDuration;
            } else {
                if (currIdleTime == idleDuration)
                {
                    anim.SetTrigger("isIdling");
                }
                currIdleTime -= Time.deltaTime;
                Debug.Log("Idling");
            }
        // } else if (currentState == BossState.Patrol)
        // {
        //     if (currPatrolIntroTime <= 0){
        //         Patrol();
        //     } else {
        //         if (currPatrolIntroTime == patrolIntroDuration)
        //         {
        //             anim.SetTrigger("isPatrolIntro");
        //         }
        //         currPatrolIntroTime -= Time.deltaTime;
        //         Debug.Log("Patrol Intro");
        //     }
        } else if (currentState == BossState.Jump)
        {
            if (currJumpIntroTime <= 0)
            {
                Jump();
            } else {
                if (currJumpIntroTime == jumpIntroDuration)
                {
                    anim.SetTrigger("isJumpIntro");
                }
                currJumpIntroTime -= Time.deltaTime;
                Debug.Log("Jump Intro");
            }
        }
    }

    private void Patrol()
    {
        if (currPatrolTime <= 0 && Vector2.Distance(transform.position, currentDestination.position) < offset)
        {
            currentState = BossState.Idle;
            currPatrolIntroTime = patrolIntroDuration;
            currPatrolTime = patrolDuration;

        } else 
        {
            if (currPatrolTime == patrolDuration)
            {
                anim.SetTrigger("isPatrolling");
            }
            if (Vector2.Distance(transform.position, currentDestination.position) < offset)
            {
                if (currentDestination == patrolSpots[0])
                {
                    currentDestination = patrolSpots[1];
                } else if (currentDestination == patrolSpots[1])
                {
                    currentDestination = patrolSpots[0];
                }
            }
            transform.position = Vector2.MoveTowards(transform.position, currentDestination.position, patrolSpeed * Time.deltaTime);
            currPatrolTime -= Time.deltaTime;
            Debug.Log("Patrolling");
        }
    }

    private void Jump()
    {


        // onStage = Physics2D.OverlapCircle(stageCheck.position, stageCheckRadius, whatIsStage);
        // anim.SetBool("onStage", onStage);
        // if (onStage && jumpsLeft <= 0)
        // {
        //     currentState = BossState.Idle;
        //     currJumpIntroTime = jumpIntroDuration;
        //     // currJumpTime = jumpTimeDuration;
        //     jumpsLeft = totalJumps;
        //     jumped = false;
        // } else {
        //     if (jumpsLeft > 0)
        //     {
        //         if(onStage && !jumped)
        //         {
                    
        //             rb2d.AddForce(new Vector2(0.0f, jumpPower) * Time.deltaTime, ForceMode2D.Force);
        //             jumpsLeft--;
        //             jumped = true;
        //             Debug.Log("Did Jump");

        //         } else if (jumped)
        //         {
        //             if (player.transform.position.x < transform.position.x) {
        //                 rb2d.velocity = new Vector2(-1 * moveSpeed * Time.deltaTime, rb2d.velocity.y);
        //             } else if (player.transform.position.x > transform.position.x) 
        //             {
        //                 rb2d.velocity = new Vector2(1* moveSpeed * Time.deltaTime, rb2d.velocity.y);
        //             }
        //             if (rb2d.velocity.y == 0)
        //             {
        //                 jumped = false;
        //             }
        //         }
        //     }
        // }
    }

    // private void CheckPlayerFound()
    // {
    //     // // Physics2D.OverlapBox()
    //     // if (target.position.x < transform.position.x + offsetX && target.position.x > transform.position.x)
    //     // {
    //     //     // Debug.Log("player found");
    //     //     playerFound = true;
    //     // }
    // }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(PlayerCheck.position, new Vector3(PlayerCheckX, Player,0));
    // }

    // void FixedUpdate()
    // {
        // if (currentState == BossState.Intro && Time.time <= currIntroTime)
        // {
        //     Debug.Log("Intro");
        //     return;
        // } 
        // else
        // {
        //     if (currIdleTime <= 0)
        //     {
        //         currentState = (BossState)UnityEngine.Random.Range(2, (int)BossState.COUNT);
        //     } else {
        //         Debug.Log("Idle");
        //         if(currIdleTime == idleDuration)
        //         {
        //             anim.SetTrigger("isIdling");
        //         }
        //         currIdleTime -= Time.deltaTime;
        //     }
        // }
        
        // {
            // if (!isPatrolling)
            // {
            //     currentState = (BossState)UnityEngine.Random.Range(2, (int)BossState.COUNT);
                
            //     //reset patrol times
            //     // currPatrolIntroTime = patrolIntroDuration;
            //     // currPatrolTime = patrolDuration;
            //     // isPatrolIntro = false;

            // }
            // if (currentState == BossState.Patrol)
            // {
                // isPatrolling = true;
                // Debug.Log("Patrol");
                // Patrol();
                // sr2d.material.color = Color.green;
            // } 
            // else if (currentState == BossState.Jump)
            // {
            //     Debug.Log("Jump");
            //     sr2d.material.color = Color.yellow;
            // }
    //     }
    // }

    // private void Patrol()
    // {
    //     if (currPatrolIntroTime > 0)
    //     {
    //         if (currPatrolIntroTime == patrolIntroDuration)
    //         {
    //             anim.SetTrigger("isPatrolIntro");
    //         }
    //         Debug.Log("Patrol Intro");
    //         currPatrolIntroTime -= Time.deltaTime;
    //     } 
    //     else 
    //     {
    //         if(currPatrolTime == patrolDuration)
    //         {
    //             anim.SetTrigger("isPatrolling");
    //         }
    //         if (currPatrolTime <= 0 && Vector2.Distance(transform.position, currentDestination.position) < offset)
    //         {
    //             currentState = BossState.Idle;
    //             currIdleTime = idleDuration;
    //             return;
    //         }
    //         while(Vector2.Distance(transform.position, currentDestination.position) < offset)
    //         {
    //             if (currentDestination == patrolSpots[0])
    //             {
    //                 currentDestination = patrolSpots[1];
    //             } else if (currentDestination == patrolSpots[1])
    //             {
    //                 currentDestination = patrolSpots[0];
    //             }
    //             // currentDestination = patrolSpots[UnityEngine.Random.Range(0, patrolSpots.Length)];
    //         }
    //         transform.position = Vector2.MoveTowards(transform.position, currentDestination.position, patrolSpeed * Time.deltaTime);
    //         currPatrolTime -= Time.deltaTime;

            // //Patrol Intro over
            // if (currPatrolTime <= 0 && Vector2.Distance(rb2d.transform.position, patrolSpots[farthestIdx].position) < offset)
            // {
            //     //Patrol Time over and reached location
            //     //Stop Patrolling
            //     Debug.Log("Stop Patrolling");
            //     isPatrolling = false;
            //     return;
            // } else if (Vector2.Distance(rb2d.transform.position, patrolSpots[farthestIdx].position) < offset)
            // {
            //     Debug.Log("change points");
            //     //choose next point to go to
            //     for (int i = 0; i < patrolSpots.Length; i++)
            //     {
            //         float temp = Vector2.Distance(rb2d.transform.position, patrolSpots[i].position);
            //         Debug.Log("temp: " + temp.ToString());
            //         if (temp > farthest)
            //         {
            //             farthest = temp;
            //             Debug.Log("farthest: " + farthest.ToString());
            //             farthestIdx = i;
            //         }
            //     }
            // } else 
            // {
            //     rb2d.transform.position = Vector2.MoveTowards(rb2d.transform.position, patrolSpots[farthestIdx].position, patrolSpeed * Time.deltaTime);
                // isPatrolling = true;
                // anim.SetBool("isPatrolling", isPatrolling);
            //     currPatrolTime -= Time.deltaTime;
            //     Debug.Log("Is Patrolling: " + currPatrolTime.ToString());
            // }
    //     }
    // }

    // if (Time.time > currentSawAttack)
    // {
    //     if (UnityEngine.Random.Range(0, 2) == 0)
    //     {
    //         for (int i=0; i < 5; i++)
    //         {
    //         GameObject obj = Instantiate(spikePrefabs[0], sawAttackPos1.position, Quaternion.identity);
    //         obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, UnityEngine.Random.Range(-7f, 7f));
    //         // GameObject object = Instantiate(spikePrefabs[0], sawAttackPos1.transform.position, Quaternion.identity);
    //         }
    //     } else {
    //         for (int i=0; i < 5; i++)
    //         {
    //         Debug.Log("Angled");
    //         GameObject obj = Instantiate(spikePrefabs[1], sawAttackPos1.transform.position, Quaternion.identity);
    //         obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, UnityEngine.Random.Range(-7f, 7f));
    //         }
    //     }
    //     currentSawAttack = Time.time + sawAttackCD;
    // }


    // void GoToNextState()
    // {
    //     BossState nextState = (BossState)UnityEngine.Random.Range(0, (int)BossState.COUNT);
    //     string nextStateString = nextState.ToString();
    //     string lastStateString = currentState.ToString();
    //     currentState = nextState;
    //     StopCoroutine(lastStateString);
    //     StartCoroutine(nextStateString);
    // }

    // IEnumerator Patrol()
    // {
    //     yield return null;
    //     Debug.Log("Idle");
    //     // GoToNextState();
    // }

    // IEnumerator Jump()
    // {
    //     yield return null;
    //     Debug.Log("Jump");
    //     GoToNextState();

    // }

    public void TakeDamage(int damage)
    {
        if(!playerScript.hitMainEnemy && Time.time > elapsedInvulnerablTime)
        {
            Debug.Log("Got Hit");
            health -= damage;
            hpBar.fillAmount = health / totalHealth;
            cam.camShake();
            StartCoroutine(DamageFlashing());
            playerScript.hitMainEnemy = true;
            elapsedInvulnerablTime = Time.time + InvulnerableCD;
        }
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlashing()
    {
        for (int i = 0; i < flashRotations; i++)
        {
            sr2d.material.color = Color.red;
            yield return new WaitForSeconds(damagedFlashTime/2);

            sr2d.material.color = Color.white;
            yield return new WaitForSeconds(damagedFlashTime/2);

        }
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject effect = Instantiate(maceDeathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player")
        {
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage(damageDealt);
        }
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player")
        {
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage(damageDealt);
        }
    }
}
