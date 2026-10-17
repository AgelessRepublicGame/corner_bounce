using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NeonUI
{
    public class NeonColorGroup : MonoBehaviour
    {
        [Header("Color Change Settings")]
        [Tooltip("Time interval between color changes in seconds")]
        [SerializeField] private float changeInterval = 1f;
        [Tooltip("Minimum color saturation (0-1)")]
        [SerializeField] private float minSaturation = 0.95f;
        [Tooltip("Minimum color brightness (0-1)")]
        [SerializeField] private float minBrightness = 0.95f;

        [Header("Group Settings")]
        [Tooltip("Images that will change color together")]
        [SerializeField] private List<Image> groupImages = new List<Image>();

        private WaitForSeconds waitTime;

        private void Start()
        {
            waitTime = new WaitForSeconds(changeInterval);
            StartCoroutine(ChangeColorRoutine());
        }

        private IEnumerator ChangeColorRoutine()
        {
            while (true)
            {
                Color newColor = GenerateBrightColor();

                foreach (var img in groupImages)
                {
                    if (img != null)
                    {
                        img.color = newColor;
                    }
                }

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

      
        public void AddToGroup(Image image)
        {
            if (image != null && !groupImages.Contains(image))
            {
                groupImages.Add(image);
            }
        }

        public void RemoveFromGroup(Image image)
        {
            groupImages.Remove(image);
        }
    }
}