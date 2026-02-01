using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource audioSource;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip normalBGM;    
    [SerializeField] private AudioClip warningBGM; 
    [SerializeField] private AudioClip gameOverBGM;
    [SerializeField] private AudioClip victoryBGM;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayNormalBGM();
    }


    public void PlayNormalBGM()
    {
        if (audioSource.clip != normalBGM) SwitchBGM(normalBGM, true);
    }

    public void PlayWarningBGM()
    {
        if (audioSource.clip != warningBGM) SwitchBGM(warningBGM, true);
        audioSource.pitch = 1.2f;
    }

    public void PlayGameOverBGM()
    {
        SwitchBGM(gameOverBGM, false);
    }

    public void PlayVictoryBGM()
    {
        SwitchBGM(victoryBGM, false);
    }

    private void SwitchBGM(AudioClip clip, bool loop)
    {
        if (clip == null) return;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }
}
