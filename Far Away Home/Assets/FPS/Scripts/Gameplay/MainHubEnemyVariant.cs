using Unity.FPS.AI;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.FPS.Gameplay
{
    /// <summary>
    /// Applies a lightweight gameplay archetype to an existing Main_Scene FPS bot.
    /// Visuals and the underlying AI prefab remain unchanged.
    /// </summary>
    public class MainHubEnemyVariant : MonoBehaviour
    {
        public enum VariantType
        {
            Scout,
            Heavy,
            Veteran
        }

        public VariantType Variant = VariantType.Scout;
        bool m_Applied;

        // Apply after the FPS framework's Start methods, which initialize health and
        // copy navigation settings from NavigationModule onto the NavMeshAgent.
        void LateUpdate()
        {
            if (m_Applied)
                return;

            m_Applied = true;
            Health health = GetComponent<Health>();
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            DetectionModule detection = GetComponentInChildren<DetectionModule>(true);
            WeaponController weapon = GetComponentInChildren<WeaponController>(true);

            switch (Variant)
            {
                case VariantType.Scout:
                    if (health)
                    {
                        health.MaxHealth = 65f;
                        health.CurrentHealth = health.MaxHealth;
                    }
                    if (agent) agent.speed *= 1.45f;
                    if (detection)
                    {
                        detection.DetectionRange *= 1.15f;
                        detection.AttackRange *= 1.1f;
                    }
                    if (weapon) weapon.DelayBetweenShots *= 0.8f;
                    break;

                case VariantType.Heavy:
                    if (health)
                    {
                        health.MaxHealth = 220f;
                        health.CurrentHealth = health.MaxHealth;
                    }
                    if (agent) agent.speed *= 0.65f;
                    if (detection) detection.AttackRange *= 1.15f;
                    if (weapon) weapon.DelayBetweenShots *= 1.35f;
                    break;

                case VariantType.Veteran:
                    if (health)
                    {
                        health.MaxHealth = 145f;
                        health.CurrentHealth = health.MaxHealth;
                    }
                    if (agent) agent.speed *= 1.1f;
                    if (detection)
                    {
                        detection.DetectionRange *= 1.3f;
                        detection.AttackRange *= 1.2f;
                    }
                    if (weapon) weapon.DelayBetweenShots *= 0.7f;
                    break;
            }
        }
    }
}
