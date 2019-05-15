using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawProjectileScript : MonoBehaviour
{
    public GameObject sawDeathEffect;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        Debug.Log("Had collision with saw");
        if (hitInfo.gameObject.tag == "Stage" || hitInfo.gameObject.tag == "Bounds")
        {
            Destroy(gameObject);

        } else if(hitInfo.gameObject.tag == "Player")
        {
            Debug.Log("Hit Player");
            PlayerScript player = hitInfo.GetComponent<PlayerScript>();
            player.takeDamage();
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
