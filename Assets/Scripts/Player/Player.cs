using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Dashing, Jumping, Hurt, Death }
public enum MaskType { Mask1, Mask2 } // Mask1: Red, Mask2: Blue

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Mask Details")]
    [SerializeField] private MaskType currentMask = MaskType.Mask1;
    [SerializeField] private Color MaskColor1 = Color.red;
    [SerializeField] private Color MaskColor2 = Color.blue;
    [SerializeField] private KeyCode switchKey = KeyCode.Q;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Movement details")]
    private bool facingRight = true;
    private int facingDir = 1;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float wallSlideFallSpeed = -0.3f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1, 2);

    [Header("Collision details")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    
    private bool isGrounded;
    private bool isOnWall;
    private RaycastHit2D wallHit; // Stores the hit info for the wall in front

    private PlayerState currentState = PlayerState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        UpdateMaskVisual(); // Initialize visual state
    }

    private void Update()
    {
        HandleCollision();
        HandleInput();
        HandleState();
        HandleAnimations();
    }

    private void HandleInput()
    {
        // Switch Mask color
        if (Input.GetKeyDown(switchKey)) SwitchMask();

        // Attempt to break the wall
        if (Input.GetKeyDown(interactKey)) TryDestroyWall();

        // Jump Logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) Jump();
            else if (isOnWall && !isGrounded) WallJump();
        }

        // Wall Slide Logic
        if (!Input.GetKey(KeyCode.S) && !isGrounded && isOnWall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, wallSlideFallSpeed));
        }
    }

    private void SwitchMask()
    {
        currentMask = (currentMask == MaskType.Mask1) ? MaskType.Mask2 : MaskType.Mask1;
        UpdateMaskVisual();
    }

    private void UpdateMaskVisual()
    {
        // Provide visual feedback for the current mask
        sr.color = (currentMask == MaskType.Mask1) ? MaskColor1 : MaskColor2;
    }

    private void TryDestroyWall()
    {
        // Only proceed if HandleCollision detects a wall
        if (isOnWall && wallHit.collider != null)
        {
            string wallTag = wallHit.collider.tag;
            bool canBreak = false;

            // Logic: Red Mask (Mask1) breaks Blue Walls, Blue Mask (Mask2) breaks Red Walls
            if (currentMask == MaskType.Mask1 && wallTag == "Wall2") canBreak = true;
            if (currentMask == MaskType.Mask2 && wallTag == "Wall1") canBreak = true;

            if (canBreak)
            {
                DestructableWall wallScript = wallHit.collider.GetComponent<DestructableWall>();
                if (wallScript != null)
                {
                    wallScript.TriggerDestruction();
                    
                    // Disable the main collider so it no longer blocks the player
                    wallHit.collider.enabled = false; 
                    Debug.Log("Wall destroyed via opposite color!");
                }
            }
            else
            {
                Debug.Log("Wrong color mask! Cannot break this wall.");
                // Placeholder for alerting the enemy
            }
        }
    }


    private void HandleState()
    {
        switch (currentState)
        {
            case PlayerState.Idle: 
                HorizontalMove(); 
                break;
            case PlayerState.Jumping:
                HorizontalMove();
                // Return to idle once landed
                if (isGrounded && rb.linearVelocity.y <= 0.1f) currentState = PlayerState.Idle;
                break;
        }
    }

    private void HorizontalMove() => rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

    private void Jump()
    {
        currentState = PlayerState.Jumping;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void WallJump()
    {
        currentState = PlayerState.Jumping;
        Vector2 force = new Vector2(wallJumpDirection.x * -facingDir, wallJumpDirection.y);
        rb.linearVelocity = new Vector2(force.x * jumpForce, force.y * jumpForce);
    }

    private void HandleAnimations()
    {
        if (anim == null) return;
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isOnWall", isOnWall);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
    }

    private void HandleCollision()
    {
        // Check if grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);

        // Check if touching a wall using two raycast
        RaycastHit2D primaryWallHitCheck = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        RaycastHit2D secondaryWallHitCheck = Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        wallHit = primaryWallHitCheck.collider ? primaryWallHitCheck : secondaryWallHitCheck;

        isOnWall = wallHit.collider != null;
    }

    private void OnDrawGizmos()
    {
        // Visual debug lines in the Scene view
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}
