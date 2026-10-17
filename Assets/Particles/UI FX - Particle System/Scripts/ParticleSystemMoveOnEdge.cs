using UnityEngine;


namespace DWFX
{
    public class ParticleSystemMoveOnEdge : MonoBehaviour
    {
        public enum ShapeType
        {
            Circle,
            Rectangle
        }

        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }

        [Header("Move Settings")]
        public ShapeType currentShape = ShapeType.Circle;
        public RotationDirection rotationDirection = RotationDirection.Clockwise;
        public float cycleTime = 2f; // Time to complete one full cycle

        [Header("Sub Effect Settings")]
        public GameObject subEffectPrefab; 
        public bool enableSubEffect = false; 

        private ParticleSystem targetParticleSystem;
        private ParticleSystem.Particle[] particles;
        [Header("Select Rectangle Shap ,Will Get Value From Shap Scale")]
        [SerializeField]
        private Vector2 rectangleSize;
        [Header("Select Circle Shap ,Will Get Value From Shap Radius")]
        [SerializeField]
        private float circleRadius;
        private GameObject[] subEffects;

        private void Awake()
        {
            targetParticleSystem = GetComponent<ParticleSystem>();
            if (targetParticleSystem == null)
            {
                Debug.LogError("No ParticleSystem component found!");
                return;
            }

            // 隐藏 Trails 模块
            var trails = targetParticleSystem.trails;
            trails.enabled = false;

            // Get circle radius from ParticleSystem shape
            var shape = targetParticleSystem.shape;
            circleRadius = shape.radius;


            particles = new ParticleSystem.Particle[targetParticleSystem.main.maxParticles];

            rectangleSize = new Vector2(shape.scale.x, shape.scale.y);
            enableSubEffect = (subEffectPrefab != null);

            Canvas canvas = transform.GetComponentInParent<Canvas>();
            //  circleRadius  = circleRadius / canvas.transform.localScale.x;
            //  rectangleSize = rectangleSize / canvas.transform.localScale.x;

            // 初始化子特效数组
            if (enableSubEffect && subEffectPrefab != null)
            {
                subEffects = new GameObject[targetParticleSystem.main.maxParticles];
                for (int i = 0; i < subEffects.Length; i++)
                {
                    subEffects[i] = Instantiate(subEffectPrefab, transform);
                    subEffects[i].SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            particlesInitialized = false;
            var trails = targetParticleSystem.trails;
            trails.enabled = false;

            if (enableSubEffect && subEffects != null)
            {
                foreach (GameObject subEffect in subEffects)
                {
                    if (subEffect != null)
                        subEffect.SetActive(false);
                }
            }
        }

        private bool particlesInitialized = false;

        private void Update()
        {
            if (!particlesInitialized && targetParticleSystem.isPlaying)
            {
                int particleCount = targetParticleSystem.GetParticles(particles);
                if (particleCount > 0)
                {
                    InitializeParticlesOnEdge();
                    particlesInitialized = true;
                    var trails = targetParticleSystem.trails;
                    trails.enabled = true;
                }
            }

            if (particlesInitialized)
            {
                UpdateParticlePositions();
            }
        }

        // 以下方法保持原样，包括 InitializeParticlesOnEdge(), MoveParticleAlongCircle(), 
        // MoveParticleAlongRectangle(), GetRectangleEdgePoint(), GetProgressOnRectangle() 方法

        private void UpdateParticlePositions()
        {
            int particleCount = targetParticleSystem.GetParticles(particles);

            float deltaProgress = Time.deltaTime / cycleTime;
            if (rotationDirection == RotationDirection.CounterClockwise)
            {
                deltaProgress = -deltaProgress;
            }

            for (int i = 0; i < particleCount; i++)
            {
                Vector3 previousPosition = particles[i].position;

                switch (currentShape)
                {
                    case ShapeType.Circle:
                        MoveParticleAlongCircle(ref particles[i], deltaProgress);
                        break;
                    case ShapeType.Rectangle:
                        MoveParticleAlongRectangle(ref particles[i], deltaProgress);
                        break;
                }

                if (enableSubEffect && subEffectPrefab != null && subEffects != null && i < subEffects.Length)
                {
                    UpdateSubEffect(subEffects[i], particles[i].position, previousPosition);
                }
            }

            targetParticleSystem.SetParticles(particles, particleCount);
        }
        private void UpdateSubEffect(GameObject subEffect, Vector3 currentPosition, Vector3 previousPosition)
        {
            if (subEffect == null) return;

            RectTransform rectTransform = subEffect.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float pixelsPerUnit = GetPixelsPerUnit(rectTransform);

                rectTransform.anchoredPosition3D = new Vector3(
                    currentPosition.x * pixelsPerUnit,
                    currentPosition.y * pixelsPerUnit, 0
                );

                Vector3 movementDirection = (currentPosition - previousPosition).normalized;
                if (movementDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                    rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
                }
            }
            else
            {
                subEffect.transform.localPosition = currentPosition;
            }

            subEffect.SetActive(true);
        }

        private float GetPixelsPerUnit(RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                return 1f / canvas.transform.localScale.x;
            }

            return 100f;
        }
        private void InitializeParticlesOnEdge()
        {
            int particleCount = targetParticleSystem.GetParticles(particles);

            if (particleCount == 0)
            {
                Debug.LogWarning("No particles found. Check Particle System settings.");
                return;
            }

            for (int i = 0; i < particleCount; i++)
            {
                float progress = (float)i / particleCount;

                if (currentShape == ShapeType.Circle)
                {
                    float angle = progress * 2 * Mathf.PI;
                    particles[i].position = new Vector3(
                        Mathf.Cos(angle) * circleRadius,
                        Mathf.Sin(angle) * circleRadius,
                        particles[i].position.z
                    );
                }
                else // Rectangle
                {
                    Vector2 edgePoint = GetRectangleEdgePoint(progress);
                    particles[i].position = new Vector3(
                        edgePoint.x,
                        edgePoint.y,
                        particles[i].position.z
                    );
                }

                if (enableSubEffect && subEffectPrefab != null && subEffects != null && i < subEffects.Length)
                {
                    subEffects[i].transform.position = particles[i].position;
                    subEffects[i].SetActive(true);
                }
            }

            targetParticleSystem.SetParticles(particles, particleCount);
        }

        private Vector2 GetRectangleEdgePoint(float progress)
        {
            float perimeter = 2 * (rectangleSize.x + rectangleSize.y);
            float distance = progress * perimeter;

            if (distance < rectangleSize.x) 
            {
                return new Vector2(distance - rectangleSize.x / 2, -rectangleSize.y / 2);
            }
            else if (distance < rectangleSize.x + rectangleSize.y) 
            {
                return new Vector2(rectangleSize.x / 2, (distance - rectangleSize.x) - rectangleSize.y / 2);
            }
            else if (distance < 2 * rectangleSize.x + rectangleSize.y) 
            {
                return new Vector2(rectangleSize.x / 2 - (distance - (rectangleSize.x + rectangleSize.y)), rectangleSize.y / 2);
            }
            else // 左边
            {
                return new Vector2(-rectangleSize.x / 2, rectangleSize.y / 2 - (distance - (2 * rectangleSize.x + rectangleSize.y)));
            }
        }
        private float GetProgressOnRectangle(Vector3 position)
        {
            float perimeter = 2 * (rectangleSize.x + rectangleSize.y);
            Vector2 halfSize = rectangleSize * 0.5f;

            if (Mathf.Approximately(position.y, -halfSize.y)) 
            {
                return (position.x + halfSize.x) / perimeter;
            }
            else if (Mathf.Approximately(position.x, halfSize.x)) 
            {
                return (rectangleSize.x + (position.y + halfSize.y)) / perimeter;
            }
            else if (Mathf.Approximately(position.y, halfSize.y)) 
            {
                return (rectangleSize.x + rectangleSize.y + (halfSize.x - position.x)) / perimeter;
            }
            else // 左边
            {
                return (2 * rectangleSize.x + rectangleSize.y + (halfSize.y - position.y)) / perimeter;
            }
        }

        private void MoveParticleAlongCircle(ref ParticleSystem.Particle particle, float deltaProgress)
        {
            Vector2 particlePosition2D = new Vector2(particle.position.x, particle.position.y);
            float angle = Mathf.Atan2(particlePosition2D.y, particlePosition2D.x);

            angle += deltaProgress * 2 * Mathf.PI;

            particle.position = new Vector3(
                Mathf.Cos(angle) * circleRadius,
                Mathf.Sin(angle) * circleRadius,
                particle.position.z
            );
        }

        private void MoveParticleAlongRectangle(ref ParticleSystem.Particle particle, float deltaProgress)
        {
            float progress = GetProgressOnRectangle(particle.position);
            progress += deltaProgress;

            if (progress >= 1f) progress -= 1f;
            if (progress < 0f) progress += 1f;

            Vector2 newPos = GetRectangleEdgePoint(progress);
            particle.position = new Vector3(newPos.x, newPos.y, particle.position.z);
        }
        private void OnDisable()
        {
            if (enableSubEffect && subEffects != null)
            {
                foreach (GameObject subEffect in subEffects)
                {
                    if (subEffect != null)
                        subEffect.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            if (enableSubEffect && subEffects != null)
            {
                foreach (GameObject subEffect in subEffects)
                {
                    if (subEffect != null)
                        Destroy(subEffect);
                }
            }
        }
    }
}