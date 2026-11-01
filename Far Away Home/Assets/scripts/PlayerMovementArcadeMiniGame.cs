using UnityEngine;

public class PlayerMovementArcadeMiniGame : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, 0, z);

        transform.Translate(
            movement * speed * Time.deltaTime,
            Space.World
        );
    }
}
