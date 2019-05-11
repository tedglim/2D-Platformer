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
    public float moveSpeed = 500f;

    private bool wantsDash;
    private float dashTime;
    public float startDashTime = 0.25f;
    public float dashSpeed = 2000;
    public GhostScript ghost;

    private bool wantsJump;
    private bool canJump;
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    private int jumpsLeft;
    public int totalJumps = 1;
    public float jumpPower = 5000f;


    public float groundCheckRadius = .5f;


    public Transform firePoint;

    public GameObject firePrefab;



    private float prevDirection;


    private bool isShooting;
    private bool wantsFire;
    public float fireAttackDelay = .25f;
    private bool isFiring;
    private bool wantsDashRight;
    private bool wantsDashLeft;
    public float dashAttackDelay = .25f;



    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        isFacingRight = true;
        wantsDash = false;
        dashTime = startDashTime;
        jumpsLeft = totalJumps;
        wantsJump = false;
        canJump = false;
        isGrounded = false;
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
        if(!wantsDash)
        {
            moveDirection = Input.GetAxisRaw("Horizontal");
        }
        CheckMovementDirection();
        CheckDash();
        CheckJump();
        // if (Input.GetButtonDown("Fire1"))
        // {
        //     wantsFire = true;
        // }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && moveDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && moveDirection > 0)
        {
            Flip();
        }
        if (rb2d.velocity.x != 0)
        {
            isRunning = true;
        } else
        {
            isRunning = false;
        }
    }

    private void Flip()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SNinja_DashAttack"))
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckDash()
    {
        if(moveDirection != 0 && Input.GetKeyDown(KeyCode.RightShift))
        {
            wantsDash = true;
        }
    }

    private void CheckJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            wantsJump = true;
        }
        if (isGrounded && rb2d.velocity.y <= 0)
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

    void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVel", rb2d.velocity.y);
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void doMovement()
    {
        if(!wantsDash)
        {
            rb2d.velocity = new Vector2(moveSpeed * moveDirection * Time.deltaTime, rb2d.velocity.y);
            Jump();
        } else
        {
            Dash();
        }

        // if(anim.GetCurrentAnimatorStateInfo(0).IsName("SNinja_DashAttack"))
        // {
        //     Dash();
        // } else {
        // rb2d.velocity = new Vector2(moveSpeed * moveDirection * Time.deltaTime, rb2d.velocity.y);
        //     Dash();
        // }
        // if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SNinja_Fire"))
        // {
        //     rb2d.velocity = new Vector2(moveSpeed * moveDirection, rb2d.velocity.y);
        //     Jump();
        //     Shoot();
        //     Dash();
        // } else {
        //     rb2d.velocity = Vector2.zero;
        // }
    }

    private void Jump()
    {
        if (wantsJump && canJump)
        {
            rb2d.AddForce(Vector2.up * jumpPower * Time.deltaTime);
            jumpsLeft--;
            wantsJump = false;
        }
    }

    private void Dash()
    {
        if(wantsDash)
        {
            if(dashTime <= 0)
            {
                dashTime = startDashTime;
                rb2d.velocity = Vector2.zero;
                ghost.makeGhost = false;
                wantsDash = false;
            } else
            {
                rb2d.velocity = new Vector2(moveDirection * dashSpeed * Time.deltaTime, 0.0f);
                ghost.makeGhost = true;
                dashTime -= Time.deltaTime;
            }
        }
    }

    private void Shoot()
    {
        if(wantsFire && isGrounded)
        {
            StartCoroutine(ShootDelay());
        }
    }

    public IEnumerator ShootDelay()
    {
        anim.SetTrigger("Fire");
        wantsFire=false;
        yield return new WaitForSeconds(fireAttackDelay);
        Instantiate(firePrefab, firePoint.position, firePoint.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
