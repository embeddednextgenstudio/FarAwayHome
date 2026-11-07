using UnityEngine;

public class SpaceShooterEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 2.4f;
    private Transform target;
    private SpaceShooterGameManager gameManager;

    public void Initialize(Transform player, SpaceShooterGameManager manager)
    {
        target = player;
        gameManager = manager;
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;

        Vector3 direction = target != null
            ? (target.position - transform.position).normalized
            : Vector3.down;

        transform.position += direction * speed * Time.deltaTime;

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpaceShooterProjectile>() != null)
        {
            if (gameManager != null) gameManager.AddScore(10);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.GetComponent<SpaceShooterPlayer>() != null)
        {
            SpaceShooterPlayer player = other.GetComponent<SpaceShooterPlayer>();
            if (player != null) player.TakeHit();
            Destroy(gameObject);
        }
    }
}
