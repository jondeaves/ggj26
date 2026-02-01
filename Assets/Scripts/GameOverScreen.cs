using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private bool holdingDown = false;

    [SerializeField] private GameObject VictoryScreen;
	[SerializeField] private GameObject LossScreen;

	private void Start()
	{
        if (VictoryScreen != null)
		    VictoryScreen.SetActive(GameManager.Instance.PlayerWon);
            
        if (LossScreen != null)
            LossScreen.SetActive(!GameManager.Instance.PlayerWon);

        GameManager.Instance.PlayerWon = false;
		GameManager.Instance.IsGameFinished = false;

	}

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            holdingDown = true;
        }

        if (!Input.anyKey && holdingDown)
        {
            holdingDown = false;
            SceneManager.LoadScene("TitleScene");
        }
        

    }
}
