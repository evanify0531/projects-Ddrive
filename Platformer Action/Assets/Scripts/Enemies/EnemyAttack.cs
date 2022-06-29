using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool attackCalled = false;
    public bool isAttacking = false;
    //private float attackCooldown = 1f;
    private float lastAttack = -0.5f;

    public Animator anim;

    public Transform enemyAttackPoint;
    public float attackRange = 0.5f;
    //public LayerMask playerLayers;




    private void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        // Press attack 
        if (attackCalled)
        {
            isAttacking = true;
            lastAttack = Time.time;
        }


        // The case where attack is finished 
        else if (Time.time - lastAttack > 1f)
            isAttacking = false;


        //anim.SetBool("isAttacking", isAttacking);

    }



    private void OnDrawGizmosSelected()
    {
        if (enemyAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enemyAttackPoint.position, attackRange);
    }



}
