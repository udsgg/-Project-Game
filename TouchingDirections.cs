using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D touchingFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;
    public Vector2 wallCheckDirection = Vector2.zero;

    private Animator animator;
    private CapsuleCollider2D col;

    [SerializeField]
    private bool _isOnWall;

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    [SerializeField]
    private bool _isOnCeiling;

    public bool IsOnCeiling
    {
        get
        {
            return _isOnCeiling;
        }
        set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }

    [SerializeField]
    private bool _isGrounded;

    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            // If the value is different than what it was before, interrupt ground or air states
            if (_isGrounded = true && value != true)
                animator.SetTrigger(AnimationStrings.ground_interrupt);
            else if (_isGrounded = false && value != false)
                animator.SetTrigger(AnimationStrings.air_interrupt);

            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();

        // Check that collider offset is 0,0
        if(col.offset.x != 0)
        {
            Debug.LogWarning("Recommended x offset of 0 for TouchingDirections collider on game object " + gameObject.name + ". Adjust the transform instead");
        }
    }

    public void FixedUpdate()
    {
        IsGrounded = col.Cast(Vector2.down, touchingFilter, groundHits,  groundDistance) > 0;
        IsOnWall = col.Cast(wallCheckDirection, touchingFilter, wallHits, wallDistance) > 0;

        // Wall collisions sometimes show up as ceiling collisions if BoxCollider (I guess corners could show up as either or both)
        IsOnCeiling = col.Cast(Vector2.up, touchingFilter, ceilingHits, ceilingDistance) > 0;
    }
}
