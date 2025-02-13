using UnityEngine;
using static Zombies ;

public class Zombies : MonoBehaviour
{
    public float walkSpeed = 3f;

    // Per Fixed Frame Lerp towards 0. 1 = 100%
    public float walkSpeedLerpStopRate = 0.2f;

    public DetectionZone clifEdgeZone;
    public DetectionZone attackZone;

    public enum WalkableDirection { Left, Right };


[SerializeField]
 private WalkableDirection _walkDirection;

    public WalkableDirection WalkDirection
    {
        get
        {
            return _walkDirection;
        }
        set
        {
            // Make changes only if new
            _walkDirection = value;

            if (value == WalkableDirection.Left)
            {
                // Facing left so negative scale
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                walkDirectionAsVector2 = Vector2.left;
            }
            else if (value == WalkableDirection.Right)
            {

                // Facing right so positive scale
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                walkDirectionAsVector2 = Vector2.right;
            }

            touchingDirections.wallCheckDirection = walkDirectionAsVector2;
        }
    }

    private CapsuleCollider2D mainCollider;
    Rigidbody2D rb;
    DetectionZone playerDetector;
    Animator animator;
    SpriteRenderer spriteRenderer;
    TouchingDirections touchingDirections;
    Vector2 walkDirectionAsVector2;

    // private Vector2 walkDirection => transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

    bool _hasTarget = false;

    private bool HasTarget
    {
        get { return _hasTarget; }
        set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    private bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool LockVelocity { get { return animator.GetBool(AnimationStrings.lockVelocity); } }

    public bool IsHit { get { return animator.GetBool(AnimationStrings.isHit); } set { animator.SetBool(AnimationStrings.isHit, value); } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDetector = GetComponentInChildren<DetectionZone>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        touchingDirections = GetComponent<TouchingDirections>();
        touchingDirections.wallDistance = walkSpeed * 1.5f * Time.fixedDeltaTime;

        // Randomize starting walk direction
        WalkDirection = Random.Range(0, 1) == 1 ? WalkableDirection.Left : WalkableDirection.Right;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        touchingDirections.wallCheckDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // If player walks into zone, set has target to trigger animations
        if (playerDetector.collidersInZone.Count > 0)
        {
            // Targetable enemy is in the zone so try to attack it
            HasTarget = true;
        }
        else
        {
            HasTarget = false;
        }


        RaycastHit2D[] wallHits = new RaycastHit2D[3]; 
        // Flip the knights walk direction when it runs into a wall or the edge of a cliff
        if (!IsHit && touchingDirections.IsGrounded && (touchingDirections.IsOnWall || clifEdgeZone.colliderCount == 0))
        {
            FlipWalkDirection();   
        }
        
        // Hit stun overrides movement
        if(IsHit)
        {
            // Hit stun cannot move
            // Velocity set OnHit
        }
        if (CanMove)
        {
            rb.velocity = new Vector2(walkSpeed * walkDirectionAsVector2.x, rb.velocity.y);
        } 
        else
        {
            // Default slow towards 0 on x velocity
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkSpeedLerpStopRate), rb.velocity.y);
        }
    }

    void FlipWalkDirection()
    {
        if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else
        {
            Debug.LogError(name + "'s WalkDirection is not set to Left or Right");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        IsHit = true;
        rb.velocity = new Vector2(knockback.x, knockback.y + rb.velocity.y);
    }
}
