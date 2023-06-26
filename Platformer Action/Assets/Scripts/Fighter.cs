using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{

    public Animator anim;
    public Rigidbody2D rb;

    // Player Taking Damage
    public int HP;
    public int maxHP;
    public bool isHit = false;


    protected float recoverTime = 0.3f;

    // Immunity
    public float immuneTime;
    protected float lastImmune;

    public Vector3 pushDirection;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        HP = maxHP;
        rb = GetComponent<Rigidbody2D>();
    }


    protected virtual void TakeDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            isHit = true;
            HP -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            //update on the visuals of hp
            //anim.SetBool("isHit", isHit);

            if (HP <= 0)
            {
                HP = 0;
                anim.SetBool("isDead", true);
                Death();
            }
        }

        
    }


    protected virtual void Death()
    {
        //implement dying.
        rb.bodyType = RigidbodyType2D.Static;
        this.GetComponent<BoxCollider2D>().enabled = false;
    }
}
