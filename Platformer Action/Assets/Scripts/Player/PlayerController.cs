using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for movement
    public float moveSpeed = 3.0f;
    public float jumpForce;
    private float moveDir;
    private bool facingRight = true;
    public Rigidbody2D rb;
    public Transform playerTransform;
    private Vector2 moveVector;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    public float jumpingGravityScale;
    public float fallingGravityScale;

    public bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    public PlayerAttack playerAttack;
    public bool isAttacking;





    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        isAttacking = playerAttack.isAttacking;

        // Get user input every frame.
        moveVector = GetInput();

        // Surroundings check

        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);


        Jump();

        doAnimations();
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            isJumping = true;

            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            moveVector.y = jumpForce;
        }



        if (Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                moveVector.y = jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }

            else
                Falling();
        }

        if (Input.GetKeyUp(KeyCode.Space))
            Falling();
    }

    private void FixedUpdate()
    {
        Run(moveVector);
    }

    private void Run(Vector2 moveVector)
    {
        // Stop motion when attacking.
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }


        rb.velocity = new Vector2(moveVector.x * moveSpeed, moveVector.y);
    }

    private Vector2 GetInput()
    {
        float x;

        if (Input.GetKey(KeyCode.RightArrow))
            x = 1;

        else if (Input.GetKey(KeyCode.LeftArrow))
            x = -1;

        else
            x = 0;


        if (((x < 0 && facingRight) || (x > 0 && !facingRight)) && !isAttacking)
        {
            Flip();
        }

        return new Vector2(x, rb.velocity.y);
    }   

    private void doAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("ySpeed", rb.velocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        playerTransform.localScale = new Vector2(-playerTransform.localScale.x, playerTransform.localScale.y);
    }

    private void Falling()
    {
        isJumping = false;
        rb.gravityScale = fallingGravityScale;

    }
}
