using UnityEngine;

public class SpaceShooterEnemy : MonoBehaviour
{
    public enum EnemyType { Scout, Hunter, Tank }

    [SerializeField] private float speed = 2.4f;
    private Transform target;
    private SpaceShooterGameManager gameManager;
    private EnemyType enemyType;
    private int health = 1;
    private float phase;

    public void Initialize(Transform player, SpaceShooterGameManager manager, EnemyType type = EnemyType.Scout)
    {
        target = player;
        gameManager = manager;
        enemyType = type;
        phase = Random.Range(0f, 6.28f);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (type == EnemyType.Scout)
        {
            speed = 3.8f;
            health = 1;
            transform.localScale = Vector3.one * 0.78f;
            Tint(renderers, new Color(1f, 0.35f, 0.12f));
        }
        else if (type == EnemyType.Hunter)
        {
            speed = 2.8f;
            health = 2;
            Tint(renderers, new Color(0.8f, 0.15f, 1f));
        }
        else
        {
            speed = 1.35f;
            health = 4;
            transform.localScale = Vector3.one * 1.45f;
            Tint(renderers, new Color(0.25f, 0.85f, 1f));
        }
    }

    private void Tint(Renderer[] renderers, Color tint)
    {
        foreach (Renderer item in renderers)
        {
            if (item.material != null) item.material.color = tint;
        }
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        Vector3 direction = target != null ? (target.position - transform.position).normalized : Vector3.down;
        if (enemyType == EnemyType.Hunter) direction.x += Mathf.Sin(Time.time * 4f + phase) * 0.8f;
        if (enemyType == EnemyType.Scout) direction.x += Mathf.Sin(Time.time * 2.5f + phase) * 0.25f;
        transform.position += direction.normalized * speed * Time.deltaTime;
        if (transform.position.y < -6f) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpaceShooterProjectile>() != null)
        {
            Destroy(other.gameObject);
            health--;
            if (health <= 0) DestroyByPlayer();
        }
        else if (other.GetComponent<SpaceShooterPlayer>() != null)
        {
            SpaceShooterPlayer player = other.GetComponent<SpaceShooterPlayer>();
            if (player != null) player.TakeHit();
            Destroy(gameObject);
        }
    }

    private void DestroyByPlayer()
    {
        if (gameManager != null)
        {
            int points = enemyType == EnemyType.Tank ? 40 : enemyType == EnemyType.Hunter ? 20 : 10;
            gameManager.AddScore(points);
            gameManager.PlayExplosionSound();
        }
        Destroy(gameObject);
    }

    public void DestroyByFireworks()
    {
        if (gameManager != null) gameManager.AddScore(25);
        Destroy(gameObject);
    }
}