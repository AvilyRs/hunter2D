using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    public GameObject attackAreaRef;
    public GameObject feetAreaRef;
    private Animator animator;
    public LayerMask feetLayerToCheckCollider;

    private bool isAttacking = false;

    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    private float originalMoveSpeed = 0;

    public float attackAreaSize = 0.14f;
    public float feetAreaSizeX = 0.10f;
    public float feetAreaSizeY = 0.10f;
    private int jumpCount = 0;
    public float attackAnimationDuration = 0.2f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Jump();
        Attack();
    }

    void FixedUpdate()
    {
        Movement();
        CheckFeetCollision();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(feetAreaRef.transform.position, new Vector2(feetAreaSizeX, feetAreaSizeY));
        Gizmos.DrawWireSphere(attackAreaRef.transform.position, attackAreaSize);
    }

    void Movement()
    {
        float movement = Input.GetAxis("Horizontal");
        rigidbody.velocity = new Vector2(movement * moveSpeed, rigidbody.velocity.y);

        if (movement > 0 && jumpCount == 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            animator.SetInteger("Transition", 1);
        }

        if (movement < 0 && jumpCount == 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            animator.SetInteger("Transition", 1);
        }

        if (movement == 0 && jumpCount == 0)
        {
            animator.SetInteger("Transition", 0);
        }
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            originalMoveSpeed = moveSpeed;
            moveSpeed = 0;

            isAttacking = true;
            animator.SetInteger("Transition", 4);

            Collider2D attackArea = Physics2D.OverlapCircle(attackAreaRef.transform.position, attackAreaSize);

            if (attackArea != null)
            {
                Debug.Log("Criou");
            }

            StartCoroutine(AfterAttack());
        }
    }

    IEnumerator AfterAttack()
    {
        yield return new WaitForSeconds(0.03f);
        isAttacking = false;
        moveSpeed = originalMoveSpeed;

        if (jumpCount > 0)
        {
            animator.SetInteger("Transition", 1);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount += 1;

            if (!isAttacking)
            {
                if (jumpCount == 1)
                {
                    animator.SetInteger("Transition", 2);
                }

                if (jumpCount == 2)
                {
                    animator.SetInteger("Transition", 3);
                }
            }
        }
    }

    void CheckFeetCollision()
    {
        Collider2D feetCollider = Physics2D.OverlapBox(
            feetAreaRef.transform.position,
            new Vector2(feetAreaSizeX, feetAreaSizeY),
            transform.rotation.x, feetLayerToCheckCollider
        );

        if (feetCollider != null)
        {
            jumpCount = 0;
        }
    }
}
