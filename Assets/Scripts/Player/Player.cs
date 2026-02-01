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

    [Header("Collision details")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    
    private bool isGrounded;
    private bool isOnWall;
    private RaycastHit2D wallHit; // Stores the hit info for the wall in front

    private PlayerState currentState = PlayerState.Idle;

    [Header("Knockback")]
    [SerializeField] private float knockbackTime = 0.5f;
    private float knockbackTimer = 0f;

    [SerializeField] private float knockbackDistance = 3f;
    private Vector2 knockbackTarget;

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
        if (currentState == PlayerState.Hurt)
            return;

        // Switch Mask color
        if (Input.GetKeyDown(switchKey)) SwitchMask();

        // Attempt to break the wall
        if (Input.GetKeyDown(interactKey)) TryDestroyWall();

        // Jump Logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) Jump();
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
                // Return to idle once landed - simplified for gravity compatibility
                if (isGrounded) currentState = PlayerState.Idle;
                break;
            case PlayerState.Hurt:
                knockbackTimer += Time.deltaTime;
                if (knockbackTimer >= knockbackTime)
                {
                    FinishKnockback();
                }

                // TODO: Add some vertical
                transform.position = Vector2.Lerp(transform.position, knockbackTarget, (knockbackTime / 2) * Time.deltaTime);
				break;
        }

        gameObject.GetComponentInChildren<SpriteRenderer>().flipY = Physics2D.gravity.y > 0;
    }

    private void HorizontalMove() => rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

    private void Jump()
    {
        currentState = PlayerState.Jumping;
        // Adjust jump direction based on current gravity
        float gravityDirection = Mathf.Sign(Physics2D.gravity.y);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * -gravityDirection);
    }

    private void HandleAnimations()
    {
        if (anim == null) return;
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isOnWall", isOnWall);
        anim.SetFloat("yVelocity", Mathf.Abs(rb.linearVelocity.y)); // Use absolute value for animator
        anim.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
    }

    private void HandleCollision()
    {
        // Check if grounded - dynamically adjust ray direction based on gravity
        Vector2 downDirection = Physics2D.gravity.y < 0 ? Vector2.down : Vector2.up;
        // Added 0.1f buffer to ensure detection when pressed against ceiling
        isGrounded = Physics2D.Raycast(transform.position, downDirection, groundCheckDistance + 0.1f, whatIsGround);


        // Check if we collide with any walls that should knock us back
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, whatIsWall);
        isOnWall = wallHit.collider != null;

        if (isOnWall && currentState != PlayerState.Hurt)
        {
            TriggerKnockback();
        }
    }

    private void TriggerKnockback()
	{
		float gravityDirection = Mathf.Sign(Physics2D.gravity.y);

		currentState = PlayerState.Hurt;
        rb.linearVelocity = new Vector2(0, (jumpForce / 3) * -gravityDirection);
        
        knockbackTimer = 0f;
        knockbackTarget = new Vector2(transform.position.x - knockbackDistance, transform.position.y);

        // TODO: Hurt animation
	}

    private void FinishKnockback()
    {
        currentState = PlayerState.Idle;
        
        // TODO: Running animation
    }

    private void OnDrawGizmos()
    {
        // Visual debug lines in the Scene view
        Gizmos.color = Color.yellow;

        // Corrected Gizmo direction to match gravity
        float gravityDir = Physics2D.gravity.y < 0 ? -1f : 1f;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, groundCheckDistance * gravityDir));
    }
}
