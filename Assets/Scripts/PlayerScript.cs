using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 500f;
    public float groundCheckRadius = .5f;
    public int totalJumps;
    public Transform groundCheck;
    public Transform firePoint;
    public LayerMask whatIsGround;
    public GameObject firePrefab;

    private Rigidbody2D rb2d;
    private Animator anim;
    private bool isFacingRight = true;
    private bool isRunning;
    private bool wantsJump;
    private bool canJump;
    private bool isGrounded;
    private float moveDirection;
    private int jumpsLeft;
    private bool wantsDash;
    public float dashSpeed = 20f;
    private float dashTime;
    public float startDashTime;
    private bool isShooting;
    private bool wantsFire;
    public float fireAttackDelay = .25f;
    private bool isFiring;

    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        jumpsLeft = totalJumps;
    }

    void Update()
    {
        CheckInputs();
        CheckMovementDirection();
        CheckJump();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        CheckSurroundings();
        doMovement();
    }

    private void CheckInputs()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            wantsJump = true;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            wantsFire = true;
        }
        if(moveDirection!=0 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Wants Dash");
            wantsDash=true;
        }
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
        }
        else
        {
            isRunning = false;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckJump()
    {
        if (isGrounded && rb2d.velocity.y <= 0)
        {
            jumpsLeft = totalJumps;
        }

        if (jumpsLeft <= 0)
        {
            canJump = false;
        }
        else
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
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SNinja_Fire"))
        {
            rb2d.velocity = new Vector2(moveSpeed * moveDirection, rb2d.velocity.y);
            Jump();
            Shoot();
            Dash();
        } else {
            rb2d.velocity = Vector2.zero;
        }
    }

    private void Jump()
    {
        if (wantsJump && canJump)
        {
            rb2d.AddForce(Vector2.up * jumpPower);
            jumpsLeft--;
            wantsJump = false;
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

    private void Dash()
    {
        if(dashTime <= 0)
        {
            dashTime = startDashTime;
            rb2d.velocity = Vector2.zero;
            wantsDash = false;
        }
        if (wantsDash)
        {
            dashTime -= Time.deltaTime;
            rb2d.velocity = new Vector2(dashSpeed * moveDirection, rb2d.velocity.y);
            Debug.Log("doin dash");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
