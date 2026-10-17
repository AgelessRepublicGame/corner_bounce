using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Button restartBtn;

    private void Start()
    {
        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartGame);
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
