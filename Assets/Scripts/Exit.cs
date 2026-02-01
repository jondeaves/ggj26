using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    [SerializeField] private Image fadeTarget;
    private float exitFadeTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameFinished)
        {
            return;
        }

        exitFadeTimer += Time.deltaTime;

        float fadePercentage = exitFadeTimer / (GameManager.Instance.exitFadeTime - 0.2f);
        fadeTarget.color = Color.white.WithAlpha(fadePercentage);

		if (exitFadeTimer > GameManager.Instance.exitFadeTime)
		{
			SceneManager.LoadScene("GameOverScene");
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsGameFinished && collision.CompareTag("Player"))
		{
			GameManager.Instance.PlayerWon = true;
            GameManager.Instance.IsGameFinished = true;

            if (fadeTarget == null)
			{
				SceneManager.LoadScene("GameOverScene");
                
			}

            exitFadeTimer = 0f;
		}
    }
}
