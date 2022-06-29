using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Fighter
{


    private void Update()
    {
        if (Time.time - lastImmune > recoverTime)
        {
            isHit = false;
        }
    }

    protected override void Death()
    {
        //implement dying.
        Debug.Log("DEAD!");
    }
}
