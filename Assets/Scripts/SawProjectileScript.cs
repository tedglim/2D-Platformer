using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawProjectileScript : MonoBehaviour
{
    public GameObject sawDeathEffect;
    private Rigidbody2D rb2d;
    public float spikeSpeed = 20f;
    public float damageDealt = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.left * spikeSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        Debug.Log("Had collision with saw");
        if (hitInfo.gameObject.tag == "Stage")
        {
            Destroy(gameObject);

        } else if(hitInfo.gameObject.tag == "Player" || hitInfo.gameObject.tag == "PlayerMelee")
        {
            Debug.Log("Hit Player");
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage(damageDealt);
        }

    }

    void OnTriggerExit2D(Collider2D hitInfo)
    {
        if(hitInfo.gameObject.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }

    // void onCollisionEnter2D(Collision2D collision)
    // {
    //             if(collision.gameObject.tag == "PlayerDestructibles")
    //     {
    //         Debug.Log("SPIKE HIT");

    //         // collision.collider.gameObject.tag
    //     }
    // }

    public void GetDestroyed()
    {
        Destroy(gameObject);
        GameObject effect = Instantiate(sawDeathEffect, transform.position, Quaternion.identity);
        Destroy(effect, .25f);
    }
}
