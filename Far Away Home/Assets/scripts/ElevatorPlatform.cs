
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    public float speed = 2f;
    public float height = 5f;
    public float waitTime = 2f;

    private Vector3 startPosition;
    private Vector3 topPosition;

    private bool movingUp = true;
    private float waitTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * height;
    }

    void Update()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 targetPosition = movingUp
            ? topPosition
            : startPosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingUp = !movingUp;
            waitTimer = waitTime;
        }
    }
}
