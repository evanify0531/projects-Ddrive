using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackPoint : MonoBehaviour
{
    public int damage;
    public float pushingForce;
    public Transform enemy;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = enemy.position,
                pushForce = pushingForce
            };

            collision.gameObject.SendMessage("TakeDamage", dmg);
        }
    }
}
