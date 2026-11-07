using UnityEngine;

public class FireworksRocket : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float detonateHeight = 4.4f;
    private bool detonated;

    private void Update()
    {
        if (detonated) return;
        transform.position += Vector3.up * speed * Time.deltaTime;
        if (transform.position.y >= detonateHeight) Detonate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!detonated && other.GetComponent<SpaceShooterEnemy>() != null) Detonate();
    }

    private void Detonate()
    {
        if (detonated) return;
        detonated = true;
        SpaceShooterGameManager manager = FindObjectOfType<SpaceShooterGameManager>();
        SpaceShooterEnemy[] enemies = FindObjectsOfType<SpaceShooterEnemy>();
        foreach (SpaceShooterEnemy enemy in enemies)
            if (enemy != null) enemy.DestroyByFireworks();
        CreateFireworksBurst();
        if (manager != null) manager.PlayExplosionSound();
        Destroy(gameObject);
    }

    private void CreateFireworksBurst()
    {
        GameObject burst = new GameObject("Fireworks Rocket Burst");
        burst.transform.position = transform.position;
        ParticleSystem particles = burst.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.duration = 1.4f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.65f, 1.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.07f, 0.18f);
        main.maxParticles = 220;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.1f, 1f, 1f), new Color(1f, 0.15f, 0.8f));
        var emission = particles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 150) });
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.18f;
        var renderer = burst.GetComponent<ParticleSystemRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader != null) renderer.material = new Material(shader);
        particles.Play();
        Destroy(burst, 2.5f);
    }
}