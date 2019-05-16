using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMaceScript : MonoBehaviour
{
    public float totalHealth = 100.0f;
    private float health;
    public float speed = 20f;
    public float delay = 1f;
    public Transform target;
    private float posY;
    private Rigidbody2D rb2d;
    private float timeLeft;
    private bool playerFound;
    private float offsetX = 2f;

    public Image hpBar;
    public Transform PlayerCheck;
    public float PlayerCheckX;
    public float PlayerCheckY;
    private SpriteRenderer sr2d;
    private float elapsedInvulnerablTime;
    public GameObject maceDeathEffect;
    public float InvulnerableDuration = 2.0f;
    private float colorChangeTime;
    public GameObject eyeR;
    public GameObject eyeL;
    private float InvulnerableColorRotation = .5f; 

    public float InvulnerableInterval = .1f;
    private CameraScript cam;
    public float sawAttackCD = 1f;
    private float currentSawAttack;
    public Transform sawAttackPos1;
    public GameObject sawPrefab;
    public GameObject sawPrefabIndestructible;
    public GameObject[] spikePrefabs;
    public PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        posY = transform.position.y;
        timeLeft = delay;
        health = totalHealth;
        sr2d = GetComponent<SpriteRenderer>();
        elapsedInvulnerablTime = InvulnerableDuration;
        eyeR.SetActive(false);
        eyeL.SetActive(false);
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraScript>();
        currentSawAttack = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // CheckPlayerFound();
    }

    private void CheckPlayerFound()
    {
        // Physics2D.OverlapBox()
        if (target.position.x < transform.position.x + offsetX && target.position.x > transform.position.x)
        {
            // Debug.Log("player found");
            playerFound = true;
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireCube(PlayerCheck.position, new Vector3(PlayerCheckX, Player,0));
    // }

    void FixedUpdate()
    {
        if (Time.time > currentSawAttack)
        {
            Instantiate(sawPrefab, sawAttackPos1.transform.position, Quaternion.identity);
            currentSawAttack = Time.time + sawAttackCD;
        }
        // if(!playerFound)
        // {
        //     transform.position = Vector2.MoveTowards(transform.position, new Vector2 (target.position.x, transform.position.y), speed * Time.deltaTime);
        // } else if(playerFound)
        // {
        //     rb2d.MovePosition((Vector2)transform.position + Vector2.down * speed * Time.deltaTime);
        //     //return up
        // }
    }

    public void TakeDamage(int damage)
    {
        if(!playerScript.hitOnce)
        {
        health -= damage;
        cam.camShake();
        StartCoroutine(DamageFlashing());
        hpBar.fillAmount = health / totalHealth;
        playerScript.hitOnce = true;

        }
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlashing()
    {
        eyeR.SetActive(true);
        eyeL.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            sr2d.material.color = Color.red;
            yield return new WaitForSeconds(InvulnerableColorRotation/2);

            sr2d.material.color = Color.white;
            yield return new WaitForSeconds(InvulnerableColorRotation/2);

        }
        eyeR.SetActive(false);
        eyeL.SetActive(false);
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
            player.takeDamage();
        }
    }

    void OnTriggerStay2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Player")
        {
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage();
        }
    }
}
