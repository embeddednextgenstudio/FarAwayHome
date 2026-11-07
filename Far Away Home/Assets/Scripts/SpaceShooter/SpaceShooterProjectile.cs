using UnityEngine;

public class SpaceShooterProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 16f;
    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }
}
