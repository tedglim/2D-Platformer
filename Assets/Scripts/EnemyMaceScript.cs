using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMaceScript : MonoBehaviour
{
    enum BossState {
        Idle,
        Jump,
        COUNT
    }
    BossState currentState = BossState.Idle;

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
        GoToNextState();

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

    }

    void GoToNextState()
    {
        BossState nextState = (BossState)UnityEngine.Random.Range(0, (int)BossState.COUNT);
        string nextStateString = nextState.ToString();
        string lastStateString = currentState.ToString();
        currentState = nextState;
        StopCoroutine(lastStateString);
        StartCoroutine(nextStateString);
    }

    IEnumerator Idle()
    {
        yield return null;
        Debug.Log("Idle");
        GoToNextState();
    }

    IEnumerator Jump()
    {
        yield return null;
        Debug.Log("Jump");
        GoToNextState();

    }

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
