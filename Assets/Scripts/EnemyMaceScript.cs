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
        health -= damage;
        cam.camShake();
        StartCoroutine(DamageFlashing());
        // while (elapsedInvulnerablTime > 0)
        // {
        //     if(sr2d.color == Color.red && Time.time > InvulnerableInterval){
        //         Debug.Log("Make Me White");

        //         sr2d.material.color = Color.white;
        //         colorChangeTime = Time.time + InvulnerableInterval;
        //     } else if (sr2d.color == Color.white && Time.time > InvulnerableInterval)
        //     {
        //         Debug.Log("Make Me Red");
        //         sr2d.material.color = Color.red;  
        //         colorChangeTime = Time.time + InvulnerableInterval;
        //     }
        //     elapsedInvulnerablTime -= Time.deltaTime;
        // }
        // Debug.Log("End White");
        // sr2d.material.color = Color.white;
        // StartCoroutine(DamageFlashing());
        // sr2d.color = Color.red;
        hpBar.fillAmount = health / totalHealth;
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

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // if (hitInfo.gameObject.tag == "Stage")
        // {
        //     Debug.Log("Hit Stage");
        //     rb2d.velocity = Vector2.zero;
        // }
        // if (hitInfo.gameObject.tag == "Player")
        // {
        //     Debug.Log("Hit Player");
        //     rb2d.velocity = Vector2.zero;
        // }
        // if (hitInfo.gameObject.tag == "PlayerMelee")
        // {
        //     Debug.Log("Hit PlayerMelee");
        //     TakeDamage(damage);
        //     // rb2d.velocity = Vector2.zero;
        // }
    }
}
