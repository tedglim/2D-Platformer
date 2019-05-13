using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaceScript : MonoBehaviour
{
    public int health = 100;
    public float speed = 20f;
    public float delay = 1f;
    public Transform target;
    private float posY;
    private Rigidbody2D rb2d;
    private float timeLeft;
    private bool playerFound;
    private float offsetX = 2f;

    public Transform PlayerCheck;
    public float PlayerCheckX;
    public float PlayerCheckY;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        posY = transform.position.y;
        timeLeft = delay;
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

    public void TakeDamage (int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Stage")
        {
            Debug.Log("Hit Stage");
            rb2d.velocity = Vector2.zero;
        }
        if(hitInfo.gameObject.tag == "Player")
        {
            Debug.Log("Hit Player");
            rb2d.velocity = Vector2.zero;
        }
    }
}
