using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SpaceShooterPlayer : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float fireInterval = 0.22f;
    [SerializeField] private float horizontalLimit = 8f;
    [SerializeField] private float verticalLimit = 4.3f;

    private float nextFireTime;
    private SpaceShooterGameManager gameManager;

    public void Initialize(SpaceShooterGameManager manager)
    {
        gameManager = manager;
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;

        float horizontal = 0f;
        float vertical = 0f;
        bool firePressed = false;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) horizontal -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) horizontal += 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) vertical -= 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) vertical += 1f;
            firePressed = keyboard.spaceKey.isPressed;
        }
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) firePressed = true;
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) vertical += 1f;
        firePressed = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
#endif
        Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;
        transform.position += movement * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -horizontalLimit, horizontalLimit),
            Mathf.Clamp(transform.position.y, -verticalLimit, verticalLimit),
            0f);

        if (firePressed && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = transform.position + Vector3.up * 0.95f;
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    }

    public void TakeHit()
    {
        if (gameManager != null) gameManager.LoseLife();
    }

    public void SetProjectilePrefab(GameObject prefab)
    {
        projectilePrefab = prefab;
    }
}
