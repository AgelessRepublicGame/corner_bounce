using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace NeonUI
{
    public class NeonColor : MonoBehaviour
    {
        [Header("Color Change Settings")]
        [Tooltip("Time interval between color changes in seconds")]
        [SerializeField] private float changeInterval = 1f;
        [Tooltip("Minimum color saturation (0-1)")]
        [SerializeField] private float minSaturation = 0.95f;
        [Tooltip("Minimum color brightness (0-1)")]
        [SerializeField] private float minBrightness = 0.95f;

        private Image imageComponent;
        private WaitForSeconds waitTime;
        private float currentAlpha;

        private void OnEnable()
        {
            imageComponent = GetComponent<Image>();
            if (imageComponent == null)
            {
                Debug.LogError("No Image component found on this object!");
                enabled = false;
                return;
            }

            currentAlpha = imageComponent.color.a;

            waitTime = new WaitForSeconds(changeInterval);
            StartCoroutine(ChangeColorRoutine());
        }

        private IEnumerator ChangeColorRoutine()
        {
            while (true)
            {
                Color newColor = GenerateBrightColor();
                newColor.a = currentAlpha;
                imageComponent.color = newColor;
                yield return waitTime;
            }
        }

        private Color GenerateBrightColor()
        {
            float hue = Random.Range(0f, 1f);
            float saturation = Random.Range(minSaturation, 1f);
            float brightness = Random.Range(minBrightness, 1f);
            return Color.HSVToRGB(hue, saturation, brightness);
        }

        public void SetChangeInterval(float interval)
        {
            changeInterval = Mathf.Max(0.1f, interval);
            waitTime = new WaitForSeconds(changeInterval);
        }
    }
}