using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Score { get; private set; }
    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore()
    {
        Score++;

        UIManager.Instance.UpdateScore(Score);
        int max_score = PlayerPrefs.GetInt("Max_Score");
        if (max_score < Score) PlayerPrefs.SetInt("Max_Score", Score);
        UIManager.Instance.UpdateBestScore();
    }

    public void GameOver()
    {
        if (!IsPlaying)
            return;

        IsPlaying = false;
        UIManager.Instance.HidePlayingPanel();
        UIManager.Instance.ShowGameOver(Score);
    }

    public void RestartGame()
    {
        Score = 0;
        IsPlaying = true;

        UIManager.Instance.UpdateScore(0);
        UIManager.Instance.UpdateBestScore();
        UIManager.Instance.HideStartPanel();
        UIManager.Instance.ShowPlayingPanel();
        BouncePhysics.Instance.ResetSquare();
        UIManager.Instance.HideGameOver();
    }

    public void ReturnToStartScreen()
    {
        Score = 0;
        IsPlaying = false;

        UIManager.Instance.UpdateScore(0);
        UIManager.Instance.UpdateBestScore();
        UIManager.Instance.ShowStartPanel();
        UIManager.Instance.HidePlayingPanel();
        UIManager.Instance.HideGameOver();
        BouncePhysics.Instance.ResetSquare();
    }
}