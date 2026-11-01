
using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinSound;

    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play coin sound
            AudioSource.PlayClipAtPoint(
                coinSound,
                transform.position
            );

            // Add score and check if all coins are collected
            GameManager.instance.AddScore(1);

            // Destroy coin
            Destroy(gameObject);
        }
    }
}
