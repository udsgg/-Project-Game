using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float airWalkSpeed = 3f;
    public float attackingMoveSpeed = 2f;
    public float jumpImpulse = 8f;
    public float groundDistance = 0.05f;

    public Collider2D groundCollider, wallCollider;


    Rigidbody2D rb;
    Vector2 moveInput;
    Animator animator;
    CapsuleCollider2D physCollider;

    [Header("Physics State")]
    [SerializeField]
    private bool _isFacingRight = true;

    internal bool IsFacingRight { get { return _isFacingRight; } set { _isFacingRight = value; animator.SetBool(AnimationStrings.isFacingRight, value); } }

    [SerializeField]
    private bool _isRunning;

    internal bool IsRunning { get { return _isRunning; } set { _isRunning = value; animator.SetBool(AnimationStrings.isRunning, value); } }

    [SerializeField]
    private bool _isMoving;
    private Vector2 facingDirection;
    private ContactPoint2D[] contactPoints = new ContactPoint2D[20];

    internal bool IsMoving { get { return _isMoving; } set { _isMoving = value; animator.SetBool(AnimationStrings.isMoving, value); } }

    // Set in animator behaviours so only getting the property
    internal bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); } }

    private bool IsAttacking { get { return animator.GetBool(AnimationStrings.isAttacking); } }

    private float currentMoveSpeed => CalculateMoveSpeed();

    public bool IsHit { get { return animator.GetBool(AnimationStrings.isHit); } set { animator.SetBool(AnimationStrings.isHit, value); } }

    public bool IsAlive { get { return animator.GetBool(AnimationStrings.isAlive); } }
    
    private TouchingDirections touchingDirections;

    

    private float CalculateMoveSpeed()
    {
        if (CanMove)
        {
            // X Movement
            if (_isMoving)
            {
                if (touchingDirections.IsGrounded)
                {
                    // Character is on the ground
                    if(IsAttacking)
                    {
                        // Attack preceeds other movement
                        return attackingMoveSpeed;
                    }
                    if (_isRunning)
                    {
                        // Run Move
                        return runSpeed;
                    }
                    else
                    {
                        // Walk Move
                        return walkSpeed;
                    }
                }
                else
                {
                    // Air walk
                    if (IsAttacking)
                    {
                        return attackingMoveSpeed;
                    } 
                    else
                    {
                        return airWalkSpeed;
                    }
                }
            }
            else
            {
                // Not moving so movement is 0
                return 0;
            }
        }

        return 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physCollider = GetComponent<CapsuleCollider2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    void OnTriggrerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Exit") {
            Debug.Log("Exit");
        }
    }

    private void FixedUpdate()
    {
         if(IsAlive)
        {
            // Keep the Touching Directions settings updated to the players current state
            touchingDirections.wallDistance =currentMoveSpeed * Time.fixedDeltaTime;
            touchingDirections.wallCheckDirection = facingDirection;

            // Update parameters on animator
            animator.SetBool(AnimationStrings.isMoving, moveInput != Vector2.zero ? true : false);
            animator.SetFloat(AnimationStrings.xVelocity, rb.velocity.x);

            // Needed for falling / jumping animation decision
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

            // Being hit locks movement from changing as the velocity is set by the knockback hit
            if (IsHit)
            {
                // Controlled by knockback
            }
            else if (!touchingDirections.IsOnWall)
            {
                rb.velocity = new Vector2(currentMoveSpeed * moveInput.x, rb.velocity.y);
            }
        }
    }

    // Unity InputSystem PlayerInput Actions
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero ? true : false;

        // Allowed to switch direction
        if (IsAlive)
            SetFacingDirection(moveInput);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
            animator.SetBool(AnimationStrings.isRunning, _isRunning);
        }
        else if (context.canceled)
        {
            IsRunning = false;
            animator.SetBool(AnimationStrings.isRunning, _isRunning);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attack);

            if(touchingDirections.IsGrounded)
                animator.SetTrigger(AnimationStrings.ground_interrupt);
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttack);

            if (touchingDirections.IsGrounded)
                animator.SetTrigger(AnimationStrings.ground_interrupt);
        }
    }

    // Jump keys pressed
    public void OnJump(InputAction.CallbackContext context)
    {
       
        if (context.started && touchingDirections.IsGrounded)
        {
            if (IsAlive)
            {
                // jumpTrigger = true;
                animator.SetTrigger(AnimationStrings.jump);
                animator.SetTrigger(AnimationStrings.ground_interrupt);

                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            }
        }
    }

    // Jump game action
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpImpulse);
    }

    // Flips facing direction of the the transform when x movement direction changes
    private void SetFacingDirection(Vector2 inputDirection)
    {
        // Flip if now moving to the right
        if (inputDirection.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
            transform.localScale *= new Vector2(-1, 1);
            facingDirection = Vector2.right;
        }
        // Flip if now moving to the left
        else if (inputDirection.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            transform.localScale *= new Vector2(-1, 1);
            facingDirection = Vector2.left;
        }
    }

    // Called from JumpBehaviour in State Machine
    internal void ApplyJumpForce()
    {
        rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        IsHit = true;
        rb.velocity = new Vector2(knockback.x, knockback.y + rb.velocity.y);
    }
}
