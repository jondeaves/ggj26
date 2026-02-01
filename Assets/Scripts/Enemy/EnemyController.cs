using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player playerScript;
    private SpriteRenderer sr;

    [Header("Distance Settings")]
    [SerializeField] private float normalXOffset = 10f; 
    [SerializeField] private float warningXOffset = 3f;   
    
    [Header("Speed Settings")]
    [SerializeField] private float constantSpeed = 30f;      

    [Header("Punishment Settings")]
    [SerializeField] private float warningDuration = 5.0f; 
    [SerializeField] private float fumbleCooldown = 2.0f;  
    
    private float warningTimer = 0f;
    private float cooldownTimer = 0f; 
    private int penaltyLevel = 0; 
    private float lockedYPosition;
    private float xVelocity = 0.0f;

    private float delayTimer = 0f;

	[SerializeField] private Image fadeTarget;
	private float deathTimer = 0f;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (playerScript == null) playerScript = Object.FindFirstObjectByType<Player>();
        
        lockedYPosition = transform.position.y;
        
        SetEnemyVisibility(true);
        if (playerScript != null)
        {
            transform.position = new Vector3(playerScript.transform.position.x - normalXOffset, lockedYPosition, 0);
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameFinished && !GameManager.Instance.PlayerWon && fadeTarget != null) {
            //
            //SceneManager.LoadScene("GameOverScene");
            deathTimer += Time.deltaTime;

			float fadePercentage = deathTimer / (GameManager.Instance.exitFadeTime - 0.2f);
			fadeTarget.color = Color.black.WithAlpha(fadePercentage);

			if (deathTimer > GameManager.Instance.exitFadeTime)
			{
				SceneManager.LoadScene("GameOverScene");
			}
		}
		if (playerScript == null || GameManager.Instance.IsGameFinished) return;


        delayTimer += Time.deltaTime;
        if (delayTimer < GameManager.Instance.GameStartDelayTime)
        {
            return;
        }

		HandleTimers();
        HandleLogic();
        MoveEnemy();
    }

    private void HandleTimers()
    {
        if (warningTimer > 0)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0 && penaltyLevel == 1)
            {
                penaltyLevel = 0; 
                if (SoundManager.Instance != null) SoundManager.Instance.PlayNormalBGM();
            }
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleLogic()
    {
        if (playerScript.CurrentState == PlayerState.Hurt && cooldownTimer <= 0)
        {
            OnPlayerFumble();
        }
    }

    private void OnPlayerFumble()
    {
        if (penaltyLevel == 0)
        {
            penaltyLevel = 1;
            warningTimer = warningDuration;
            cooldownTimer = fumbleCooldown;
            Debug.Log("Penalty Level 1: Maintaining constant speed 25.");
            if (SoundManager.Instance != null) SoundManager.Instance.PlayWarningBGM();
        }
        else if (penaltyLevel == 1)
        {
            penaltyLevel = 2;
            OnCaught();
        }
    }
    private void MoveEnemy()
    {
        float playerX = playerScript.transform.position.x;
        float currentTargetOffset;

        if (penaltyLevel == 2) 
            currentTargetOffset = 0f; 
        else if (penaltyLevel == 1) 
            currentTargetOffset = warningXOffset; 
        else 
            currentTargetOffset = normalXOffset; 

        float targetX = playerX - currentTargetOffset;
        
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref xVelocity, 0.3f, constantSpeed);
        transform.position = new Vector3(newX, lockedYPosition, transform.position.z);
    }

    private void SetEnemyVisibility(bool visible)
    {
        if (sr != null) sr.enabled = visible;
    }

    private void OnCaught()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayGameOverBGM();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerWon = false;
            GameManager.Instance.IsGameFinished = true;
		}
    }
}


