using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject attackAreaRef;
    private Rigidbody2D rigidbody;
    private Animator animator;

    public float attackAreaSize = 0.14f;
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
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
    }

    void OnDrawGizmos()
    {
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
            animator.SetInteger("Transition", 4);
            Collider2D attackArea = Physics2D.OverlapCircle(attackAreaRef.transform.position, attackAreaSize);

            if (attackArea != null)
            {
                Debug.Log("Criou");
            }

        }
    }

    IEnumerator AfterAttack(float settedMoveSpeed)
    {
        yield return new WaitForSeconds(0.2f);
        moveSpeed = settedMoveSpeed;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount += 1;

            if (jumpCount == 1)
            {
                animator.SetInteger("Transition", 2);
            } else if (jumpCount == 2)
            {
                animator.SetInteger("Transition", 3);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            jumpCount = 0;
            animator.SetInteger("Transition", 0);
        }
    }
}
