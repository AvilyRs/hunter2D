using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    private new Rigidbody2D rigidbody;
    public GameObject raycastPointRef;
    public GameObject raycastBehindRef;
    public Vector2 raycastDirection;
    public Vector2 raycastBehindDirection;
    private Collider2D attackArea;
    private Animator animator;

    public GameObject attackAreaRef;
    public float attackAreaSize;

    private bool isAlive = true;
    private bool isRage = false;
    private bool isAttackEnabled = true;
    public bool isVisionToRight = true;
    public float visionArea = 3f;
    public float moveSpeed = 1f;
    private float originalMoveSpeed = 0;

    private float distanceBetweenEnemyAndTargetObject;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        SetDirection();
        Animate();

        if (isAttackEnabled && distanceBetweenEnemyAndTargetObject <= 0.7f && isAlive)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        DetectVision();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastPointRef.transform.position, raycastDirection * visionArea);
        Gizmos.DrawRay(raycastBehindRef.transform.position, raycastBehindDirection * visionArea);
        Gizmos.DrawWireSphere(attackAreaRef.transform.position, attackAreaSize);
    }

    void Animate()
    {
        if (isRage && distanceBetweenEnemyAndTargetObject > 0.7f)
        {
            animator.SetBool("walking", true);
        } else
        {
            animator.SetBool("walking", false);
        }
    }

    void SetDirection()
    {
        if (isVisionToRight)
        {
            transform.eulerAngles = new Vector2(0, 0);
            raycastDirection = Vector2.right;
            raycastBehindDirection = Vector2.left / 2;
        } else
        {
            transform.eulerAngles = new Vector2(0, 180);
            raycastDirection = Vector2.left;
            raycastBehindDirection = Vector2.right / 2;
        }
    }

    void MoveClose(RaycastHit2D target)
    {
        if (isRage)
        {
            distanceBetweenEnemyAndTargetObject = Vector2.Distance(transform.position, target.transform.position);

            if (distanceBetweenEnemyAndTargetObject <= 0.7f)
            {
                rigidbody.velocity = Vector2.zero;
            }
            else
            {
                if (isVisionToRight)
                {
                    rigidbody.velocity = new Vector2(moveSpeed, rigidbody.velocity.y);
                }
                else
                {
                    rigidbody.velocity = new Vector2(-moveSpeed, rigidbody.velocity.y);
                }
            }
        }
    }

    void DetectVision()
    {
        RaycastHit2D collidedObject = Physics2D.Raycast(raycastPointRef.transform.position, raycastDirection, visionArea);
        RaycastHit2D behindCollidedObject = Physics2D.Raycast(raycastBehindRef.transform.position, raycastBehindDirection, visionArea);


        if (collidedObject.collider != null && collidedObject.collider.gameObject.CompareTag("Player"))
        {
            isRage = true;
            MoveClose(collidedObject);
        } else
        {
            isRage = false;
            rigidbody.velocity = Vector2.zero;
        }

        if (behindCollidedObject.collider != null && behindCollidedObject.collider.gameObject.CompareTag("Player"))
        {
            isVisionToRight = !isVisionToRight;
        }
    }

    void Attack()
    {
        Collider2D atkArea = Physics2D.OverlapCircle(attackAreaRef.transform.position, attackAreaSize);
        
        if (atkArea != null)
        {
            if (atkArea.gameObject.CompareTag("Player") && isAttackEnabled)
            {
                Player player = atkArea.gameObject.GetComponent<Player>();

                player.OnReceiveDamage(damage);

                animator.SetTrigger("attack");
                isAttackEnabled = false;
                StartCoroutine(AfterAttack());
            }
        }
    }

    IEnumerator AfterAttack()
    {
        yield return new WaitForSeconds(1f);
        Destroy(attackArea);
        isAttackEnabled = true;
    }

    public override void OnReceiveDamage(int receivedDamage)
    {
        health -= receivedDamage;

        if (health <= 0)
        {
            isAlive = false;
            moveSpeed = 0;
            animator.SetBool("Die", true);
            Destroy(gameObject, 1f);
        }
        else
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
}
