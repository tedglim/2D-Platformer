using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Animator anim;

    private float moveDirection;
    private bool isFacingRight;
    private bool isRunning;
    public float moveSpeed = 650f;

    private bool wantsDash;
    private float dashTime;
    public float startDashTime = 0.15f;
    public float dashSpeed = 3500f;
    public float dashLoft = 2.5f;
    public GhostScript ghost;
    public float dashCD = .6f;
    private float timeUntilDash;

    private bool wantsJump;
    private bool canJump;
    private bool onStage;
    public Transform stageCheck;
    public float stageCheckRadius = .5f;
    public LayerMask whatIsStage;
    private int jumpsLeft;
    public int totalJumps = 1;
    public float jumpPower = 79500f;

    private bool wantsFire;
    public Transform firePoint;
    public GameObject firePrefab;
    private float fireRelease;
    public float fireStart = .5f;
    private bool shotFire = false;
    public float shootFireTime = .25f;
    
    private bool wantsMelee;
    public Collider2D meleeHitBox;
    private float meleeCurrentTime;
    public float meleeAttackTime = .3f;



    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        isFacingRight = true;

        wantsDash = false;
        dashTime = startDashTime;
        timeUntilDash = 0;

        jumpsLeft = totalJumps;
        wantsJump = false;
        canJump = false;
        onStage = false;

        wantsFire = false;
        fireRelease = fireStart;

        wantsMelee = false;
        meleeCurrentTime = meleeAttackTime;
    }

    void Update()
    {
        CheckInputs();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        CheckSurroundings();
        doMovement();
    }

    private void CheckInputs()
    {
        if (!wantsDash)
        {
            moveDirection = Input.GetAxisRaw("Horizontal");
        }
        CheckMovementDirection();
        CheckDash();
        CheckJump();
        CheckFire();
        CheckMelee();
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && moveDirection < 0)
        {
            Flip();
        } else if (!isFacingRight && moveDirection > 0)
        {
            Flip();
        }
        if (rb2d.velocity.x != 0 && onStage)
        {
            isRunning = true;
        } else
        {
            isRunning = false;
        }
    }

    private void Flip()
    {
        if (!wantsFire && !wantsDash && !wantsMelee)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckDash()
    {
        if (moveDirection != 0 && Input.GetKeyDown(KeyCode.RightShift) && !wantsFire && !wantsDash && !wantsMelee)
        {
            if(Time.time > timeUntilDash)
            {
                wantsDash = true;
                timeUntilDash = Time.time + dashCD;
            }
        }
    }

    private void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && !wantsDash && !wantsFire && jumpsLeft > 0 && !wantsMelee)
        {
            wantsJump = true;
        }
        if (onStage && rb2d.velocity.y <= 0)
        {
            jumpsLeft = totalJumps;
        }
        if (jumpsLeft <= 0)
        {
            canJump = false;
        } else
        {
            canJump = true;
        }
    }

    private void CheckFire()
    {
        if (Input.GetButtonDown("Fire1") && !wantsDash && !wantsJump && rb2d.velocity.y == 0.0f && !wantsMelee)
        {
            wantsFire = true;
        }
    }

    private void CheckMelee()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !wantsDash && !wantsFire && rb2d.velocity.y <= 2.5f && !wantsMelee)
        {
            wantsMelee = true;
        }
    }

    void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", onStage);
        anim.SetFloat("yVel", rb2d.velocity.y);
    }

    private void CheckSurroundings()
    {
        onStage = Physics2D.OverlapCircle(stageCheck.position, stageCheckRadius, whatIsStage);
    }

    private void doMovement()
    {
        if (wantsDash)
        {
            Dash();
        } else if (wantsFire)
        {
            rb2d.velocity = Vector2.zero;
            Fire();
        } else if (wantsMelee)
        {
            rb2d.velocity = Vector2.zero;
            MeleeAttack();
        } else
        {
            rb2d.velocity = new Vector2(moveSpeed * moveDirection * Time.deltaTime, rb2d.velocity.y);
            Jump();
        }
    }

    private void Jump()
    {
        if (wantsJump && canJump)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
            rb2d.AddForce(Vector2.up * jumpPower * Time.deltaTime);
            jumpsLeft--;
            wantsJump = false;
        }
    }

    private void Dash()
    {
        if (dashTime <= 0)
        {
            dashTime = startDashTime;
            rb2d.velocity = Vector2.zero;
            ghost.makeGhost = false;
            wantsDash = false;
        } else
        {
            rb2d.velocity = new Vector2(moveDirection * dashSpeed * Time.deltaTime, dashLoft);
            ghost.makeGhost = true;
            dashTime -= Time.deltaTime;
        }
    }

    private void Fire()
    {
        if (fireRelease == fireStart)
        {
            shotFire = false;
            anim.SetTrigger("Fire");
            fireRelease -= Time.deltaTime;
        } else if (fireRelease < fireStart && fireRelease > 0)
        {
            fireRelease -= Time.deltaTime;
            if (fireRelease <= shootFireTime && shotFire == false)
            {
                Instantiate(firePrefab, firePoint.position, firePoint.rotation);
                shotFire = true;
            }
        } else if (fireRelease <= 0)
        {
            wantsFire = false;
            fireRelease = fireStart;
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Do attack");
        if (meleeCurrentTime <= 0)
        {
            meleeHitBox.enabled = false;
            wantsMelee = false;
            meleeCurrentTime = meleeAttackTime;
        } else if (meleeCurrentTime == meleeAttackTime)
        {
            meleeHitBox.enabled = true;
            anim.SetTrigger("Melee");
            meleeCurrentTime -= Time.deltaTime;
        } else
        {
            meleeCurrentTime -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stageCheck.position, stageCheckRadius);
    }

}
