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
    private RaycastHit2D isTouchingWallRC; // bool checking whether wall is being touched
    public bool isWallSliding; // bool checking when wall sliding
    public bool wallJumping; // bool checking when wall jumping
    public float wallSlideSpeed;
    public float wallJumpForce;
    //public LayerMask whatIsWall;

    // used to check whether the player wants to drop off from the wall or not.
    public float wallSlideCancelTime = 1f;
    public float wallSlideCancelPressTime = 0f;

    public float lastWallJumpTime = 0f;
    public float wallSlideCooldown = 0.3f; // So that player immediately doesn't attach to the same wall after jumping off.
    public float wallJumpKickOffForce = 1.8f;


    // variables for ledge detetction and grabbing
    public Transform ledgeCheck;
    private bool isTouchingLedge = false;
    private bool isGrabbingLedge = false;
    private float ledgeGrabXPos;
    private float ledgeGrabXOffset;
    private float ledgeGrabYOffset;
    private Collider2D ledgeBC;



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
    private float dashingCooldown = 0.8f;
    [SerializeField] private TrailRenderer tr;

    // Variables for dash attack
    public bool isDashAttacking = false;
    public bool canDashAttack = false;
    public float dashAttackLeeway = 0.8f;


    // variables for crouching (not moving yet)
    private bool isCrouching = false;
    public CeilingCheck ceilingCheck1; // for checking if we can stand while crouching.
    public CeilingCheck ceilingCheck2;
    private bool canStand;


    // variables for sliding
    private bool canSlide = false;
    public bool isSliding = false;
    private float slideLeeway = 0.6f;
    private float slidingPower = 13f;
    private float maxSlideTime = 0.8f;
    private float slideInputTime;

    // For ledge grabbing and idle states animation control.
    private int i = 0;


    // Coyote time
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    // Jump buffer
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // ledge grab to jump
    private float grabTime = 0.2f;
    private float grabTimeCounter;

    public PlayerHealth playerHealth;

    // Different run system
    //public float acceleration = 9f;
    //public float deceleration = 9f;
    //public float velPower = 1.2f;





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

        CoyoteTime();

        // Get user input every frame.
        moveVector = GetInput();

        GetDashInput();

        DashAttack();

        Slide();

        JumpBuffer();

        Jump();

        WallJump();

        Crouch();

        Hurt();

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

        else if ((isDashing || isDashAttacking || isSliding) && isGrounded)
        {
            return false;
        }

        else if ((isDashing && !isGrounded))
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
        //isTouchingWall = Physics2D.Raycast(wallCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);
        isTouchingWallRC = Physics2D.Raycast(wallCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);



        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, new Vector2(playerTransform.localScale.x, 0), wallCheckDistance, whatIsGround);



        if (isTouchingWallRC && !isGrounded && rb.velocity.y < 5 && Time.time - lastWallJumpTime > wallSlideCooldown && !isDashAttacking && isTouchingLedge)
        {
            isWallSliding = true;
            i = 0;
        }

        else if (!isTouchingLedge && isTouchingWallRC)
        {
            // Enter ledge grabbing state
            if (i == 0 && Input.GetKey(KeyCode.Z))
            {
                isWallSliding = false;
                isGrabbingLedge = true;
                grabTimeCounter = Time.time;

                ledgeBC = isTouchingWallRC.collider;

                ledgeGrabXPos = ledgeBC.bounds.center.x;
                ledgeGrabXOffset = (ledgeBC.bounds.size.x / 2f) + 0.31f;
                ledgeGrabYOffset = ledgeBC.bounds.center.y + (ledgeBC.bounds.size.y / 2f) - 1.03f;

                if (facingRight)
                    transform.position = new Vector3(ledgeGrabXPos - ledgeGrabXOffset, ledgeGrabYOffset, 0f);
                else
                    transform.position = new Vector3(ledgeGrabXPos + ledgeGrabXOffset, ledgeGrabYOffset, 0f);
                i = 1;
            }

            if (isGrabbingLedge)
            {
                if (facingRight)
                    transform.position = new Vector3(ledgeGrabXPos - ledgeGrabXOffset, ledgeGrabYOffset, 0f);
                else
                    transform.position = new Vector3(ledgeGrabXPos + ledgeGrabXOffset, ledgeGrabYOffset, 0f);
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
        if (Input.GetKeyDown(KeyCode.C) && canDash && !isWallSliding && !isAttacking && !isGrabbingLedge && !isCrouching)
        {
            StartCoroutine(CanDashAttack());
            StartCoroutine(CanSlide());
            StartCoroutine(Dash());

        }
    }

    private void JumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isAttacking && !isWallSliding && !isDashAttacking)
        {
            jumpBufferCounter = 0f;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            moveVector.y = jumpForce;

        }

        // I'm changing this to getting up on the edge later, instead of just a upward jump
        else if (isGrabbingLedge && Input.GetKeyDown(KeyCode.Z) && Time.time - grabTime > grabTimeCounter)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            isGrabbingLedge = false;
            rb.gravityScale = jumpingGravityScale;
            moveVector.y = jumpForce;

        }

        if (isJumping && !wallJumping)
        {
            if (jumpTimeCounter > 0)
            {
                moveVector.y = jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            
            else
            {
                Falling();
            }
        }

        if (isJumping && !wallJumping && Input.GetKeyUp(KeyCode.Z) && !justWallJumped)
        {
            Falling();
        }
    }

    private void WallJump()
    {
        if (isWallSliding && Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = 0f;
            lastWallJumpTime = Time.time;
            isJumping = true;
            wallJumping = true;
            jumpTimeCounter = jumpTime;
            rb.gravityScale = jumpingGravityScale;
            wallJumpKickOffForce = 2f;
            Flip();
            moveVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
            moveVector.y = wallJumpForce;
            justWallJumped = true;
        }

        if (isJumping && wallJumping)
        {
            if (jumpTimeCounter > 0)
            {
                wallJumpKickOffForce -= 0.1f;
                if (wallJumpKickOffForce < 1)
                    wallJumpKickOffForce = 1;
                moveVector.y = wallJumpForce;
                moveVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
                jumpTimeCounter -= Time.deltaTime;
            }

            else
                Falling();
        }

 /*       if (Input.GetKeyUp(KeyCode.Z) && justWallJumped)
        {
            Debug.Log("Here");
            Falling();
        }*/

        if (justWallJumped && (isGrounded || isWallSliding) && jumpTimeCounter <= 0)
        {
            justWallJumped = false;
        }
    }

    private void CheckCanStandBool()
    {
        if (ceilingCheck1.canStand && ceilingCheck2.canStand)
            canStand = true;

        else
            canStand = false;
    }

    private void Crouch()
    {
        CheckCanStandBool();

        if (isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && !isSliding)
        {
            isCrouching = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && isCrouching)
        {
            isCrouching = true;
        }

        else if (isCrouching && canStand)
        { 
            isCrouching = false;
        }

        

    }

    private void CoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }

        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void Hurt()
    {
        if (playerHealth.isHit)
        {
            if (isAttacking)
                isAttacking = false;

            if (isDashing)
                isDashing = false;

            if (isDashAttacking)
                isDashAttacking = false;

            if (isSliding)
                isSliding = false;

            if (isWallSliding)
                isWallSliding = false;

            if (isCrouching)
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
        anim.SetBool("isSliding", isSliding);
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


        if (isWallSliding && !isGrabbingLedge)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                motorVector.y = -wallSlideSpeed;
            }
        }


        if (wallJumping)
        {
            motorVector.x = playerTransform.localScale.x * wallJumpKickOffForce;
        }

        // is falling
        if (!isGrounded && !isJumping && !isDashing && !isWallSliding && !isGrabbingLedge)
        
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

        else if (isSliding)
        {
            rb.velocity = new Vector2(transform.localScale.x * slidingPower, 0f);
            if (slidingPower >= 1)
                slidingPower -= 0.2f;
        }

        else
        {
            // This is a force based run system that doesn't work with my get axis raw movement system.
            // I'll keep the code here and commented so that if I return with a new project I can utilize that system.
            // https://www.youtube.com/watch?v=KbtcEVCM7bw
            /*float targetSpeed = motorVector.x * moveSpeed;
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
            rb.AddForce(movement * Vector2.right);
            rb.velocity = new Vector2(rb.velocity.x, moveVector.y);*/
            rb.velocity = new Vector2(motorVector.x * moveSpeed, motorVector.y);
        }   


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
        coyoteTimeCounter = 0f;
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
    
    private void Slide()
    {
        if (canSlide && Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            slideInputTime = Time.time;
            slidingPower = 13f;
            isSliding = true;
            isDashing = false;
            canSlide = false;
        }

        else if (!canSlide && Input.GetKeyDown(KeyCode.LeftShift))
        {
            return;
        }

        else if (isSliding && (Input.GetKeyUp(KeyCode.LeftShift) || Time.time - slideInputTime > maxSlideTime))
        {
            isSliding = false;

            if (!canStand)
            {
                Debug.Log("cant stand");
                isCrouching = true;
            }

        }
    }

    private IEnumerator CanSlide()
    {
        canSlide = true;

        yield return new WaitForSecondsRealtime(slideLeeway);

        canSlide = false;
    }


    // Function to be called by animation event, at the end of the dash attack animation.
    private void TurnOffIsDashAttacking()
    {
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


/*private void WallJump()
{
    if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isAttacking && !isWallSliding && !isDashAttacking)
    {

        jumpBufferCounter = 0f;
        isJumping = true;
        jumpTimeCounter = jumpTime;
        rb.gravityScale = jumpingGravityScale;
        moveVector.y = jumpForce;
    }

    if (isWallSliding && jumpBufferCounter > 0f)
    {
        jumpBufferCounter = 0f;
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

    // I'm changing this to getting up on the edge later, instead of just a upward jump
    if (isGrabbingLedge && Input.GetKeyDown(KeyCode.Z))
    {
        isJumping = true;
        jumpTimeCounter = jumpTime;
        isGrabbingLedge = false;
        rb.gravityScale = jumpingGravityScale;
        moveVector.y = jumpForce;

    }

    if (Input.GetKey(KeyCode.Z))
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
}*/
