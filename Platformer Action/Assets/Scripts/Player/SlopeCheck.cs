using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeCheck : MonoBehaviour
{
    //private float checkRadius = 0.225f;
    public LayerMask whatIsGround;
    public RaycastHit2D isOnSlope;
    private float checkDistance = 0.3f;
    public float angle;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isOnSlope = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, whatIsGround);
        angle = Vector2.Angle(isOnSlope.normal.normalized, Vector2.left);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + Vector3.down);
        Gizmos.DrawLine(new Vector3(isOnSlope.point.x, isOnSlope.point.y, 0), new Vector3(isOnSlope.point.x + isOnSlope.normal.normalized.x, isOnSlope.point.y + isOnSlope.normal.normalized.y, 0));
    }
}
