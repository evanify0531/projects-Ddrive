using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : EnemyState
{
    public ChaseState chaseState;
    public HurtState hurtState;
    public bool canSeePlayer;

    public float lookRadius = 5f;

    public EnemyStateManager enemyStateManager;

    public float minimumDistance = 2f;


    public override EnemyState RunCurrentState()
    {
        enemyStateManager.i = 0;
        enemyStateManager.moveVector = Vector2.zero;

        // Check distance between enemy and player
        canSeePlayer = PlayerDetection();

        if (canSeePlayer)
        {
            return chaseState;
        }

        if(enemyStateManager.enemyScript.isHit)
        {
            return hurtState;
        }
        
        else
        {
            return this;
        }
    }

    private bool PlayerDetection()
    {
        if (enemyStateManager.distanceToPlayer <= minimumDistance && enemyStateManager.angle > 120f)
        {
            return true;
        }

        else if (enemyStateManager.distanceToPlayer <= lookRadius && enemyStateManager.angle < 65f)
        {
            return true;
        }

        else
            return false;


        
    }

    // I'll enable this when I need to check the vectors.
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - enemyStateManager.enemyDetectionObject.localScale.x, transform.position.y, 0f));
        Gizmos.DrawLine(transform.position, enemyStateManager.player.position);
    }*/
}
