using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Fighter
{

    protected override void TakeDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            isHit = true;
            HP -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            //update on the visuals of hp
            

            if (HP <= 0)
            {
                HP = 0;
                Death();
            }
        }
    }

    protected override void Death()
    {
        //implement dying.
        //rb.bodyType = RigidbodyType2D.Static;
        this.GetComponent<BoxCollider2D>().enabled = false;
        // Add destroying game object later (or is it already implemented at the end of death animation?
    }



}
