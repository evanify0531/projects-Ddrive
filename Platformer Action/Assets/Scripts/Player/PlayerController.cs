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

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    public float jumpingGravityScale;
    public float fallingGravityScale;

    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;





    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();   
    }


    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            rb.velocity = Vector2.up * jumpForce;
        }

        

        if (Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                rb.velocity = Vector2.up * jumpForce;
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
        moveDir = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("xSpeed", Mathf.Abs(moveDir));
        anim.SetFloat("ySpeed", rb.velocity.y);
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        if ((moveDir < 0 && facingRight) || (moveDir > 0 && !facingRight))
        {
            Flip();
        }
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
        anim.SetBool("isJumping", isJumping);
    }
}
