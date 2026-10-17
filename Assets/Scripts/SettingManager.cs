using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public const float MinPlayAreaPaddingPixels = 0f;
    public const float MaxPlayAreaPaddingPixels = 60f;

    public static SettingsManager Instance;

    public bool SoundEnabled = true;

    public float PlayAreaPaddingPixels = 0f;

    private void Awake()
    {
        Instance = this;
        LoadSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("SoundEnabled", SoundEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("PlayAreaPadding", PlayAreaPaddingPixels);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        SoundEnabled = PlayerPrefs.GetInt( "SoundEnabled", 1) == 1;

        PlayAreaPaddingPixels = ClampPadding(PlayerPrefs.GetFloat( "PlayAreaPadding", 0f));
    }

    public void SetSound(bool value)
    {
        SoundEnabled = value;
        SaveSettings();
    }

    public void SetPadding(float value)
    {
        PlayAreaPaddingPixels = ClampPadding(value);
        SaveSettings();
        BouncePhysics.Instance?.RecalculateBounds();
    }

    private static float ClampPadding(float value)
    {
        return Mathf.Clamp(
            Mathf.Round(value),
            MinPlayAreaPaddingPixels,
            MaxPlayAreaPaddingPixels);
    }
}