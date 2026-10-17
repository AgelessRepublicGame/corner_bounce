using UnityEngine;
using UnityEngine.UI;

public class PlayingPanel : MonoBehaviour
{
    public Button homeBtn;

    private void Start()
    {
        if (homeBtn != null)
            homeBtn.onClick.AddListener(ReturnToStartScreen);
    }

    public void ReturnToStartScreen()
    {
        GameManager.Instance.ReturnToStartScreen();
    }
}
