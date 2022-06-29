using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Fighter
{

    protected override void Death()
    {
        //implement dying.
        Debug.Log("DEAD!");
    }
}
