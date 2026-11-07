using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Adds a small, enterable four-room house to the FPS gameplay scenes.
/// The house is generated from Unity primitives, so no external assets are required.
/// </summary>
public static class FarAwayHomeBuildingBootstrap
{
    private const string BuildingName = "FarAwayHome - Multi Room House";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        BuildForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildForScene(scene);
    }

    private static void BuildForScene(Scene scene)
    {
        // Keep the addition in the main FPS level and its follow-up level only.
        if (scene.name != "MainScene" && scene.name != "SecondaryScene")
            return;

        if (GameObject.Find(BuildingName) != null)
            return;

        CharacterController controller = Object.FindFirstObjectByType<CharacterController>();
        Camera playerCamera = Camera.main;
        Transform player = controller != null
            ? controller.transform
            : (playerCamera != null ? playerCamera.transform : null);

        if (player == null)
        {
            Debug.LogWarning("FarAwayHome house was not created: no player or MainCamera was found.");
            return;
        }

        Vector3 forward = Vector3.ProjectOnPlane(
            playerCamera != null ? playerCamera.transform.forward : player.forward,
            Vector3.up
        ).normalized;

        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.forward;

        Vector3 housePosition = player.position + forward * 24f;
        Vector3 rayOrigin = housePosition + Vector3.up * 60f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit groundHit, 120f))
            housePosition.y = groundHit.point.y;
        else if (controller != null)
            housePosition.y = controller.bounds.min.y;

        Quaternion houseRotation = Quaternion.LookRotation(forward, Vector3.up);
        BuildHouse(housePosition, houseRotation);
    }

    private static void BuildHouse(Vector3 position, Quaternion rotation)
    {
        GameObject root = new GameObject(BuildingName);
        root.transform.SetPositionAndRotation(position, rotation);

        Material wall = MakeMaterial("Warm plaster", new Color(0.78f, 0.72f, 0.61f));
        Material trim = MakeMaterial("Dark timber trim", new Color(0.25f, 0.16f, 0.10f));
        Material floor = MakeMaterial("Oak floor", new Color(0.42f, 0.27f, 0.15f));
        Material ceiling = MakeMaterial("Ceiling", new Color(0.88f, 0.85f, 0.77f));
        Material glass = MakeMaterial("Blue glass", new Color(0.28f, 0.65f, 0.78f, 0.55f));
        Material fabric = MakeMaterial("Sofa fabric", new Color(0.22f, 0.36f, 0.42f));
        Material rug = MakeMaterial("Living room rug", new Color(0.55f, 0.24f, 0.17f));
        Material cabinet = MakeMaterial("Kitchen cabinets", new Color(0.48f, 0.29f, 0.16f));
        Material white = MakeMaterial("Ceramic", new Color(0.88f, 0.89f, 0.86f));
        Material bedCover = MakeMaterial("Bed cover", new Color(0.36f, 0.43f, 0.62f));

        // Main shell: 18 x 14 metres, with a clear front entrance.
        Cube(root.transform, "Oak floor", new Vector3(0f, 0.08f, 0f),
            new Vector3(18f, 0.16f, 14f), floor);
        // Upper-storey floor panels leave an opening for the staircase.
        Cube(root.transform, "Upper floor left panel", new Vector3(-4.9f, 3.35f, 0f), new Vector3(8.2f, 0.2f, 14f), floor);
        Cube(root.transform, "Upper floor right panel", new Vector3(4.9f, 3.35f, 0f), new Vector3(8.2f, 0.2f, 14f), floor);
        // Keep the central strip open from the entrance through to the upper landing.
        Cube(root.transform, "Upper floor rear panel", new Vector3(0f, 3.35f, 3.8f), new Vector3(1.6f, 0.2f, 6.4f), floor);
        Cube(root.transform, "Second floor ceiling", new Vector3(0f, 6.65f, 0f), new Vector3(18f, 0.2f, 14f), ceiling);

        Cube(root.transform, "Left outside wall", new Vector3(-8.9f, 1.6f, 0f),
            new Vector3(0.2f, 3.2f, 14f), wall);
        Cube(root.transform, "Right outside wall", new Vector3(8.9f, 1.6f, 0f),
            new Vector3(0.2f, 3.2f, 14f), wall);
        Cube(root.transform, "Back outside wall", new Vector3(0f, 1.6f, 6.9f),
            new Vector3(18f, 3.2f, 0.2f), wall);

        // Front facade is split around a 2.4 metre-wide entrance.
        Cube(root.transform, "Front wall left", new Vector3(-5.1f, 1.6f, -6.9f),
            new Vector3(7.8f, 3.2f, 0.2f), wall);
        Cube(root.transform, "Front wall right", new Vector3(5.1f, 1.6f, -6.9f),
            new Vector3(7.8f, 3.2f, 0.2f), wall);
        Cube(root.transform, "Entrance lintel", new Vector3(0f, 2.9f, -6.9f),
            new Vector3(2.4f, 0.6f, 0.25f), trim);
        Cube(root.transform, "Entrance left post", new Vector3(-1.2f, 1.45f, -6.9f),
            new Vector3(0.16f, 2.9f, 0.3f), trim);
        Cube(root.transform, "Entrance right post", new Vector3(1.2f, 1.45f, -6.9f),
            new Vector3(0.16f, 2.9f, 0.3f), trim);

        // A central hall connects the living room, kitchen, bedroom and bathroom.
        BuildHallPartition(root.transform, -1.25f, wall);
        BuildHallPartition(root.transform, 1.25f, wall);

        // Split the left and right wings into two distinct rooms each.
        Cube(root.transform, "Left room divider", new Vector3(-5f, 1.6f, 0f),
            new Vector3(7.6f, 3.2f, 0.2f), wall);
        Cube(root.transform, "Right room divider", new Vector3(5f, 1.6f, 0f),
            new Vector3(7.6f, 3.2f, 0.2f), wall);

        // Simple windows on the outer walls.
        Window(root.transform, new Vector3(-8.77f, 2.0f, -3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(root.transform, new Vector3(8.77f, 2.0f, -3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(root.transform, new Vector3(-8.77f, 2.0f, 3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(root.transform, new Vector3(8.77f, 2.0f, 3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);

        // Room signs are placed on the inside face of the front/back walls.
        RoomSign(root.transform, "LIVING ROOM", new Vector3(-5f, 2.65f, -6.77f), 0f);
        RoomSign(root.transform, "KITCHEN", new Vector3(5f, 2.65f, -6.77f), 0f);
        RoomSign(root.transform, "BEDROOM", new Vector3(-5f, 2.65f, 6.77f), 180f);
        RoomSign(root.transform, "BATHROOM", new Vector3(5f, 2.65f, 6.77f), 180f);
        RoomSign(root.transform, "HALL", new Vector3(0f, 2.65f, -5.9f), 0f);

        // Living room.
        Cube(root.transform, "Living room rug", new Vector3(-5f, 0.18f, -3.25f),
            new Vector3(5.4f, 0.05f, 4.5f), rug);
        Cube(root.transform, "Sofa base", new Vector3(-5f, 0.58f, -4.65f),
            new Vector3(3.6f, 0.75f, 1.15f), fabric);
        Cube(root.transform, "Sofa back", new Vector3(-5f, 1.05f, -5.12f),
            new Vector3(3.6f, 1.0f, 0.28f), fabric);
        Cube(root.transform, "Coffee table", new Vector3(-5f, 0.48f, -2.2f),
            new Vector3(2.0f, 0.18f, 1.0f), trim);
        Cube(root.transform, "TV stand", new Vector3(-5f, 0.45f, -0.35f),
            new Vector3(3.0f, 0.7f, 0.55f), cabinet);

        // Kitchen.
        Cube(root.transform, "Kitchen counter", new Vector3(5f, 0.62f, -5.25f),
            new Vector3(5.6f, 1.15f, 0.85f), cabinet);
        Cube(root.transform, "Kitchen worktop", new Vector3(5f, 1.22f, -5.25f),
            new Vector3(5.75f, 0.08f, 0.95f), white);
        Cube(root.transform, "Kitchen island", new Vector3(5f, 0.62f, -2.6f),
            new Vector3(2.8f, 1.15f, 1.15f), cabinet);
        Cube(root.transform, "Kitchen island top", new Vector3(5f, 1.22f, -2.6f),
            new Vector3(2.95f, 0.08f, 1.25f), white);
        for (int i = 0; i < 3; i++)
            Cube(root.transform, "Kitchen upper cabinet " + i,
                new Vector3(3.2f + i * 1.8f, 2.25f, -6.55f),
                new Vector3(1.45f, 0.85f, 0.35f), cabinet);

        // Bedroom.
        Cube(root.transform, "Bedroom rug", new Vector3(-5f, 0.18f, 3.35f),
            new Vector3(5.2f, 0.05f, 5.0f), rug);
        Cube(root.transform, "Bed frame", new Vector3(-5f, 0.42f, 3.5f),
            new Vector3(3.4f, 0.55f, 4.2f), trim);
        Cube(root.transform, "Mattress", new Vector3(-5f, 0.78f, 3.35f),
            new Vector3(3.25f, 0.35f, 4.0f), white);
        Cube(root.transform, "Bed cover", new Vector3(-5f, 0.98f, 2.7f),
            new Vector3(3.2f, 0.08f, 2.6f), bedCover);
        Cube(root.transform, "Headboard", new Vector3(-5f, 1.0f, 5.55f),
            new Vector3(3.5f, 1.6f, 0.22f), trim);
        Cube(root.transform, "Bedside table", new Vector3(-7.1f, 0.5f, 5.1f),
            new Vector3(0.9f, 0.9f, 0.8f), cabinet);
        Cube(root.transform, "Wardrobe", new Vector3(-2.9f, 1.35f, 5.6f),
            new Vector3(1.3f, 2.6f, 1.0f), cabinet);

        // Bathroom.
        Cube(root.transform, "Bathtub", new Vector3(6.7f, 0.45f, 4.7f),
            new Vector3(2.8f, 0.8f, 1.45f), white);
        Cube(root.transform, "Bathroom sink", new Vector3(3.45f, 0.75f, 5.6f),
            new Vector3(1.25f, 0.9f, 0.7f), white);
        Cube(root.transform, "Bathroom vanity", new Vector3(3.45f, 0.45f, 5.6f),
            new Vector3(1.35f, 0.8f, 0.8f), cabinet);
        Cube(root.transform, "Toilet base", new Vector3(4.6f, 0.35f, 2.45f),
            new Vector3(0.75f, 0.65f, 1.0f), white);
        Cube(root.transform, "Toilet cistern", new Vector3(4.6f, 0.95f, 2.95f),
            new Vector3(0.75f, 1.0f, 0.25f), white);

        // Warm lighting helps the separate rooms read clearly indoors.
        RoomLight(root.transform, new Vector3(-5f, 2.85f, -3.2f));
        RoomLight(root.transform, new Vector3(5f, 2.85f, -3.2f));
        RoomLight(root.transform, new Vector3(-5f, 2.85f, 3.2f));
        RoomLight(root.transform, new Vector3(5f, 2.85f, 3.2f));
        RoomLight(root.transform, new Vector3(0f, 2.9f, 0f));

        BuildUpperFloor(root.transform, wall, trim, glass, cabinet, white, bedCover);

        // Small sign above the entrance.
        RoomSign(root.transform, "FAR AWAY HOME", new Vector3(0f, 2.55f, -7.08f), 0f);

        Debug.Log("FarAwayHome: generated an enterable house with four rooms and a connecting hall.");
    }


    private static void BuildUpperFloor(Transform parent, Material wall, Material trim,
        Material glass, Material cabinet, Material white, Material bedCover)
    {
        Cube(parent, "Upper left outside wall", new Vector3(-8.9f, 5.05f, 0f), new Vector3(0.2f, 3.2f, 14f), wall);
        Cube(parent, "Upper right outside wall", new Vector3(8.9f, 5.05f, 0f), new Vector3(0.2f, 3.2f, 14f), wall);
        Cube(parent, "Upper back outside wall", new Vector3(0f, 5.05f, 6.9f), new Vector3(18f, 3.2f, 0.2f), wall);
        Cube(parent, "Upper front outside wall", new Vector3(0f, 5.05f, -6.9f), new Vector3(18f, 3.2f, 0.2f), wall);

        BuildUpperHallPartition(parent, -1.25f, wall, trim);
        BuildUpperHallPartition(parent, 1.25f, wall, trim);
        Cube(parent, "Upper left room divider", new Vector3(-5f, 5.05f, 0f), new Vector3(7.6f, 3.2f, 0.2f), wall);
        Cube(parent, "Upper right room divider", new Vector3(5f, 5.05f, 0f), new Vector3(7.6f, 3.2f, 0.2f), wall);

        // Fourteen shallow steps rise through the central hall into the upper landing.
        const int stepCount = 14;
        const float stepRise = 3.36f / stepCount;
        const float stepDepth = 5.6f / stepCount;
        for (int i = 0; i < stepCount; i++)
        {
            float top = stepRise * (i + 1);
            float z = -6f + stepDepth * (i + 0.5f);
            Cube(parent, "Stair step " + (i + 1), new Vector3(0f, top - stepRise * 0.5f, z),
                new Vector3(1.12f, stepRise, stepDepth + 0.025f), trim);
        }
        Cube(parent, "Upper stair landing", new Vector3(0f, 3.35f, 0.2f), new Vector3(1.6f, 0.2f, 1.2f), floor);

        GameObject railLeft = Cube(parent, "Stair handrail left", new Vector3(-0.68f, 1.85f, -3.2f),
            new Vector3(0.1f, 0.12f, 5.9f), trim);
        railLeft.transform.localRotation = Quaternion.Euler(-31f, 0f, 0f);
        GameObject railRight = Cube(parent, "Stair handrail right", new Vector3(0.68f, 1.85f, -3.2f),
            new Vector3(0.1f, 0.12f, 5.9f), trim);
        railRight.transform.localRotation = Quaternion.Euler(-31f, 0f, 0f);

        RoomSign(parent, "GUEST BEDROOM", new Vector3(-5f, 5.9f, -6.77f), 0f);
        RoomSign(parent, "STUDY", new Vector3(5f, 5.9f, -6.77f), 0f);
        RoomSign(parent, "LIBRARY", new Vector3(-5f, 5.9f, 6.77f), 180f);
        RoomSign(parent, "HOBBY ROOM", new Vector3(5f, 5.9f, 6.77f), 180f);
        RoomSign(parent, "UPPER HALL", new Vector3(0f, 5.9f, 1.2f), 180f);

        Cube(parent, "Guest bed frame", new Vector3(-5f, 3.85f, -3.3f), new Vector3(3f, 0.45f, 3.6f), trim);
        Cube(parent, "Guest mattress", new Vector3(-5f, 4.15f, -3.3f), new Vector3(2.9f, 0.25f, 3.5f), white);
        Cube(parent, "Guest bed cover", new Vector3(-5f, 4.3f, -2.7f), new Vector3(2.85f, 0.08f, 2.1f), bedCover);
        Cube(parent, "Study desk", new Vector3(5f, 4f, -4.9f), new Vector3(3.4f, 0.15f, 1.4f), cabinet);
        Cube(parent, "Study desk legs", new Vector3(5f, 3.7f, -4.9f), new Vector3(3.1f, 0.6f, 1.1f), trim);
        Cube(parent, "Study bookcase", new Vector3(7.6f, 4.8f, -1.5f), new Vector3(1.1f, 2.4f, 0.45f), cabinet);
        Cube(parent, "Library shelf", new Vector3(-7.5f, 4.8f, 3.6f), new Vector3(1.2f, 2.4f, 3.8f), cabinet);
        Cube(parent, "Hobby work table", new Vector3(5f, 4f, 3.5f), new Vector3(3.6f, 0.15f, 2f), cabinet);
        Cube(parent, "Hobby storage", new Vector3(7.5f, 4.4f, 5.5f), new Vector3(1f, 1.4f, 1.5f), trim);

        Window(parent, new Vector3(-8.77f, 5.2f, -3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(parent, new Vector3(8.77f, 5.2f, -3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(parent, new Vector3(-8.77f, 5.2f, 3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);
        Window(parent, new Vector3(8.77f, 5.2f, 3.3f), new Vector3(0.06f, 1.1f, 2.1f), glass, trim);

        RoomLight(parent, new Vector3(-5f, 6f, -3.2f));
        RoomLight(parent, new Vector3(5f, 6f, -3.2f));
        RoomLight(parent, new Vector3(-5f, 6f, 3.2f));
        RoomLight(parent, new Vector3(5f, 6f, 3.2f));
        RoomLight(parent, new Vector3(0f, 6f, 1.5f));
    }

    private static void BuildUpperHallPartition(Transform parent, float x, Material wall, Material trim)
    {
        const float gap = 0.85f;
        const float firstDoor = -4.2f;
        const float secondDoor = 4.2f;
        const float zMin = -6.8f;
        const float zMax = 6.8f;
        UpperWallZ(parent, "Upper hall partition front", x, (zMin + firstDoor - gap) * 0.5f, (firstDoor - gap) - zMin, wall);
        UpperWallZ(parent, "Upper hall partition middle", x, 0f, (secondDoor - gap) - (firstDoor + gap), wall);
        UpperWallZ(parent, "Upper hall partition rear", x, (secondDoor + gap + zMax) * 0.5f, zMax - (secondDoor + gap), wall);

        foreach (float doorZ in new float[] { firstDoor, secondDoor })
        {
            Cube(parent, "Upper door frame front", new Vector3(x, 4.9f, doorZ - gap), new Vector3(0.28f, 2.9f, 0.12f), trim);
            Cube(parent, "Upper door frame rear", new Vector3(x, 4.9f, doorZ + gap), new Vector3(0.28f, 2.9f, 0.12f), trim);
            Cube(parent, "Upper door lintel", new Vector3(x, 6.35f, doorZ), new Vector3(0.28f, 0.18f, 1.7f), trim);
        }
    }

    private static void UpperWallZ(Transform parent, string name, float x, float z, float length, Material material)
    {
        if (length <= 0.05f) return;
        Cube(parent, name, new Vector3(x, 5.05f, z), new Vector3(0.2f, 3.2f, length), material);
    }

    private static void BuildHallPartition(Transform parent, float x, Material material)
    {
        const float zMin = -6.8f;
        const float zMax = 6.8f;
        const float openingWidth = 1.7f;
        const float firstDoor = -4.2f;
        const float secondDoor = 4.2f;
        const float halfGap = openingWidth * 0.5f;

        WallZ(parent, "Hall partition front", x, (zMin + firstDoor - halfGap) * 0.5f,
            (firstDoor - halfGap) - zMin, material);
        WallZ(parent, "Hall partition middle", x, 0f,
            (secondDoor - halfGap) - (firstDoor + halfGap), material);
        WallZ(parent, "Hall partition back", x, (secondDoor + halfGap + zMax) * 0.5f,
            zMax - (secondDoor + halfGap), material);

        // Timber trim makes each open doorway visible without blocking movement.
        Cube(parent, "Door frame front", new Vector3(x, 1.45f, firstDoor - halfGap),
            new Vector3(0.28f, 2.9f, 0.12f), material);
        Cube(parent, "Door frame back", new Vector3(x, 1.45f, firstDoor + halfGap),
            new Vector3(0.28f, 2.9f, 0.12f), material);
        Cube(parent, "Door lintel", new Vector3(x, 2.9f, firstDoor),
            new Vector3(0.28f, 0.18f, openingWidth), material);
        Cube(parent, "Door frame front", new Vector3(x, 1.45f, secondDoor - halfGap),
            new Vector3(0.28f, 2.9f, 0.12f), material);
        Cube(parent, "Door frame back", new Vector3(x, 1.45f, secondDoor + halfGap),
            new Vector3(0.28f, 2.9f, 0.12f), material);
        Cube(parent, "Door lintel", new Vector3(x, 2.9f, secondDoor),
            new Vector3(0.28f, 0.18f, openingWidth), material);
    }

    private static void WallZ(Transform parent, string name, float x, float z, float length, Material material)
    {
        if (length <= 0.05f) return;
        Cube(parent, name, new Vector3(x, 1.6f, z),
            new Vector3(0.2f, 3.2f, length), material);
    }

    private static void Window(Transform parent, Vector3 position, Vector3 size, Material glass, Material frame)
    {
        Cube(parent, "Window glass", position, size, glass);
        Cube(parent, "Window sill", position + Vector3.down * 0.62f,
            new Vector3(size.x + 0.12f, 0.1f, size.z + 0.16f), frame);
        Cube(parent, "Window top frame", position + Vector3.up * 0.62f,
            new Vector3(size.x + 0.12f, 0.1f, size.z + 0.16f), frame);
    }

    private static void RoomSign(Transform parent, string label, Vector3 position, float yaw)
    {
        GameObject sign = new GameObject("Room label - " + label);
        sign.transform.SetParent(parent, false);
        sign.transform.localPosition = position;
        sign.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        TextMesh text = sign.AddComponent<TextMesh>();
        text.text = label;
        text.fontSize = 42;
        text.characterSize = 0.055f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = new Color(0.18f, 0.12f, 0.08f);
    }

    private static void RoomLight(Transform parent, Vector3 position)
    {
        GameObject lightObject = new GameObject("Warm room light");
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = position;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.82f, 0.62f);
        light.intensity = 2.0f;
        light.range = 8f;
        light.shadows = LightShadows.Soft;
    }

    private static GameObject Cube(Transform parent, string name, Vector3 localPosition,
        Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localScale = localScale;
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = material;
        return cube;
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material material = new Material(shader) { name = name };
        material.color = color;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.18f);
        return material;
    }
}
