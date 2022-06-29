using UnityEngine;

public class CeilingCheck : MonoBehaviour
{
    private RaycastHit2D ceilingCheckRC;
    public bool canStand;
    public float checkDistance = 0.15f;
    public LayerMask whatIsGround;

    private void Update()
    {
        ceilingCheckRC = Physics2D.Raycast(transform.position, Vector2.up, checkDistance, whatIsGround);

        if (ceilingCheckRC)
        {
            canStand = false;
        }

        else
            canStand = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f, checkDistance, 0f));
    }
}
