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
        Patrol,
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
    public int totalJumps = 4;
    private int jumpsLeft;
    public GameObject player;
    public float moveSpeed = 1000.0f;
    public float jumpPower = 1000.0f;
    private Transform target;
    private float originalHeight;
    private Vector3 playerPos;
    private Vector3 myPos;
    private bool isJumping;
    
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
        jumpsLeft = totalJumps;
        originalHeight = transform.position.y;
        myPos = transform.position;
        playerPos = player.transform.position;
        isJumping = false;


        rb2d = GetComponent<Rigidbody2D>();
        sr2d = GetComponent<SpriteRenderer>();
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraScript>();
        health = totalHealth;
        elapsedInvulnerablTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
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
        } else if (currentState == BossState.Patrol)
        {
            if (currPatrolIntroTime <= 0){
                Patrol();
            } else {
                if (currPatrolIntroTime == patrolIntroDuration)
                {
                    anim.SetTrigger("isPatrolIntro");
                }
                currPatrolIntroTime -= Time.deltaTime;
                Debug.Log("Patrol Intro");
            }
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
        if (rb2d.transform.position.y <= originalHeight)
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;
            rb2d.transform.position = new Vector2(rb2d.transform.position.x, originalHeight);
            if (jumpsLeft <= 0)
            {
                currentState = BossState.Idle;
                jumpsLeft = totalJumps;
            } else 
            {
                myPos = rb2d.transform.position;
                playerPos = player.transform.position;
                rb2d.AddForce(Vector2.up * jumpPower * Time.deltaTime, ForceMode2D.Impulse);
                jumpsLeft--;
                anim.SetTrigger("Jump");
            }
        } else 
        {
            rb2d.gravityScale = 1;
            rb2d.velocity += Physics2D.gravity * Time.deltaTime;
            if (rb2d.velocity.y < 0)
            {
                rb2d.velocity += 2*Physics2D.gravity * Time.deltaTime;
            }
            if (playerPos.x <= myPos.x && myPos.x < patrolSpots[0].position.x && myPos.x > patrolSpots[1].position.x)
            {
                Debug.Log("Left");
                rb2d.velocity = new Vector2(-moveSpeed * Time.deltaTime, rb2d.velocity.y);
                if (transform.position.x < patrolSpots[1].position.x + 1.0f)
                {
                    Debug.Log("Too Far Left");
                    rb2d.velocity = new Vector2(10f * Time.deltaTime, rb2d.velocity.y);
                }
            } else if (playerPos.x > myPos.x && myPos.x < patrolSpots[0].position.x && myPos.x > patrolSpots[1].position.x)
            {
                Debug.Log("Right");
                rb2d.velocity = new Vector2(moveSpeed * Time.deltaTime, rb2d.velocity.y);
                 if (transform.position.x > patrolSpots[0].position.x - 1.0f)
                {
                    Debug.Log("Too Far Right");
                    rb2d.velocity = new Vector2(-10f * Time.deltaTime, rb2d.velocity.y);
                }
            }
        }
    }

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
        } else if (hitInfo.gameObject.tag == "Bounds")
        {
            rb2d.velocity = new Vector2 (0.0f, rb2d.velocity.y);
        }
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player")
        {
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage(damageDealt);
        } else if (hitInfo.gameObject.tag == "Bounds")
        {
            rb2d.velocity = new Vector2 (0.0f, rb2d.velocity.y);
        }
    }
}
