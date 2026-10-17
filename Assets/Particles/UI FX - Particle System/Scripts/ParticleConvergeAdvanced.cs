using UnityEngine;
using System.Collections.Generic;


namespace DWFX
{
    public class ParticleConvergeAdvanced : MonoBehaviour
    {
        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;
        private ParticleSystem.MainModule mainModule;

        [Header("Basic Settings")]
        public Transform target;

        [Header("Time Control")]
        public float convergenceDuration = 2f;    // Time needed to complete convergence
        public float convergenceStartTime = 2f;   // Time to start convergence
        private float timeElapsed = 0f;           // Time elapsed
        private bool isConverging = false;        // Whether converging
        private bool hasFinished = false;         // Whether convergence has finished

        [Header("Basic Settings")]
        [Range(0f, 10f)]
        public float arrivalDistance = 0.5f;      // Distance threshold to target point

        [Header("Effect Settings")]
        public bool useSpiral = true;             // Whether to use spiral motion
        public float spiralRadius = 2f;           // Spiral radius
        public float spiralSpeed = 2f;            // Spiral speed

        [Header("Finish Settings")]
        public float deactivateDelay = 1f;        // Delay time before deactivating gameObject

        private Dictionary<uint, Vector3> particleStartPositions;
        private Dictionary<uint, float> particleStartTimes;
        private Dictionary<uint, float> particleAngles;
        private Dictionary<uint, bool> particleInitialized;

        void OnEnable()
        {
            if (!ValidateTarget())
            {
                Debug.LogError("ParticleConvergeAdvanced requires either a target Transform or a parent Transform!");
                enabled = false;
                return;
            }
            Initialize();
        }

        private bool ValidateTarget()
        {
            if (target == null)
            {
                // Try to get parent if target is null
                Transform parentTransform = transform.parent;
                if (parentTransform != null)
                {
                    target = parentTransform;
                    Debug.LogWarning("Target not set, using parent Transform as target.");
                    return true;
                }
                return false;
            }
            return true;
        }

        void Initialize()
        {
            ps = GetComponent<ParticleSystem>();
            mainModule = ps.main;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.Custom;

            particleStartPositions = new Dictionary<uint, Vector3>();
            particleStartTimes = new Dictionary<uint, float>();
            particleAngles = new Dictionary<uint, float>();
            particleInitialized = new Dictionary<uint, bool>();

            timeElapsed = 0f;
            isConverging = false;
            hasFinished = false;

            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }

        void LateUpdate()
        {
            if (ps == null || hasFinished) return;

            // Revalidate target in case it was destroyed during runtime
            if (!ValidateTarget())
            {
                Debug.LogError("Target and parent are missing! Disabling ParticleConvergeAdvanced.");
                enabled = false;
                return;
            }

            timeElapsed += Time.deltaTime;

            if (!isConverging && timeElapsed >= convergenceStartTime)
            {
                isConverging = true;
            }

            if (!isConverging)
            {
                return;
            }

            if (particles == null || particles.Length < ps.main.maxParticles)
            {
                particles = new ParticleSystem.Particle[ps.main.maxParticles];
            }

            int numParticlesAlive = ps.GetParticles(particles);
            bool allParticlesArrived = true;

            var main = ps.main; // Get ParticleSystem's MainModule
                                // Get particle system's world space transformation
            Matrix4x4 customSimulationSpaceMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            main.customSimulationSpace = transform;

            for (int i = 0; i < numParticlesAlive; i++)
            {
                if (!UpdateParticle(ref particles[i], customSimulationSpaceMatrix))
                {
                    allParticlesArrived = false;
                }
            }

            ps.SetParticles(particles, numParticlesAlive);

            if (allParticlesArrived)
            {
                FinishConvergence();
            }
        }

        bool UpdateParticle(ref ParticleSystem.Particle particle, Matrix4x4 customSimulationSpaceMatrix)
        {
            // Convert target position to custom space
            Vector3 targetWorldPos = target.position;
            Vector3 targetLocalPos = customSimulationSpaceMatrix.inverse.MultiplyPoint3x4(targetWorldPos);

            // Current particle position is already in custom space
            Vector3 currentPos = particle.position;
            float distance = Vector3.Distance(currentPos, targetLocalPos);

            // Initialize particle state
            if (!particleInitialized.ContainsKey(particle.randomSeed))
            {
                particleStartPositions[particle.randomSeed] = currentPos;
                particleStartTimes[particle.randomSeed] = timeElapsed;
                particleAngles[particle.randomSeed] = Random.Range(0f, 360f); // Random initial angle
                particleInitialized[particle.randomSeed] = true;
                return false;
            }

            // If reached near target point, stop moving
            if (distance < arrivalDistance)
            {
                particle.position = targetLocalPos;
                particle.velocity = Vector3.zero;
                return true;
            }

            // Calculate motion progress
            float particleTime = timeElapsed - particleStartTimes[particle.randomSeed];
            float progress = Mathf.Clamp01(particleTime / convergenceDuration);

            // Calculate base position
            Vector3 startPos = particleStartPositions[particle.randomSeed];
            Vector3 basePosition = Vector3.Lerp(startPos, targetLocalPos, progress);

            // Add spiral motion
            if (useSpiral)
            {
                Vector3 direction = (targetLocalPos - startPos).normalized;
                float angle = particleAngles[particle.randomSeed];
                angle += spiralSpeed * Time.deltaTime;
                particleAngles[particle.randomSeed] = angle;

                // Calculate spiral offset
                Quaternion rotation = Quaternion.AngleAxis(angle, direction);
                Vector3 right = Vector3.right;
                if (direction.y > 0.99f || direction.y < -0.99f)
                {
                    right = Vector3.forward;
                }
                Vector3 spiralOffset = rotation * right;
                spiralOffset *= spiralRadius * (1f - progress);
                basePosition += spiralOffset;
            }

            // Update particle position and velocity
            Vector3 newVelocity = (basePosition - currentPos) / Time.deltaTime;
            particle.velocity = newVelocity;
            particle.position = basePosition;

            return false;
        }

        void FinishConvergence()
        {
            hasFinished = true;
            ps.Stop();
            StartCoroutine(DeactivateAfterDelay());
        }

        System.Collections.IEnumerator DeactivateAfterDelay()
        {
            yield return new WaitForSeconds(deactivateDelay);
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            particleStartPositions.Clear();
            particleStartTimes.Clear();
            particleAngles.Clear();
            particleInitialized.Clear();
        }
    }
}