using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private bool holdingDown = false;

    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        if (GameManager.Instance.PlayerWon)
        {
            resultText.text = "You Win!";
        }
        else
        {
            resultText.text = "Game Over";
        }
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
