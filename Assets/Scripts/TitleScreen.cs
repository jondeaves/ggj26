using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private bool holdingDown = false;

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
            SceneManager.LoadScene("GameplayScene");
        }
    }
}
