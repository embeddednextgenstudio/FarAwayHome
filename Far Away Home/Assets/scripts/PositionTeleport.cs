
using UnityEngine;

public class PositionTeleport : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Teleport Settings")]
    public Transform teleportArea;
    public Transform teleportDestination;

    [Header("Area Size")]
    public Vector3 areaSize = new Vector3(5f, 3f, 5f);

    private bool hasTeleported = false;

    void Update()
    {
        if (player == null ||
            teleportArea == null ||
            teleportDestination == null)
            return;

        // Calculate player's position relative to the area
        Vector3 localPosition =
            teleportArea.InverseTransformPoint(player.position);

        Vector3 halfSize = areaSize / 2f;

        // Check if player is inside the marked area
        bool insideArea =
            Mathf.Abs(localPosition.x) <= halfSize.x &&
            Mathf.Abs(localPosition.y) <= halfSize.y &&
            Mathf.Abs(localPosition.z) <= halfSize.z;

        if (insideArea && !hasTeleported)
        {
            player.position = teleportDestination.position;
            hasTeleported = true;

            Debug.Log("Player teleported!");
        }

        // Allow teleporting again after leaving the area
        if (!insideArea)
        {
            hasTeleported = false;
        }
    }

    // Show the teleport area in the Scene view
    void OnDrawGizmos()
    {
        if (teleportArea == null) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = teleportArea.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, areaSize);
    }
}
