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
        Patrol,
        Jump,
        COUNT
    }
    BossState currentState;
    public Animator anim;
    public float IntroDuration = 3f;
    private float currIntroTime;

    public float patrolIntroDuration = 2.0f; 
    private float currPatrolIntroTime;
    private int farthestIdx;
    private float farthest;
    public float patrolSpeed = 10.0f;
    public float patrolDuration = 10.0f;
    private float currPatrolTime;
    private bool isPatrolling;
    public Transform[] patrolSpots;
    public float offset = 1.0f;
    private Transform currentDestination;



    private bool isJumping;


    private Rigidbody2D rb2d;
    private SpriteRenderer sr2d;
    public GameObject eyeR;
    public GameObject eyeL;
    public PlayerScript playerScript;
    private CameraScript cam;

    public Image hpBar;
    public float totalHealth = 100.0f;
    private float health;
    public float damagedFlashTime = .5f;
    public float flashRotations = 2;
    public float InvulnerableCD = 1.0f;
    private float elapsedInvulnerablTime;
    public GameObject maceDeathEffect;


    public float damageDealt = 5f;
    private bool isPatrolIntro;


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

        currIntroTime = IntroDuration;

        currPatrolIntroTime = patrolIntroDuration;
        isPatrolIntro = false;

        farthestIdx = 0;
        farthest = 0.0f;
        currPatrolTime = patrolDuration;
        currentDestination = patrolSpots[0];

        isPatrolling = false;

        isJumping = false;

        rb2d = GetComponent<Rigidbody2D>();
        sr2d = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraScript>();
        health = totalHealth;
        elapsedInvulnerablTime = 0.0f;


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

    private void CheckPlayerFound()
    {
        // // Physics2D.OverlapBox()
        // if (target.position.x < transform.position.x + offsetX && target.position.x > transform.position.x)
        // {
        //     // Debug.Log("player found");
        //     playerFound = true;
        // }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(PlayerCheck.position, new Vector3(PlayerCheckX, Player,0));
    // }

    void FixedUpdate()
    {
        if (currentState == BossState.Intro && Time.time <= currIntroTime)
        {
            Debug.Log("Intro");
            // sr2d.material.color = Color.blue;
            return;
        }
        else 
        {
            if (!isPatrolling && !isJumping)
            {
                currentState = (BossState)UnityEngine.Random.Range(1, (int)BossState.COUNT);
                
                //reset patrol times
                currPatrolIntroTime = patrolIntroDuration;
                currPatrolTime = patrolDuration;
                isPatrolIntro = false;

            }
            if (currentState == BossState.Patrol)
            {
                isPatrolling = true;
                // Debug.Log("Patrol");
                Patrol();
                // sr2d.material.color = Color.green;
            } else if (currentState == BossState.Jump)
            {
                Debug.Log("Jump");
                sr2d.material.color = Color.yellow;
            }
        }
    }

    private void Patrol()
    {
        if (currPatrolIntroTime > 0)
        {
            if (!isPatrolIntro)
            {
                anim.SetTrigger("isPatrolIntro");
                isPatrolIntro = true;
            }
            Debug.Log("Patrol Intro");
            currPatrolIntroTime -= Time.deltaTime;
        } 
        else 
        {
            if(currPatrolTime == patrolDuration)
            {
                anim.SetTrigger("isPatrolling");
            } else if (currPatrolTime <= 0 && Vector2.Distance(transform.position, currentDestination.position) < offset)
            {
                isPatrolling = false;
                return;
            }
            while(Vector2.Distance(transform.position, currentDestination.position) < offset)
            {
                currentDestination = patrolSpots[UnityEngine.Random.Range(0, patrolSpots.Length)];
            }
            transform.position = Vector2.MoveTowards(transform.position, currentDestination.position, patrolSpeed * Time.deltaTime);
            currPatrolTime -= Time.deltaTime;

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
        }
    }

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
            // eyeR.SetActive(true);
            // eyeL.SetActive(true);
            sr2d.material.color = Color.red;
            yield return new WaitForSeconds(damagedFlashTime/2);

            // eyeR.SetActive(false);
            // eyeL.SetActive(false);
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
