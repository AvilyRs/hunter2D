using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    public GameObject colliderAreaRef;
    public LayerMask layerToCheckCollider;

    public float colliderAreaSize;

    public float moveSpeed = 1;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
