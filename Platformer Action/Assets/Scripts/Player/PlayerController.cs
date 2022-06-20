using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables for movement
    public float moveSpeed = 3.0f;
    public float jumpForce;
    private bool facingRight = true;
    private Vector2 moveVector;

    // Variables for jumping from the ground
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    //private bool isFalling;
    // Different gravity scales in different states of jumping.
    public float jumpingGravityScale;
    public float fallingGravityScale;

    // Variables for Ground checking
    public bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    // wall checks and sliding
    public float wallCheckDistance;
    public bool isTouchingWall; // bool checking whether wall is being touched
    public bool isWallSliding; // bool checking when wall sliding
    public bool wallJumping; // bool checking when wall jumping
    public float wallSlideSpeed;
    public float wallJumpForce;

    // used to check whether the player wants to drop off from the wall or not.
    public float wallSlideCancelTime = 1f;
    public float wallSlideCancelPressTime = 0f;
    
    public float lastWallJumpTime = 0f;
    public float wallSlideCooldown = 0.2f; // So that player immediately doesn't attach to the same wall after jumping off.
    public float wallJumpKickOffForce = 1.8f;


    // variables for ledge detetction and grabbing
    // ledge grab is postponed until I get an animation
    //public bool isTouchingLedge = false;

    // Utilized in GetInput function.
    public float x;

    // Bool used so that player can move immediately after a wall jump
    private bool canMove;

    // Bool so that after a wall jump there is a little bit of x axis movement when there is no input.
    private bool justWallJumped;


    // Reference to game objects
    public Rigidbody2D rb;
    public Transform playerTransform;
    private Animator anim;
    public Transform wallCheck; // For checking whether there is a wall next to the player.
    //public Transform ledgeCheck;

    // Variables for attacking
    public PlayerAttack playerAttack;
    public bool isAttacking;

    // Variables for dashing
    private bool canDash = true;
    public bool isDashing;
    private float dashingPower = 20f;
    private float dashingTime = 0.20f;
    private float dashingCooldown = 0.85f;
    [SerializeField] private TrailRenderer tr;

    // Variables for dash attack
    public bool isDashAttacking = false;
    public bool canDashAttack = false;
    public float dashAttackLeeway = 0.85f;


    // variables for crouching (not moving yet)
    public bool isCrouching = false;

    void Start()
    {
        anim = GetComponent<Animator>();

    }


    void Update()
    {

        isAttacking = playerAttack.isAttacking;

        canMove = CheckCanMoveBool();

        // Surroundings check
        CheckSurroundings();

        // Get user input every frame.
        moveVector = GetInput();

        GetDashInput();

        DashAttack();

        Jump();

        Crouch();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {

        ApplyMovement(moveVector);
    }


    private bool CheckCanMoveBool()
    {
        if (Time.time - lastWallJumpTime <= wallSlideCooldown)
        {
            return false;
        }

        else if ((isDashing || isDashAttacking) && isGrounded)
        {
            return false;
        }

        else
            return true;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        // I might need to change the layer mask to whatIsGround to another layer just for walls, if I find myself in some trouble with the logic.
        isTouchingWall = Physics2D.Raycast(wallCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);

        //isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);
        
        if (isTouchingWall && !isGrounded && rb.velocity.y < 5 && Time.time - lastWallJumpTime > wallSlideCooldown && !isDashAttacking)
        {
            isWallSliding = true;
        }
        
        //else if (!isTouchingLedge && isTouchingWall &&)
        
            
        



        else
            isWallSliding = false;
    }

    private Vector2 GetInput()
    {
        if (Input.GetKey(KeyCode.RightArrow) && !isWallSliding && canMove)
            x = 1;

        else if (Input.GetKey(KeyCode.LeftArrow) && !isWallSliding && canMove)
            x = -1;


        else if (isWallSliding)
        {
            if (facingRight)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    wallSlideCancelPressTime = Time.time;
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (Time.time - wallSlideCancelPressTime > 0.5f)
                    {
                        x = -1;
                        isWallSliding = false;
                    }
                }

                else
                    x = 0;

            }

            else if (!facingRight)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    wallSlideCancelPressTime = Time.time;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    if (Time.time - wallSlideCancelPressTime > 0.5f)
                    {
                        x = 1;
                        isWallSliding = false;
                    }
                }

                else
                    x = 0;

            }

        }

        else
            x = 0;


        if (((x < 0 && facingRight) || (x > 0 && !facingRight)) && !isAttacking && canMove)
        {
            Flip();
        }

        // Controlling wall slide speed
        wallSlideSpeed = WallSlideSpeed();

        return new Vector2(x, rb.velocity.y);
    }

    private void GetDashInput()
    {
        if (Input.GetKeyDown(KeyCode.D) && canDash && !isWallSliding && !isAttacking)
        {
            StartCoroutine(CanDashAttack());
            StartCoroutine(Dash());
            
        }
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

        else if (isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            lastWallJumpTime = Time.time;
            isJumping = true;
            wallJumping = true;
            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            wallJumpKickOffForce = 1.8f;
            Flip();
            moveVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
            moveVector.y = wallJumpForce;
            justWallJumped = true;
        }

        else if (Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                // Only when wall jumping
                if (wallJumping)
                {
                    wallJumpKickOffForce -= 0.1f;
                    if (wallJumpKickOffForce < 1)
                        wallJumpKickOffForce = 1;
                    moveVector.y = wallJumpForce;
                    moveVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
                    jumpTimeCounter -= Time.deltaTime;
                }
                // Only when jumping from the ground
                else if (isJumping && !wallJumping)
                {
                    moveVector.y = jumpForce;
                    jumpTimeCounter -= Time.deltaTime;

                }
            }

            else
                Falling();
        }

        else if (Input.GetKeyUp(KeyCode.Space))
            Falling();

        if (justWallJumped && isGrounded)
        {
            justWallJumped = false;
        }
    }

    private void Crouch()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking)
        {
            isCrouching = true;
        }

        if (isGrounded && Input.GetKeyUp(KeyCode.LeftShift))
        { 
            isCrouching = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isDashAttacking", isDashAttacking);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("ySpeed", rb.velocity.y);
    }

    private void ApplyMovement(Vector2 motorVector)
    {
        // Stop motion when attacking.
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isWallSliding)
        {

            if (rb.velocity.y < -wallSlideSpeed)
            {
                motorVector.y = -wallSlideSpeed;
            }
        }


        if (justWallJumped)
        {
            motorVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
        }

        // is falling
        if (!isGrounded && !isJumping && !isDashing)
        
        {
            rb.velocity = new Vector2(motorVector.x * moveSpeed / 1.25f, motorVector.y);

        }

        else if (isDashing)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        }

        else if (isCrouching)
        {
            rb.velocity = new Vector2(motorVector.x * moveSpeed / 3f, motorVector.y);
        }

        else
            rb.velocity = new Vector2(motorVector.x * moveSpeed, motorVector.y);
    }


    private float WallSlideSpeed()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            return 5f;
        }

        else if (Input.GetKey(KeyCode.UpArrow))
        {
            return 1f;
        }

        else
            return 2f;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        playerTransform.localScale = new Vector2(-playerTransform.localScale.x, playerTransform.localScale.y);
    }

    private void Falling()
    {
        wallJumping = false;
        isJumping = false;
        //isFalling = true;
        rb.gravityScale = fallingGravityScale;
    }

    private IEnumerator Dash()
    {
        bool checkIfJumping;
        if (isJumping)
            checkIfJumping = true;
        else
            checkIfJumping = false;

       // dashInputTime = Time.time;
        canDash = false;
        isDashing = true;
        float originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        tr.emitting = true;
        yield return new WaitForSecondsRealtime(dashingTime);

        tr.emitting = false;

        if (checkIfJumping)
            rb.gravityScale = 5f;
        else
            rb.gravityScale = originalGravityScale;

        isDashing = false;
        yield return new WaitForSecondsRealtime(dashingCooldown);
        canDash = true;
    }

    private void DashAttack()
    {
        if (canDashAttack && Input.GetKeyDown(KeyCode.A))
        {
            isDashAttacking = true;
            isDashing = false;
            canDashAttack = false;
        }
            

        else if (!canDashAttack && Input.GetKeyDown(KeyCode.A))
        {
            return;
        }
    }

    private IEnumerator CanDashAttack()
    {
        canDashAttack = true;

        yield return new WaitForSecondsRealtime(dashAttackLeeway);

        canDashAttack = false;
        if (isDashAttacking)
        {
            isDashAttacking = false;
        }

    }
    

    // Function to be called by animation event, at the end of the dash attack animation.
    private void TurnOffIsDashAttacking()
    {
        Debug.Log("ehy");
        isDashAttacking = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + playerTransform.localScale.x * wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
