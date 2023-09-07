using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private new CapsuleCollider2D collider;
    public GameObject attackArea;
    public GameObject feetAreaRef;
    private Animator animator;
    public LayerMask feetLayerToCheckCollider;

    private bool isAttacking = false;

    public int health = 5;
    public int damage = 1;

    public bool isGrounded = true;
    public int jumpsToUse = 1;
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    private float originalMoveSpeed = 0;

    public float attackAreaSize = 0.14f;
    public float feetAreaSizeX = 0.10f;
    public float feetAreaSizeY = 0.10f;
    public float attackAnimationDuration = 0.2f;

    enum TransitionsType
    {
        Idle = 0,
        Running = 1
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        originalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        Attack();
    }

    void FixedUpdate()
    {
        CheckFeetCollision();
        Movement();
    }

    private void LateUpdate()
    {
        Jump();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(feetAreaRef.transform.position, new Vector2(feetAreaSizeX, feetAreaSizeY));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack Area Enemy"))
        {
            AttackArea attackFromEnemy = collision.gameObject.GetComponent<AttackArea>();
            OnReceiveDamage(attackFromEnemy.damage);
        }
    }

    void Movement()
    {
        if (!isAttacking)
        {
            float movement = Input.GetAxis("Horizontal");
            rigidbody.velocity = new Vector2(movement * moveSpeed, rigidbody.velocity.y);

            if (movement > 0)
            {
                transform.eulerAngles = new Vector2(0, 0);
                animator.SetInteger("Running | Idle", (int) TransitionsType.Running);
            }

            if (movement < 0)
            {
                transform.eulerAngles = new Vector2(0, 180);
                animator.SetInteger("Running | Idle", (int )TransitionsType.Running);
            }

            if (movement == 0)
            {
                animator.SetInteger("Running | Idle", (int) TransitionsType.Idle);
            }
        }
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                attackArea.SetActive(true);
                rigidbody.velocity = Vector2.zero;
                animator.SetTrigger("Attack");

                if (attackArea.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemy = attackArea.gameObject.GetComponent<Enemy>();
                    enemy.OnReceiveDamage(damage);
                }

                Invoke(nameof(AfterAttack), 1f);
            }
        }
    }

    private void AfterAttack()
    {
        isAttacking = false;
        attackArea.SetActive(false);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rigidbody.velocity = Vector2.up * jumpForce;
                animator.SetTrigger("Jump");
            } else if (jumpsToUse > 0)
            {
                rigidbody.velocity = Vector2.up * jumpForce;
                animator.SetTrigger("Jump");

                jumpsToUse -= 1;
            }
        }
    }

    void CheckFeetCollision()
    {
        Collider2D feetCollider = Physics2D.OverlapBox(
            feetAreaRef.transform.position,
            new Vector2(feetAreaSizeX, feetAreaSizeY),
            transform.rotation.x, feetLayerToCheckCollider // Here is verifying the collision feet with layer 8 (ground) selected from inspector
        );

        if (feetCollider != null)
        {
            isGrounded = true;
            jumpsToUse = 1;
        } else
        {
            isGrounded = false;
        }
    }

    public void OnReceiveDamage(int receivedDamage)
    {
        health -= receivedDamage;
        animator.SetTrigger("Receive Damage");

        if (health <= 0)
        {
            moveSpeed = 0;
            collider.isTrigger = true;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            animator.SetBool("Die", true);
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
