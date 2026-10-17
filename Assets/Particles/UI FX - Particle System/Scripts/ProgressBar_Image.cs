using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace DWFX
{
    public class ProgressBar_Image : ProgressCore
    {
        public enum FillDirection
        {
            Horizontal,
            Vertical
        }

        [Header("Image Settings")]
        [SerializeField] private Image fillImage;
        [SerializeField] private FillDirection fillDirection = FillDirection.Horizontal;

        [Header("Text Settings")]
        [SerializeField] private TextMeshProUGUI progressText;  
        [SerializeField] private string progressFormat = "{0:P0}";  

        [Header("Size Settings")]
        [SerializeField] private float maxWidth = 200f;
        [SerializeField] private float maxHeight = 200f;

        private RectTransform fillRectTransform;
        private Vector2 originalSizeDelta;

        protected override void Initialize()
        {
            base.Initialize();
            if (fillImage == null)
            {
                Debug.LogError("Fill Image is not assigned!", this);
                return;
            }

            fillRectTransform = fillImage.rectTransform;
            originalSizeDelta = fillRectTransform.sizeDelta;

            OnProgressChanged += UpdateVisual;
            OnProgressChanged += UpdateProgressText; 

            ResetSize();
            UpdateProgressText(currentProgress);  
        }

        private void ResetSize()
        {
            Vector2 newSize = fillRectTransform.sizeDelta;
            if (fillDirection == FillDirection.Horizontal)
            {
                newSize.x = 0;
                newSize.y = originalSizeDelta.y;
            }
            else
            {
                newSize.y = 0;
                newSize.x = originalSizeDelta.x;
            }
            fillRectTransform.sizeDelta = newSize;
        }

        private void UpdateVisual(float progress)
        {
            if (fillRectTransform == null) return;

            float progressPercentage = ProgressPercentage;
            Vector2 newSize = fillRectTransform.sizeDelta;

            if (fillDirection == FillDirection.Horizontal)
            {
                newSize.x = maxWidth * progressPercentage;
                newSize.y = originalSizeDelta.y;
            }
            else
            {
                newSize.x = originalSizeDelta.x;
                newSize.y = maxHeight * progressPercentage;
            }
            fillRectTransform.sizeDelta = newSize;
        }

        private void UpdateProgressText(float progress)
        {
            if (progressText != null)
            {
                float percentage = ProgressPercentage;
                progressText.text = string.Format(progressFormat, percentage);
            }
        }

        public override void Reset()
        {
            base.Reset();
            ResetSize();
            UpdateProgressText(currentProgress);  
        }

        public void SetFillDirection(FillDirection direction)
        {
            fillDirection = direction;
            ResetSize();
            UpdateVisual(currentProgress);
        }

        public void SetMaxSize(float size)
        {
            if (fillDirection == FillDirection.Horizontal)
            {
                maxWidth = size;
            }
            else
            {
                maxHeight = size;
            }
            UpdateVisual(currentProgress);
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
        }
    }
}