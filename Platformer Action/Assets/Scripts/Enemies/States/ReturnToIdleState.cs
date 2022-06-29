
using UnityEngine;

public class ReturnToIdleState : EnemyState
{
    public IdleState idleState;
    public ChaseState chaseState;
    public HurtState hurtState;
    public EnemyStateManager enemyStateManager;

    public override EnemyState RunCurrentState()
    {
        enemyStateManager.i = 1;

        if (enemyStateManager.distanceToPlayer < 2f)
        {
            return chaseState;
        }

        if (enemyStateManager.enemyScript.isHit)
        {
            return hurtState;
        }

        float x = enemyStateManager.enemy.transform.position.x - enemyStateManager.initialPos.x;

        if (x > 0.3)
        {
            if (enemyStateManager.enemy.transform.localScale.x != 1)
                enemyStateManager.enemy.transform.localScale = new Vector3(1, 1, 1);

            enemyStateManager.moveVector = new Vector2(-0.03f, 0);

            return this;
        }

        else if (x < -0.3)
        {
            if (enemyStateManager.enemy.transform.localScale.x != -1)
                enemyStateManager.enemy.transform.localScale = new Vector3(-1, 1, 1);

            enemyStateManager.moveVector = new Vector2(0.03f, 0);

            return this;
        }

        else
        {
            return idleState;
        }

    }
}
