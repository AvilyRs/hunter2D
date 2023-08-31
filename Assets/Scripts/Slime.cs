using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    public GameObject colliderAreaRef;
    public LayerMask layerToCheckCollider;

    private bool isReceivingDamage = false;

    public float colliderAreaSize;

    public float moveSpeed = 1;
    private float originalMoveSpeed = 0;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Movement();
        CollisionCheck();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(colliderAreaRef.transform.position, colliderAreaSize);
    }

    void Movement()
    {
        rigidbody.velocity = new Vector2(moveSpeed, rigidbody.velocity.y);
    }

    public override void OnReceiveDamage(int receivedDamage)
    {
        isReceivingDamage = true;

        health -= receivedDamage;

        if (health <= 0)
        {
            moveSpeed = 0;
            animator.SetTrigger("Die");
            Destroy(gameObject, 1f);
        } else
        {
            originalMoveSpeed = moveSpeed;
            moveSpeed = 0;

            animator.SetTrigger("Receive Damage");
        }

        StartCoroutine(AfterReceiveDamage());
    }

    IEnumerator AfterReceiveDamage()
    {
        yield return new WaitForSeconds(0.3f);
        if (health > 0)
        {
            moveSpeed = originalMoveSpeed;
        }
    }

    void CollisionCheck()
    {
        Collider2D collider = Physics2D.OverlapCircle(colliderAreaRef.transform.position, colliderAreaSize, layerToCheckCollider);

        if (collider != null)
        {
            if (transform.eulerAngles.y == 180)
            {
                transform.eulerAngles = new Vector2(0, 0);
            } else
            {
                transform.eulerAngles = new Vector2(0, 180);
            }

            moveSpeed = -moveSpeed;
        }
    }
}
