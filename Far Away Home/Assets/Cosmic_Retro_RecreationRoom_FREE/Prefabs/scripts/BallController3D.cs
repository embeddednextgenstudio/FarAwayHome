
using UnityEngine;

public class BallController3D : MonoBehaviour
{
    public float speed = 7f;

    private Rigidbody rb;
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        direction = new Vector3(1f, 0.5f, 0f).normalized;

        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            float hitPosition =
                transform.position.y -
                collision.transform.position.y;

            direction = new Vector3(
                -direction.x,
                hitPosition,
                0f
            ).normalized;

            rb.linearVelocity = direction * speed;
        }
    }
}
