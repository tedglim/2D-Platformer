using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    private float ghostDelaySeconds;
    public bool makeGhost;
    public float ghostDelay = 0.01f;
    public float ghostDestroyTime = 1.0f;
    public GameObject ghost;


    // Start is called before the first frame update
    void Start()
    {
        makeGhost = false;
        ghostDelaySeconds = ghostDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(makeGhost)
        {
            if(ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            } else 
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, ghostDestroyTime);
            }
        }
    }

}
