
using UnityEngine;

public class PaddleController3D : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        float movement = 0f;

        if (Input.GetKey(KeyCode.W))
            movement = 1f;

        if (Input.GetKey(KeyCode.S))
            movement = -1f;

        transform.Translate(
            Vector3.up * movement * speed * Time.deltaTime,
            Space.World
        );

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -3f, 3f);
        transform.position = pos;
    }
}
