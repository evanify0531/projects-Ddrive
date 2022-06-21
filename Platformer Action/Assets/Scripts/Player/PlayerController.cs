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
    public Transform ledgeCheck;
    private bool isTouchingLedge = false;
    private bool isGrabbingLedge = false;


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

    // For ledge grabbing and idle states.
    private int i = 0;

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

        else if (isGrabbingLedge)
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

        // Change layer mask to walls later
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);
        
        if (isTouchingWall && !isGrounded && rb.velocity.y < 5 && Time.time - lastWallJumpTime > wallSlideCooldown && !isDashAttacking && isTouchingLedge)
        {
            isWallSliding = true;
            i = 0;
        }

        // things to do: ledge grab sprite locking in with offset
        // when jumping from ledge jumps forward, not straight up
        
        else if (!isTouchingLedge && isTouchingWall)
        {
            if (i == 0)
            {
                isWallSliding = false;
                isGrabbingLedge = true;
                i = 1;
            }
        }

        else
        {
            i = 0;
            isGrabbingLedge = false;
            isWallSliding = false;
        }
            
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

        else if (isGrabbingLedge)
        {
            rb.gravityScale = 0f;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                wallSlideCancelPressTime = Time.time;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (Time.time - wallSlideCancelPressTime > 0.5f)
                {
                    isGrabbingLedge = false;
                    rb.gravityScale = fallingGravityScale;
                    isWallSliding = true;
                }
            }

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
                        isGrabbingLedge = false;
                        rb.gravityScale = fallingGravityScale;
                    }
                }

                else
                    x = 0;

            }

            if (!facingRight)
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
                        isGrabbingLedge = false;
                        rb.gravityScale = fallingGravityScale;
                    }
                }

                else
                    x = 0;

            }

            /*else if(Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("ASDI");
                isGrabbingLedge = false;
            }*/
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
        if (Input.GetKeyDown(KeyCode.C) && canDash && !isWallSliding && !isAttacking)
        {
            StartCoroutine(CanDashAttack());
            StartCoroutine(Dash());
            
        }
    }

    private void Jump()
    {

        if (isGrounded && Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            moveVector.y = jumpForce;
        }

        if (isWallSliding && Input.GetKeyDown(KeyCode.Z))
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
        //is jumping never called when facing right. why?
        if (isGrabbingLedge && Input.GetKeyDown(KeyCode.Z))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            isGrabbingLedge = false;
            rb.gravityScale = jumpingGravityScale;
            moveVector.y = jumpForce;

        }

        else if (Input.GetKey(KeyCode.Z))
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

        else if (Input.GetKeyUp(KeyCode.Z))
            Falling();

        if (justWallJumped && (isGrounded || isWallSliding))
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

        if (Input.GetKeyUp(KeyCode.LeftShift))
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
        anim.SetInteger("ledgeGrabState", i);
        anim.SetBool("isGrabbingLedge", isGrabbingLedge);
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

        //keeps going down even when grabbing ledge... I don't know why
        if (isWallSliding && !isGrabbingLedge)
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
        else if (!isGrounded && !isJumping && !isDashing && !isWallSliding && !isGrabbingLedge && !justWallJumped)
        
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

        else if (isGrabbingLedge)
        {
            rb.velocity = Vector2.zero;
        }

        else
            rb.velocity = new Vector2(motorVector.x * moveSpeed, motorVector.y);
    }


    private float WallSlideSpeed()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            return 7f;
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
        if (canDashAttack && Input.GetKeyDown(KeyCode.X))
        {
            isDashAttacking = true;
            isDashing = false;
            canDashAttack = false;
        }
            

        else if (!canDashAttack && Input.GetKeyDown(KeyCode.X))
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

    //Called at the end of the animation for ledge grabbing.
    private void SetIToTwo()
    {
        i = 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetPos.position, checkRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + playerTransform.localScale.x * wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + playerTransform.localScale.x * wallCheckDistance, ledgeCheck.position.y, ledgeCheck.position.z));

    }
}
