using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace DWFX
{
    public class ProgressBar_Slider : ProgressCore
    {
        [Header("Slider Settings")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private bool interactable = false;  

        [Header("Text Settings")]
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private string progressFormat = "{0:P0}";

        [Header("Fill Settings")]
        [SerializeField] private bool smoothFill = true;  
        [SerializeField] private float sliderFillSpeed = 10f; 

        private float targetSliderValue;

        protected override void Initialize()
        {
            base.Initialize();

            if (progressSlider == null)
            {
                Debug.LogError("Progress Slider is not assigned!", this);
                return;
            }

            progressSlider.minValue = minProgress;
            progressSlider.maxValue = maxProgress;
            progressSlider.value = currentProgress;
            progressSlider.interactable = interactable;

            if (interactable)
            {
                progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }

            OnProgressChanged += UpdateVisual;
            OnProgressChanged += UpdateProgressText;

            targetSliderValue = currentProgress;
            UpdateProgressText(currentProgress);
        }

        private void UpdateVisual(float progress)
        {
            if (progressSlider == null) return;

            if (smoothFill)
            {
                targetSliderValue = progress;
            }
            else
            {
                progressSlider.value = progress;
            }
        }

        private void UpdateProgressText(float progress)
        {
            if (progressText != null)
            {
                float percentage = ProgressPercentage;
                progressText.text = string.Format(progressFormat, percentage);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (smoothFill && !Mathf.Approximately(progressSlider.value, targetSliderValue))
            {
                progressSlider.value = Mathf.Lerp(progressSlider.value, targetSliderValue,
                    sliderFillSpeed * Time.deltaTime);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            if (interactable)
            {
                SetProgress(value);
            }
        }

        public override void Reset()
        {
            base.Reset();
            targetSliderValue = minProgress;
            progressSlider.value = minProgress;
            UpdateProgressText(minProgress);
        }

        public void SetInteractable(bool isInteractable)
        {
            interactable = isInteractable;
            progressSlider.interactable = isInteractable;
        }

        public void SetSmoothFill(bool smooth)
        {
            smoothFill = smooth;
            if (!smooth)
            {
                progressSlider.value = targetSliderValue;
            }
        }

        public void SetSliderFillSpeed(float speed)
        {
            sliderFillSpeed = Mathf.Max(0f, speed);
        }

        public void SetProgressFormat(string format)
        {
            progressFormat = format;
            UpdateProgressText(currentProgress);
        }

        private void OnDestroy()
        {
            OnProgressChanged -= UpdateVisual;
            OnProgressChanged -= UpdateProgressText;

            if (progressSlider != null && interactable)
            {
                progressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }
    }
}