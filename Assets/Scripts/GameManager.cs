using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool PlayerWon = false;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this new one to enforce the singleton pattern
            Destroy(this.gameObject);
            return; // Exit to prevent further execution for this duplicate object
        }

        // If no instance exists, set this object as the instance
        Instance = this;

        // Mark the GameObject to not be destroyed when a new scene loads
        DontDestroyOnLoad(this.gameObject);
    }
}
