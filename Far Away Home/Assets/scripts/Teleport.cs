
using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportDestination;
    public float teleportCooldown = 1f;

    private bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player") && canTeleport)
        {
            // Check if destination exists
            if (teleportDestination == null)
            {
                Debug.LogError("Teleport destination is missing!");
                return;
            }

            Debug.Log("Player entered teleport area!");

            // Start teleport process
            StartCoroutine(TeleportPlayer(other));
        }
    }

    private IEnumerator TeleportPlayer(Collider player)
    {
        canTeleport = false;

        // Get the player Rigidbody if available
        Rigidbody rb = player.attachedRigidbody;

        // Move player to destination
        if (rb != null)
        {
            rb.position = teleportDestination.position;
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            player.transform.position = teleportDestination.position;
        }

        Debug.Log("Player successfully teleported!");

        // Wait before allowing another teleport
        yield return new WaitForSeconds(teleportCooldown);

        canTeleport = true;
    }
}
