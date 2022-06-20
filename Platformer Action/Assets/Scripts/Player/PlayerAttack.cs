using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //public Animator anim;
    public bool isAttacking = false;
    private float attackCooldown = 0.5f;
    private float lastAttack = -0.5f;




    public PlayerController player;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;


    private void Start()
    {
        //anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && Time.time - lastAttack > attackCooldown && player.isGrounded && !player.isDashing && !player.isDashAttacking)
        {
            isAttacking = true;
            lastAttack = Time.time;
            //Attack();
        }


        // The case where attack is finished 
        else if (Time.time - lastAttack > 0.5f)
            isAttacking = false;


    }


    private void Attack()
    {
        
        // play attack animation
        //anim.SetBool("isAttacking", isAttacking);

        /*
        // detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // apply damage to detected enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
        */

    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
