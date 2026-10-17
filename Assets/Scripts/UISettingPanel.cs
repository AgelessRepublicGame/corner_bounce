using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingPanel : MonoBehaviour
{
    public Toggle SoundTgl;
    public Slider areaSlider;
    public TMP_Text areaPaddingTxt;

    public Button closeBtn;

    void Start()
    {
        areaSlider.minValue = SettingsManager.MinPlayAreaPaddingPixels;
        areaSlider.maxValue = SettingsManager.MaxPlayAreaPaddingPixels;
        areaSlider.wholeNumbers = true;

        // Load saved values into UI
        SoundTgl.isOn = SettingsManager.Instance.SoundEnabled;
        areaSlider.SetValueWithoutNotify(SettingsManager.Instance.PlayAreaPaddingPixels);

        UpdatePaddingText(areaSlider.value);

        // Register UI events
        SoundTgl.onValueChanged.AddListener(OnSoundChanged);
        areaSlider.onValueChanged.AddListener(OnPaddingChanged);
        closeBtn.onClick.AddListener(OnClosePanel);
    }

    private void OnClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void OnSoundChanged(bool value)
    {
        SettingsManager.Instance.SetSound(value);

        // Optional: immediately mute/unmute game audio
        AudioListener.volume = value ? 1f : 0f;
    }

    private void OnPaddingChanged(float value)
    {
        SettingsManager.Instance.SetPadding(value);
        areaSlider.SetValueWithoutNotify(SettingsManager.Instance.PlayAreaPaddingPixels);
        UpdatePaddingText(SettingsManager.Instance.PlayAreaPaddingPixels);
    }
    private void UpdatePaddingText(float value)
    {
        areaPaddingTxt.text = $"{Mathf.RoundToInt(value)}px";
    }

    private void OnDestroy()
    {
        SoundTgl.onValueChanged.RemoveListener(OnSoundChanged);
        areaSlider.onValueChanged.RemoveListener(OnPaddingChanged);
        closeBtn.onClick.RemoveListener(OnClosePanel);
    }
}
