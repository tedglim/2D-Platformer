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
    private bool wantsJump;
    private bool canJump;
    private bool onStage;
    public Transform stageCheck;
    public float stageCheckRadius = .5f;
    public LayerMask whatIsStage;
    public int totalJumps = 1;
    private int jumpsLeft;
    public float jumpPower = 79500f;
    public float actOutOfJumpSpeed = 25.0f;


    //Dash
    private bool wantsDash;
    private bool canDash;
    public float dashDuration = 0.1f;
    private float currentDashTime;
    public float dashSpeed = 5000f;
    public float dashLoft = 7.0f;
    public float dashCD = .25f;
    private float nextDashTime;
    public int totalAirDashes = 1;
    private int airDashesLeft;
    public GhostScript ghost;

    //Melee Attack
    private bool wantsMelee;
    private bool canMeleeAttack;
    public float meleeAttackDuration = .26f;
    private float currentMeleeAttackTime;
    public float meleeAttackCD = .28f;
    private float nextMeleeAttackTime;
    public int totalAirMeleeAttacks = 1;
    private int airMeleeAttacksLeft;
    public GameObject meleeHitBox;
    private Collider2D[] enemiesToDamage;
    public float attackRangeX = 2.4f;
    public float attackRangeY = 2.5f;
    public LayerMask whatAreEnemies;
    public bool hitOnce;

    //Fire Attack
    private bool wantsFire;
    private bool canFire;
    public float fireAttackDuration = .5f;
    private float currentFireAttackTime;
    public float fireAttackCD = .6f;
    private float nextFireAttack;
    public float shootFireDelay = .25f;
    private bool shotFire;
    public int totalAirFireAttacks = 1;
    private int airFireAttacksLeft;
    public Transform firePoint;
    public GameObject firePrefab;


    public int damage = 20;
    public float playerHealth = 20.0f;
    private float currentHealth;
    public Image hpBar;
    private float invincibleDuration;
    public float invincibleCD = 1f;
    private SpriteRenderer sr2d;
    private bool isHurt;

    // Use this for initialization
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();

        //Move & Flip
        isFacingRight = true;
        isRunning = false;
        moveDirection = 0.0f;

        //Jump
        wantsJump = false;
        canJump = false;
        onStage = false;
        jumpsLeft = totalJumps;

        //Dash
        wantsDash = false;
        canDash = false;
        airDashesLeft = totalAirDashes;
        currentDashTime = dashDuration;
        nextDashTime = 0.0f;

        //Melee Attack
        wantsMelee = false;
        canMeleeAttack = false;
        airMeleeAttacksLeft = totalAirMeleeAttacks;
        currentMeleeAttackTime = meleeAttackDuration;
        nextMeleeAttackTime = 0.0f;
        hitOnce = false;

        //Fire Attack
        wantsFire = false;
        canFire = false;
        shotFire = false;
        airFireAttacksLeft = totalAirFireAttacks;
        currentFireAttackTime = fireAttackDuration;
        nextFireAttack = 0.0f;

        currentHealth = playerHealth;
        sr2d = GetComponent<SpriteRenderer>();
        isHurt = false;
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
        lockedActionOn = (wantsDash && canDash) || (wantsMelee && canMeleeAttack) || (wantsFire && canFire);
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
            CheckFire();
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
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckJump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("Requested Jump");
            wantsJump = true;
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
        if (!isHurt && moveDirection != 0 && (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)))
        // if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))

        {
            Debug.Log("Requested Dash");
            wantsDash = true;
            if (Time.time > nextDashTime && airDashesLeft > 0 && rb2d.velocity.y <= actOutOfJumpSpeed && !lockedActionOn)
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
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E)) && !isHurt)
        {
            Debug.Log("Requested Melee Attack");
            wantsMelee = true;
            if (Time.time > nextMeleeAttackTime && airMeleeAttacksLeft > 0 && rb2d.velocity.y <= actOutOfJumpSpeed && !lockedActionOn)
            {
                Debug.Log("Confirmed Melee Attack Possible");
                canMeleeAttack = true;
                nextMeleeAttackTime = Time.time + meleeAttackCD;
            }
            else
            {
                Debug.Log("Confirmed Melee Attack NOT Possible");
                canMeleeAttack = false;
            }
        }
    }

    private void CheckFire()
    {
        if ((Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.Q)) && !isHurt)
        {
            Debug.Log("Requested Fire Attack");
            wantsFire = true;
            if (Time.time > nextFireAttack && airFireAttacksLeft > 0 && rb2d.velocity.y <= actOutOfJumpSpeed && !lockedActionOn)
            {
                Debug.Log("Confirmed Fire Attack Possible");
                canFire = true;
                nextFireAttack = Time.time + fireAttackCD;
            }
            else
            {
                Debug.Log("Confiremd Fire Attack NOT Possible");
                canFire = false;
            }
        }
    }

    void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("onStage", onStage);
        anim.SetFloat("yVel", rb2d.velocity.y);
    }

    private void CheckSurroundings()
    {
        onStage = Physics2D.OverlapCircle(stageCheck.position, stageCheckRadius, whatIsStage);
        if (onStage)
        {
            // Debug.Log("1 Available Air Dash");
            airDashesLeft = totalAirDashes;
            // Debug.Log("1 Available Jump");
            jumpsLeft = totalJumps;
            // Debug.Log("1 Available Air Melee Attack");
            airMeleeAttacksLeft = totalAirMeleeAttacks;
            // Debug.Log("1 Available Air Fire Attack");
            airFireAttacksLeft = totalAirFireAttacks;
        }
        enemiesToDamage = Physics2D.OverlapBoxAll(meleeHitBox.transform.position, new Vector2(attackRangeX, attackRangeY), 0, whatAreEnemies);
    }

    private void doMovement()
    {
        if (wantsDash && canDash)
        {
            Dash();
        }
        else if (wantsMelee && canMeleeAttack)
        {
            MeleeAttack();
        }
        else if (wantsFire && canFire)
        {
            Fire();
        }
        else
        {
            if (wantsJump && canJump)
            {
                Jump();
            }
            if(!isHurt)
            {
                rb2d.velocity = new Vector2(moveSpeed * moveDirection * Time.deltaTime, rb2d.velocity.y);

            }
        }
    }

    private void Jump()
    {
        Debug.Log("Performed Jump");
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpPower * Time.deltaTime);
        jumpsLeft--;
        wantsJump = false;
        Debug.Log("Turned Off Jump Request");
    }

    private void Dash()
    {
        if (currentDashTime <= 0)
        {
            rb2d.velocity = Vector2.zero;
            ghost.makeGhost = false;
            wantsDash = false;
            airDashesLeft--;
            currentDashTime = dashDuration;
            Debug.Log("Turned Off Dash Request");
        }
        else
        {

            rb2d.velocity = new Vector2(moveDirection * dashSpeed * Time.deltaTime, dashLoft);
            ghost.makeGhost = true;
            currentDashTime -= Time.deltaTime;
        }
    }

    private void MeleeAttack()
    {
        if (currentMeleeAttackTime <= 0)
        {
            hitOnce = false;
            wantsMelee = false;
            airMeleeAttacksLeft--;
            currentMeleeAttackTime = meleeAttackDuration;
            Debug.Log("Turned off Melee Request");
        }
        else if (currentMeleeAttackTime <= meleeAttackDuration)
        {
            if (currentMeleeAttackTime == meleeAttackDuration)
            {
                anim.SetTrigger("Melee");
                Debug.Log("Performed Melee Attack");
            }
            rb2d.velocity = Vector2.zero;
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                Debug.Log("Detected object");
                if (enemiesToDamage[i].gameObject.tag == "Enemy")
                {
                    if(!hitOnce)
                    {
                        enemiesToDamage[i].GetComponent<EnemyMaceScript>().TakeDamage(damage);
                    }
                }
                else if (enemiesToDamage[i].gameObject.tag == "EnemyDestructibles")
                {
                    Debug.Log("Hit Spike with Sword");
                    enemiesToDamage[i].GetComponent<SawProjectileScript>().GetDestroyed();
                }
            }
            currentMeleeAttackTime -= Time.deltaTime;
        }
    }

    private void Fire()
    {
        if (currentFireAttackTime <= 0)
        {
            wantsFire = false;
            airFireAttacksLeft--;
            currentFireAttackTime = fireAttackDuration;
            Debug.Log("Turned off Fire Request");

        }
        else if (currentFireAttackTime == fireAttackDuration)
        {
            Debug.Log("Performed Fire Attack");
            rb2d.velocity = Vector2.zero;
            shotFire = false;
            anim.SetTrigger("Fire");
            currentFireAttackTime -= Time.deltaTime;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            currentFireAttackTime -= Time.deltaTime;
            if (currentFireAttackTime <= shootFireDelay && shotFire == false)
            {
                Instantiate(firePrefab, firePoint.position, firePoint.rotation);
                shotFire = true;
            }
        }
    }

    public void takeDamage()
    {
        if (Time.time > invincibleDuration)
        {
            anim.SetTrigger("Hurt");
            currentHealth -= 1.0f;
            rb2d.AddForce(Vector2.up * 2000f * Time.deltaTime, ForceMode2D.Impulse);
            rb2d.AddForce(Vector2.left * 1000f * Time.deltaTime, ForceMode2D.Impulse);
            hpBar.fillAmount = currentHealth / playerHealth;
            StartCoroutine(DamageFlashing());
            invincibleDuration = Time.time + invincibleCD;
            Debug.Log("Took damage from enemy");
        }
    }

    private IEnumerator DamageFlashing()
    {
        isHurt = true;
        for (int i = 0; i < 2; i++)
        {
            sr2d.material.color = Color.red;
            yield return new WaitForSeconds(.25f/2);

            sr2d.material.color = Color.white;
            yield return new WaitForSeconds(.25f/2);
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
