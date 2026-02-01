using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player playerScript;
    private SpriteRenderer sr;

    [Header("Movement Settings")]
    [SerializeField] private float offScreenXOffset = 15f; 
    [SerializeField] private float warningXOffset = 4f;   // Distance behind player when warning
    [SerializeField] private float chaseSpeed = 15f;      
    [SerializeField] private float retreatSpeed = 5f;     

    [Header("Punishment Settings")]
    [SerializeField] private float warningDuration = 5.0f; // Time before enemy retreats
    [SerializeField] private float fumbleCooldown = 1.5f;  // Invincibility window after a mistake
    
    private float warningTimer = 0f;
    private float cooldownTimer = 0f; 
    private int penaltyLevel = 0; // 0: Hidden, 1: Warning/Chasing, 2: Caught

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (playerScript == null) playerScript = Object.FindFirstObjectByType<Player>();
        
        // Ensure enemy starts hidden
        SetEnemyVisibility(false);
    }

    void Update()
    {
        if (playerScript == null) return;

        HandleTimers();
        HandleLogic();
        MoveEnemy();
    }

    private void HandleTimers()
    {
        // Countdown for how long the enemy stays on screen
        if (warningTimer > 0)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0 && penaltyLevel == 1)
            {
                penaltyLevel = 0; // Time's up, retreat
            }
        }

        // Countdown for the invincibility/cooldown period
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleLogic()
    {
        // Trigger mistake logic only if player is Hurt and NOT in cooldown
        if (playerScript.CurrentState == PlayerState.Hurt && cooldownTimer <= 0)
        {
            OnPlayerFumble();
        }
    }

    private void OnPlayerFumble()
    {
        if (penaltyLevel == 0)
        {
            // First Fumble: Summon the enemy to chase
            penaltyLevel = 1;
            warningTimer = warningDuration;
            cooldownTimer = fumbleCooldown; // Start invincibility window
            SetEnemyVisibility(true);
            Debug.Log("Warning: Enemy summoned. Invincibility active.");
        }
        else if (penaltyLevel == 1)
        {
            // Second Fumble: Catch the player (only triggers if cooldown has expired)
            penaltyLevel = 2;
            OnCaught();
        }
    }

    private void MoveEnemy()
    {
        float targetXOffset;

        if (penaltyLevel == 2) 
            targetXOffset = 0f; // Catching position
        else if (penaltyLevel == 1) 
            targetXOffset = -warningXOffset; // Warning position
        else 
            targetXOffset = -offScreenXOffset; // Hidden position

        // Calculate target position based on Player's current position and gravity-ready Y
        Vector3 targetPos = new Vector3(playerScript.transform.position.x + targetXOffset, playerScript.transform.position.y, 0);
        
        float currentSpeed = (penaltyLevel > 0) ? chaseSpeed : retreatSpeed;
        
        // Smoothly interpolate towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

        // Turn off visibility once retreated far enough
        if (penaltyLevel == 0 && Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            SetEnemyVisibility(false);
        }
    }

    private void SetEnemyVisibility(bool visible)
    {
        if (sr != null) sr.enabled = visible;
    }

    private void OnCaught()
    {
        GameManager.Instance.PlayerWon = false;
        SceneManager.LoadScene("GameOverScene");
    }
}
