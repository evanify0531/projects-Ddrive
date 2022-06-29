using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public EnemyState currentState;
    public Vector3 initialPos;

    public Animator anim;

    public GameObject enemy;
    public Enemy enemyScript;
    public Transform player;
    public Transform enemyDetectionObject;
    public bool facingRight = false;

    public Vector2 moveVector;

    public float distanceToPlayer;
    public Vector3 vectorFacingForward;
    public Vector3 enemyToPlayer;
    public float angle;
    public RaycastHit2D hit;
    public Rigidbody2D rb;

    public int i;
    

    private void Awake()
    {
        initialPos = this.transform.position;
        anim = GetComponent<Animator>();

    }


    void Update()
    {
        enemyDetectionObject.localScale = enemy.transform.localScale;
        GetInformation();
        RunStateMachine();
        //update animations
        anim.SetInteger("i", i);
        
    }

    private void FixedUpdate()
    {
        UpdateMovement(moveVector);
    }

    private void GetInformation()
    {
        distanceToPlayer = Vector2.Distance(enemyDetectionObject.position, player.position);
        vectorFacingForward = new Vector3(-enemyDetectionObject.localScale.x, 0f, 0f);
        enemyToPlayer = player.position - enemyDetectionObject.position;
        angle = Vector3.Angle(vectorFacingForward, enemyToPlayer);
        hit = Physics2D.Raycast(enemyDetectionObject.position, player.position);
    }

    private void RunStateMachine()
    {
        // Every frame, the nextState is determined by the code below. If currentState returns null, nextState will be null.
        // If it isn't null after running the 'RunCurrentState' function, then nextState will be assigned with whatever that function returned.
        EnemyState nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            SwitchToTheNextState(nextState);
        }


    }

    private void SwitchToTheNextState(EnemyState nextState)
    {
        currentState = nextState;
    }


    private void UpdateMovement(Vector2 motorVector)
    {
        rb.MovePosition(new Vector2 (enemy.transform.position.x + motorVector.x, enemy.transform.position.y + motorVector.y));
    }
    
}
