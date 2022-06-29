using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyState
{
    public float attackTime;
    public float attackCooldown = 1f;

    public ChaseState chaseState;
    public EnemyAttack enemyAttack;
    public HurtState hurtState;
    public EnemyStateManager enemyStateManager;
    public Enemy enemyScript;

    public override EnemyState RunCurrentState()
    {
        if (enemyStateManager.i != 2)
        {
            attackTime = Time.time;
            enemyStateManager.i = 2;
        }

        enemyStateManager.moveVector = Vector2.zero;

        if (enemyScript.isHit)
        {
            enemyAttack.isAttacking = false;
            enemyAttack.attackCalled = false;
            return hurtState;

        }

        if (Time.time - attackTime < attackCooldown)
        {
            enemyAttack.attackCalled = true;
            return this;
        }


        else
            return chaseState;

    }
}
