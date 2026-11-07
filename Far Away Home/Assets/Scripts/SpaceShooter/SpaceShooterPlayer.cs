using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SpaceShooterPlayer : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject fireworksRocketPrefab;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float fireInterval = 0.22f;
    [SerializeField] private float rocketCooldown = 7f;
    [SerializeField] private float horizontalLimit = 8f;
    [SerializeField] private float verticalLimit = 4.3f;

    private float nextFireTime;
    private float nextRocketTime;
    private int weaponMode;
    [SerializeField] private SpaceShooterGameManager gameManager;

    public void Initialize(SpaceShooterGameManager manager) { gameManager = manager; }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        float horizontal = 0f, vertical = 0f;
        bool firePressed = false, rocketPressed = false, spreadPressed = false;
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) horizontal -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) horizontal += 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) vertical -= 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) vertical += 1f;
            firePressed = keyboard.spaceKey.isPressed;
            rocketPressed = keyboard.xKey.wasPressedThisFrame;
            spreadPressed = keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame;
            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame) weaponMode = 0;
        }
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) firePressed = true;
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) vertical += 1f;
        firePressed = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        rocketPressed = Input.GetKeyDown(KeyCode.X);
        spreadPressed = Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2);
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) weaponMode = 0;
#endif
        if (spreadPressed) weaponMode = 1;
        Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;
        transform.position += movement * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -horizontalLimit, horizontalLimit),
            Mathf.Clamp(transform.position.y, -verticalLimit, verticalLimit), 0f);

        if (firePressed && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + (weaponMode == 1 ? fireInterval * 1.8f : fireInterval);
        }
        if (rocketPressed && Time.time >= nextRocketTime) FireFireworksRocket();
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;
        Vector3 origin = transform.position + Vector3.up * 0.95f;
        if (weaponMode == 0)
        {
            Instantiate(projectilePrefab, origin, Quaternion.identity);
        }
        else
        {
            Instantiate(projectilePrefab, origin, Quaternion.Euler(0f, 0f, -17f));
            Instantiate(projectilePrefab, origin, Quaternion.identity);
            Instantiate(projectilePrefab, origin, Quaternion.Euler(0f, 0f, 17f));
        }
        if (gameManager != null) gameManager.PlayLaserSound();
    }

    private void FireFireworksRocket()
    {
        if (fireworksRocketPrefab == null) return;
        Instantiate(fireworksRocketPrefab, transform.position + Vector3.up * 1.1f, Quaternion.identity);
        nextRocketTime = Time.time + rocketCooldown;
        if (gameManager != null) gameManager.PlayLaserSound();
    }

    public void TakeHit()
    {
        if (gameManager != null) { gameManager.PlayPlayerHitSound(); gameManager.LoseLife(); }
    }

    public void SetProjectilePrefab(GameObject prefab) { projectilePrefab = prefab; }
    public void SetFireworksRocketPrefab(GameObject prefab) { fireworksRocketPrefab = prefab; }
}