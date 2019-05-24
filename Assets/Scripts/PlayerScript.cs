using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Animator anim;
    private bool lockedActionOn;


    //Move & Flip
    private float moveDirection;
    private bool isFacingRight;
    private bool isRunning;
    public float moveSpeed = 650f;

    //Jump
    private bool canJump;
    private bool onStage;
    public Transform stageCheck;
    public LayerMask whatIsStage;
    public float stageCheckRadius = .56f;
    public float jumpPower = 79500f;
    public int totalJumps = 1;
    private int jumpsLeft;
    public float actOutOfJumpSpeed = 15.0f;

    //Dash
    private bool canDash;
    public float dashSpeed = 5000f;
    public float dashLoft = 7.0f;
    public float dashDuration = 0.1f;
    private float currentDashTime;
    public float dashCD = .25f;
    private float nextDashTime;
    public int totalAirDashes = 1;
    private int airDashesLeft;
    public GhostScript ghost;

    //Melee Attack
    private bool canMelee;
    public float meleeAttackDuration = .32f;
    private float currentMeleeAttackTime;
    public float meleeAttackCD = .32f;
    private float nextMeleeAttackTime;
    public int totalAirMeleeAttacks = 1;
    private int airMeleeAttacksLeft;
    public GameObject meleeHitBox;
    private Collider2D[] enemiesToDamage;
    public LayerMask whatAreEnemies;
    public float attackRangeX = 2.4f;
    public float attackRangeY = 2.5f;
    public bool hitMainEnemy;
    public int dmgToMainEnemy = 20;

    //Fire Attack
    // private bool canFire;
    // public float fireAttackDuration = 1.0f;
    // private float currentFireAttackTime;
    // public float fireAttackCD = 3.0f;
    // private float nextFireAttack;
    // public float shootFireDelay = .6f;
    // private bool shotFire;
    // public int totalAirFireAttacks = 1;
    // private int airFireAttacksLeft;
    // public Transform firePoint;
    // public GameObject firePrefab;

    //Health
    public float playerHealth = 20.0f;
    private float currentHealth;

    //Hurt animation
    private bool isHurt;
    public float damagedFlashTime = .4f;
    public float flashRotations = 2;
    private float nextInvincibleChance;
    public float invincibleCD = .8f;
    private SpriteRenderer sr2d;
    public Image hpBar;

    public GameObject maceEnemy;
    public float horizontalHurtForce = 1000.0f;
    public float verticalHurtForce = 2000.0f;
    private CameraScript cam;


    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        sr2d = GetComponent<SpriteRenderer>();

        //Move & Flip
        isFacingRight = true;
        isRunning = false;
        moveDirection = 0.0f;

        //Jump
        canJump = false;
        onStage = false;
        jumpsLeft = totalJumps;

        //Dash
        canDash = false;
        airDashesLeft = totalAirDashes;
        currentDashTime = dashDuration;
        nextDashTime = 0.0f;

        //Melee Attack
        canMelee = false;
        airMeleeAttacksLeft = totalAirMeleeAttacks;
        currentMeleeAttackTime = meleeAttackDuration;
        nextMeleeAttackTime = 0.0f;
        hitMainEnemy = false;

        //Fire Attack
        // canFire = false;
        // shotFire = false;
        // airFireAttacksLeft = totalAirFireAttacks;
        // currentFireAttackTime = fireAttackDuration;
        // nextFireAttack = 0.0f;

        //Player Health & Hurt
        currentHealth = playerHealth;
        nextInvincibleChance = 0.0f;
        isHurt = false;
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraScript>();

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
        // lockedActionOn = (canDash || canMelee || canFire);
        lockedActionOn = (canDash || canMelee);

        if (lockedActionOn)
        {
            return;
        }
        else
        {
            moveDirection = Input.GetAxisRaw("Horizontal");
            CheckMovementDirection();
            CheckJump();
            CheckDash();
            CheckMelee();
            // CheckFire();
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && moveDirection < 0)
        {
            Debug.Log("Flip Left");
            Flip();
        }
        else if (!isFacingRight && moveDirection > 0)
        {
            Debug.Log("Flip Right");
            Flip();
        }
        if (rb2d.velocity.x != 0 && onStage)
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
        if(!isHurt)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void CheckJump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (jumpsLeft > 0 && !lockedActionOn)
            {
                Debug.Log("Confirmed Jump Possible");
                canJump = true;
            }
            else
            {
                Debug.Log("Confirmed Jump NOT Possible");
                canJump = false;
            }
        }
    }

    private void CheckDash()
    {
        if (moveDirection != 0 && (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)))
        {
            if (Time.time > nextDashTime && rb2d.velocity.y <= actOutOfJumpSpeed && airDashesLeft > 0 && !lockedActionOn)
            {
                Debug.Log("Confirmed Dash Possible");
                canDash = true;
                nextDashTime = Time.time + dashCD;
            }
            else
            {
                Debug.Log("Confirmed Dash NOT Possible");
                canDash = false;
            }
        }
    }

    private void CheckMelee()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E)))
        {
            if (Time.time > nextMeleeAttackTime && rb2d.velocity.y <= actOutOfJumpSpeed && airMeleeAttacksLeft > 0 && !lockedActionOn)
            {
                Debug.Log("Confirmed Melee Attack Possible");
                canMelee = true;
                nextMeleeAttackTime = Time.time + meleeAttackCD;
            }
            else
            {
                Debug.Log("Confirmed Melee Attack NOT Possible");
                canMelee = false;
            }
        }
    }

    // private void CheckFire()
    // {
    //     if ((Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.Q)))
    //     {
    //         if (Time.time > nextFireAttack && rb2d.velocity.y <= actOutOfJumpSpeed && airFireAttacksLeft > 0 && !lockedActionOn)
    //         {
    //             Debug.Log("Confirmed Fire Attack Possible");
    //             canFire = true;
    //             nextFireAttack = Time.time + fireAttackCD;
    //         }
    //         else
    //         {
    //             Debug.Log("Confiremd Fire Attack NOT Possible");
    //             canFire = false;
    //         }
    //     }
    // }

    void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("onStage", onStage);
        anim.SetBool("isHurt", isHurt);
        anim.SetFloat("yVel", rb2d.velocity.y);
    }

    private void CheckSurroundings()
    {
        onStage = Physics2D.OverlapCircle(stageCheck.position, stageCheckRadius, whatIsStage);
        if (onStage)
        {
            airDashesLeft = totalAirDashes;
            jumpsLeft = totalJumps;
            airMeleeAttacksLeft = totalAirMeleeAttacks;
            // airFireAttacksLeft = totalAirFireAttacks;
        }
        enemiesToDamage = Physics2D.OverlapBoxAll(meleeHitBox.transform.position, new Vector2(attackRangeX, attackRangeY), 0, whatAreEnemies);
    }

    private void doMovement()
    {
        if (isHurt)
        {
            return;
        }
        else if (canDash)
        {
            Dash();
        }
        else if (canMelee)
        {
            MeleeAttack();
        }
        // else if (canFire)
        // {
        //     Fire();
        // }
        else if (canJump)
        {
            Jump();
        }
        else
        {
            rb2d.velocity = new Vector2(moveSpeed * moveDirection * Time.deltaTime, rb2d.velocity.y);
        }
    }

    private void Jump()
    {
        Debug.Log("Performed Jump");
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpPower * Time.deltaTime);
        jumpsLeft--;
        canJump = false;
        Debug.Log("Turned Off Jump Request");
    }

    private void Dash()
    {
        if (currentDashTime <= 0)
        {
            rb2d.velocity = Vector2.zero;
            ghost.makeGhost = false;
            canDash = false;
            airDashesLeft--;
            currentDashTime = dashDuration;
            Debug.Log("Turned Off Dash Request");
        }
        else
        {
            if (currentDashTime == dashDuration)
            {
                Debug.Log("Performed Dash");
            }
            rb2d.velocity = new Vector2(moveDirection * dashSpeed * Time.deltaTime, dashLoft);
            ghost.makeGhost = true;
            currentDashTime -= Time.deltaTime;
        }
    }

    private void MeleeAttack()
    {
        if (currentMeleeAttackTime <= 0)
        {
            hitMainEnemy = false;
            canMelee = false;
            airMeleeAttacksLeft--;
            currentMeleeAttackTime = meleeAttackDuration;
            Debug.Log("Turned off Melee Request");
        }
        else
        {
            if (currentMeleeAttackTime == meleeAttackDuration)
            {
                Debug.Log("Performed Melee Attack");
                anim.SetTrigger("Melee");
            }
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                Debug.Log("Detected object " + i);
                if (enemiesToDamage[i].gameObject.tag == "Enemy")
                {
                    if (!hitMainEnemy)
                    {
                        enemiesToDamage[i].GetComponent<EnemyMaceScript>().TakeDamage(dmgToMainEnemy);
                    }
                }
                // else if (enemiesToDamage[i].gameObject.tag == "EnemyDestructibles")
                // {
                //     Debug.Log("Hit Spike with Sword");
                //     enemiesToDamage[i].GetComponent<SawProjectileScript>().GetDestroyed();
                // }
            }
            rb2d.velocity = Vector2.zero;
            currentMeleeAttackTime -= Time.deltaTime;
        }
    }

    // private void Fire()
    // {
    //     if (currentFireAttackTime <= 0)
    //     {
    //         canFire = false;
    //         shotFire = false;
    //         airFireAttacksLeft--;
    //         currentFireAttackTime = fireAttackDuration;
    //         Debug.Log("Turned off Fire Request");
    //     } 
    //     else 
    //     {
    //         if (currentFireAttackTime == fireAttackDuration)
    //         {
    //             Debug.Log("Performed Fire Attack");
    //             anim.SetTrigger("Fire");
    //         }
    //         if (currentFireAttackTime <= shootFireDelay && !shotFire)
    //         {
    //             Instantiate(firePrefab, firePoint.position, firePoint.rotation);
    //             shotFire = true;
    //         }
    //         rb2d.velocity = Vector2.zero;
    //         currentFireAttackTime -= Time.deltaTime;
    //     }
    // }

    public void takeDamage(float damageTaken)
    {
        if (Time.time > nextInvincibleChance)
        {
            rb2d.velocity = Vector2.zero;
            cam.camShake();
            currentHealth -= damageTaken;
            hpBar.fillAmount = currentHealth / playerHealth;
            StartCoroutine(DamageFlashing());
            nextInvincibleChance = Time.time + invincibleCD;
            Debug.Log("Took damage from enemy");
        }
    }

    private IEnumerator DamageFlashing()
    {
        isHurt = true;
        for (int i = 0; i < flashRotations; i++)
        {
            sr2d.material.color = Color.red;
            yield return new WaitForSeconds(damagedFlashTime / 2);

            sr2d.material.color = Color.white;
            yield return new WaitForSeconds(damagedFlashTime / 2);
        }
        isHurt = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stageCheck.position, stageCheckRadius);
        Gizmos.DrawWireCube(meleeHitBox.transform.position, new Vector3(attackRangeX, attackRangeY, 1));
    }

}
