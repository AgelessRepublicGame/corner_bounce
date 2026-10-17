using UnityEngine;
using System.Collections;


namespace DWFX
{
    public class ParticleCollector : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private ParticleSystem.Particle[] particles;

        public Transform collectTarget; 
        public float initialSpreadRadius = 2f; 
        public float collectSpeed = 5f;
        public float collectStartDelay = 1f; 

        private bool isCollecting = false;
        private float collectTimer = 0f;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

            var main = particleSystem.main;
            main.loop = false;
            main.playOnAwake = false;

            var emission = particleSystem.emission;
            emission.rateOverTime = 0;

            StartCoroutine(PlayEffect());
        }

        IEnumerator PlayEffect()
        {
            EmitParticles();

            yield return new WaitForSeconds(collectStartDelay);
            isCollecting = true;
        }

        void EmitParticles()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPos = Random.insideUnitSphere * initialSpreadRadius;
                particleSystem.Emit(new ParticleSystem.EmitParams()
                {
                    position = transform.position + randomPos,
                    velocity = randomPos.normalized * 2f
                }, 1);
            }
        }

        void Update()
        {
            if (!isCollecting) return;

            int numParticles = particleSystem.GetParticles(particles);

            for (int i = 0; i < numParticles; i++)
            {
                Vector3 directionToTarget = collectTarget.position - particles[i].position;
                float distance = directionToTarget.magnitude;

                float speed = collectSpeed * (1f + distance * 0.5f);

                particles[i].position += directionToTarget.normalized * speed * Time.deltaTime;

                if (distance < 0.1f)
                {
                    particles[i].remainingLifetime = 0f;
                }
            }

            particleSystem.SetParticles(particles, numParticles);
        }

        public void Replay()
        {
            isCollecting = false;
            collectTimer = 0f;
            particleSystem.Clear();
            StartCoroutine(PlayEffect());
        }
    }
}