using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : EnemyState
{
    public Enemy enemy;
    public IdleState idleState;
    public DeathState deathState;
    public EnemyStateManager enemyStateManager;
    public float hurtTime = 0.25f;
    public float hurtTimeCounter;

    public override EnemyState RunCurrentState()
    {
        if (enemyStateManager.i != 3)
        {
            enemyStateManager.i = 3;
            hurtTimeCounter = Time.time;
            enemyStateManager.moveVector = new Vector2(enemy.pushDirection.x / 200f, 0);
        }

        if (Time.time - hurtTimeCounter > hurtTime)
        {
            enemy.isHit = false;
            enemyStateManager.moveVector = Vector2.zero;
            return idleState;
        }

        if (enemy.HP <= 0)
        {
            return deathState;
        }

        else
        {
            return this;
        }

    }
}
