using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : EnemyState
{
    public EnemyStateManager enemyStateManager;

    public override EnemyState RunCurrentState()
    {
        if (enemyStateManager.i != 4)
        {
            enemyStateManager.moveVector.x = 0;
            enemyStateManager.i = 4;
        }

        return this;
    }
}

