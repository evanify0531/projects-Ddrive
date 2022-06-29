using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    public int damage;
    public float pushingForce;
    public Transform player;




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = player.position,
                pushForce = pushingForce
            };

            collision.gameObject.SendMessage("TakeDamage", dmg);
        }
    }
}
