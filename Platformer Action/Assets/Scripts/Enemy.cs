using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Rigidbody2D rb;
    public Animator anim;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;


        // play hurt animation
        anim.SetTrigger("Hurt");

        // Die if dead
        if (currentHealth <= 0)
        {
            anim.SetBool("IsDead", true);
            Death();
        }

    }

    private void Death()
    {
        rb.bodyType = RigidbodyType2D.Static;
        this.GetComponent<BoxCollider2D>().enabled = false;

    }


}
