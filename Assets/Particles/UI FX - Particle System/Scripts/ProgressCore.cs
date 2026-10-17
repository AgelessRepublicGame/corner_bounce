using UnityEngine;
using System;


namespace DWFX
{
    public class ProgressCore : MonoBehaviour
    {
        [Header("Progress Settings")]
        [SerializeField] public float minProgress = 0f;
        [SerializeField] public float maxProgress = 100f;
        [SerializeField] public float fillSpeed = 5f;

        protected float currentProgress;
        protected float targetProgress;

        public float CurrentProgress => currentProgress;
        public float ProgressPercentage => (currentProgress - minProgress) / (maxProgress - minProgress);
        public bool IsCompleted => Mathf.Approximately(currentProgress, maxProgress);

        public event Action<float> OnProgressChanged;
        public event Action OnProgressComplete;

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            currentProgress = minProgress;
            targetProgress = minProgress;
        }

        protected virtual void Update()
        {
            UpdateProgress();
        }

        protected virtual void UpdateProgress()
        {
            if (!Mathf.Approximately(currentProgress, targetProgress))
            {
                float previousProgress = currentProgress;
                currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, fillSpeed * Time.deltaTime);

                if (!Mathf.Approximately(previousProgress, currentProgress))
                {
                    OnProgressChanged?.Invoke(currentProgress);

                    if (IsCompleted)
                    {
                        OnProgressComplete?.Invoke();
                    }
                }
            }
        }

        public virtual void SetProgress(float progress)
        {
            float previousProgress = currentProgress;
            currentProgress = Mathf.Clamp(progress, minProgress, maxProgress);
            targetProgress = currentProgress;

            if (!Mathf.Approximately(previousProgress, currentProgress))
            {
                OnProgressChanged?.Invoke(currentProgress);

                if (IsCompleted)
                {
                    OnProgressComplete?.Invoke();
                }
            }
        }

        public virtual void SetTargetProgress(float target)
        {
            targetProgress = Mathf.Clamp(target, minProgress, maxProgress);
        }

        public virtual void AddProgress(float amount)
        {
            SetProgress(currentProgress + amount);
        }

        public virtual void Reset()
        {
            SetProgress(minProgress);
        }

        public virtual float GetRemainingProgress()
        {
            return maxProgress - currentProgress;
        }

        public virtual void SetFillSpeed(float speed)
        {
            fillSpeed = Mathf.Max(0f, speed);
        }
    }
}