using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public int damage = 1;
    public bool damageOnCollision = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack Area"))
        {
            int receivedDamage = collision.gameObject.GetComponent<AttackArea>().damage;
            OnReceiveDamage(receivedDamage);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player player = collision.gameObject.GetComponent<Player>();
                player.OnReceiveDamage(damage);
            }
        }
    }

    // Method used to be overrided by the class that will extend.
    // Have to be created because this method is used here
    public virtual void OnReceiveDamage(int receivedDamage) {}
}
