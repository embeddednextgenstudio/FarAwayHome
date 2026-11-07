using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// A slow, readable rocket that detonates on contact and defeats every active FPS bot.
    /// This is intentionally scoped to the FPS framework; it does not affect the arcade shooter.
    /// </summary>
    public class FireworksRocketProjectile : ProjectileBase
    {
        [Header("Rocket")]
        [Min(1f)] public float Speed = 18f;
        [Min(0.1f)] public float Radius = 0.12f;
        [Min(1f)] public float MaxLifetime = 8f;
        public LayerMask HittableLayers = -1;

        [Header("Fireworks")]
        [Min(0.1f)] public float FireworksLifetime = 2f;
        [Min(1)] public int FireworksParticleCount = 90;
        public AudioClip ExplosionSfx;

        Vector3 m_Velocity;
        Vector3 m_LastPosition;
        Collider[] m_OwnerColliders;
        bool m_Detonated;

        void OnEnable()
        {
            OnShoot += BeginFlight;
            Destroy(gameObject, MaxLifetime);
        }

        void OnDisable()
        {
            OnShoot -= BeginFlight;
        }

        void BeginFlight()
        {
            m_Velocity = transform.forward * Speed + InheritedMuzzleVelocity;
            m_LastPosition = transform.position;
            m_OwnerColliders = Owner != null ? Owner.GetComponentsInChildren<Collider>() : new Collider[0];
        }

        void Update()
        {
            if (m_Detonated)
                return;

            Vector3 displacement = m_Velocity * Time.deltaTime;
            float distance = displacement.magnitude;
            if (distance > 0f)
            {
                RaycastHit[] hits = Physics.SphereCastAll(m_LastPosition, Radius, displacement / distance,
                    distance, HittableLayers, QueryTriggerInteraction.Ignore);

                foreach (RaycastHit hit in hits)
                {
                    if (IsOwnerCollider(hit.collider))
                        continue;

                    Detonate(hit.point);
                    return;
                }
            }

            transform.position += displacement;
            if (m_Velocity.sqrMagnitude > 0.001f)
                transform.forward = m_Velocity.normalized;
            m_LastPosition = transform.position;
        }

        bool IsOwnerCollider(Collider candidate)
        {
            if (m_OwnerColliders == null)
                return false;

            for (int i = 0; i < m_OwnerColliders.Length; i++)
            {
                if (candidate == m_OwnerColliders[i])
                    return true;
            }

            return false;
        }

        void Detonate(Vector3 position)
        {
            if (m_Detonated)
                return;

            m_Detonated = true;
            CreateFireworks(position);

            // Only FPS enemies are targeted. The player and other Health components are untouched.
            EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            foreach (EnemyController enemy in enemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    continue;

                Health health = enemy.GetComponent<Health>();
                if (health != null)
                    health.Kill();
            }

            if (ExplosionSfx != null)
                AudioSource.PlayClipAtPoint(ExplosionSfx, position);

            Destroy(gameObject);
        }

        void CreateFireworks(Vector3 position)
        {
            GameObject burstObject = new GameObject("Fireworks Rocket Burst");
            burstObject.transform.position = position;

            ParticleSystem particles = burstObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.duration = 0.25f;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.7f, FireworksLifetime);
            main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 11f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.18f);
            main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.magenta);
            main.maxParticles = FireworksParticleCount;

            var emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)FireworksParticleCount) });

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.15f;

            var color = particles.colorOverLifetime;
            color.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[] {
                    new GradientColorKey(Color.yellow, 0f),
                    new GradientColorKey(Color.magenta, 0.45f),
                    new GradientColorKey(Color.cyan, 1f)
                },
                new[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                });
            color.color = gradient;

            particles.Play();
            Destroy(burstObject, FireworksLifetime + 0.5f);
        }
    }
}
