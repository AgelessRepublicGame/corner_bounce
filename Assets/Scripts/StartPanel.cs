using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    public Button startButton;
    public Button modeSelectButton;
    public Button customizeButton;
    public Button scoresButton;
    public Button settingButton;
    public GameObject backgroundObj;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(ShowSettingPanel);
        }    
    }

    public void StartGame()
    {
        GameManager.Instance.RestartGame();
        gameObject.SetActive(false);
        backgroundObj.SetActive(false);
    }

    public void startPlayButtonClicked()
    {
        StartGame();
    }

    public void ShowStartPanel()
    {
        gameObject.SetActive(true);
        backgroundObj.SetActive(true);
    }

    public void HideStartPanel()
    {
        gameObject.SetActive(false);
        backgroundObj.SetActive(false);
    }

    public void ShowSettingPanel()
    {
        UIManager.Instance.settingsPanel.SetActive(true);
    }
}
