using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;

    public StartPanel startPanel;
    public GameObject playingPanel;
    public GameObject gameOverPanel;
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip cornerSound;

    [Header("Particles")]
    public ParticleSystem cornerParticlePrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource.PlayOneShot(cornerSound);
        UpdateScore(0);
        UpdateBestScore();
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
        playingPanel.SetActive(false);
        startPanel.ShowStartPanel();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateBestScore()
    {
        int max_score = PlayerPrefs.GetInt("Max_Score");
        bestScoreText.text = max_score.ToString();
    }

    public void ShowPlayingPanel()
    {
        playingPanel.SetActive(true);
    }

    public void HidePlayingPanel()
    {
        playingPanel.SetActive(false);
    }

    public void ShowStartPanel()
    {
        startPanel.ShowStartPanel();
    }

    public void HideStartPanel()
    {
        startPanel.HideStartPanel();
    }

    public void ShowGameOver(int score)
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score : " + score.ToString();
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void PlayCornerSound()
    {
        audioSource.PlayOneShot(cornerSound);
    }

    public void SpawnCornerParticles(Vector2 position)
    {
        if (cornerParticlePrefab != null)
        {
            Instantiate(cornerParticlePrefab, position, Quaternion.identity);
            return;
        }
        Debug.Log("you don't have corner particle");
    }
}