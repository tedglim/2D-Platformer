using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public GameObject player;
    public int damage = 50;
    public float fireSpeed = 20f;
    public float fireMaxDistance = 25.0f;
    public int fireHealth = 2;
    private int currentFireHealth;
    private float fireStartPos;

    void Awake()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.right * fireSpeed;
        fireStartPos = transform.position.x;
        currentFireHealth = fireHealth;
    }

    private void reduceFireHealth(int hpLoss)
    {
        currentFireHealth -= hpLoss;
        if (currentFireHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.tag == "Stage")
        {
            reduceFireHealth(fireHealth);
        } else if (hitInfo.gameObject.tag == "Enemy")
        {
            EnemyMaceScript enemy = hitInfo.GetComponent<EnemyMaceScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            reduceFireHealth(fireHealth);
        } else if (hitInfo.gameObject.tag == "EnemyDestructibles")
        {
            Debug.Log("HIT SPIKE");
            reduceFireHealth(1);
            SawProjectileScript saw = hitInfo.GetComponent<SawProjectileScript>();
            saw.GetDestroyed();
        }
    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Bounds")
        {
            reduceFireHealth(fireHealth);
        }
    }
}
