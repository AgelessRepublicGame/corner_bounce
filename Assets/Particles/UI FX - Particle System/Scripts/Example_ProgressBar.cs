using UnityEngine;


namespace DWFX
{
    public class Example_ProgressBar : MonoBehaviour
    {
        private ProgressCore progressCore;

        [Header("Auto Loop Settings")]
        [SerializeField] private bool autoLoop = false;  
        private bool isIncreasing = true;  
        [SerializeField] private float autoChangeSpeed = 20f;  

        private void Awake()
        {
            progressCore = GetComponent<ProgressCore>();

            progressCore.OnProgressChanged += OnProgressChanged;
            progressCore.OnProgressComplete += OnProgressComplete;
        }

        private void OnProgressChanged(float progress)
        {
            //Debug.Log($"Progress changed to: {progress:F2}, Percentage: {progressCore.ProgressPercentage:P2}");
        }

        private void OnProgressComplete()
        {
          //  Debug.Log("Progress Complete!");
        }

        private void Update()
        {
            if (autoLoop)
            {
                if (isIncreasing)
                {
                    progressCore.AddProgress(autoChangeSpeed * Time.deltaTime);
                    if (Mathf.Approximately(progressCore.CurrentProgress, progressCore.maxProgress))
                    {
                        isIncreasing = false;
                    }
                }
                else
                {
                    progressCore.AddProgress(-autoChangeSpeed * Time.deltaTime);
                    if (Mathf.Approximately(progressCore.CurrentProgress, progressCore.minProgress))
                    {
                        isIncreasing = true;
                    }
                }
            }

          
        }

        private void OnDestroy()
        {
            if (progressCore != null)
            {
                progressCore.OnProgressChanged -= OnProgressChanged;
                progressCore.OnProgressComplete -= OnProgressComplete;
            }
        }
    }
}