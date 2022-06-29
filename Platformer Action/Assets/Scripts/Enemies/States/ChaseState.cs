using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyState
{
    public IdleState idleState;
    public AttackState attackState;
    public ReturnToIdleState returnToIdleState;
    public HurtState hurtState;
    public bool isInAttackRange;
    public float chaseSpeed;

    public EnemyStateManager enemyStateManager;


    public override EnemyState RunCurrentState()
    {
        enemyStateManager.i = 1;

        // Flipping
        if (enemyStateManager.angle > 110f)
        {
            enemyStateManager.enemy.transform.localScale = new Vector2(-enemyStateManager.enemy.transform.localScale.x, enemyStateManager.enemy.transform.localScale.y);
        }

        // Moving toward the player
        enemyStateManager.moveVector = new Vector2(-idleState.transform.localScale.x * chaseSpeed, 0);



        // in attack range
        if (enemyStateManager.distanceToPlayer < 3f && Time.time - attackState.attackTime > 2f)
        {
            return attackState;
        }

        // player ran away and is far from the enemy. Return to idle
        if (enemyStateManager.distanceToPlayer > 15f)
        {
            // return_to_idle state
            return returnToIdleState;

        }

        if (enemyStateManager.enemyScript.isHit)
        {
            return hurtState;
        }

        else
        {
            return this;
        }
        
    }
}
