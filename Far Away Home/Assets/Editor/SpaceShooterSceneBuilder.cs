using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SpaceShooterSceneBuilder
{
    private const string Root = "Assets/SpaceShooter";
    private const string PrefabFolder = Root + "/Prefabs";
    private const string MaterialFolder = Root + "/Materials";
    private const string SceneFolder = "Assets/Scenes/SpaceShooter";
    private const string ScenePath = SceneFolder + "/SpaceShooter.unity";

    [MenuItem("Tools/Far Away Home/Create Space Shooter Scene")]
    public static void CreateScene()
    {
        EnsureFolder("Assets", "SpaceShooter");
        EnsureFolder(Root, "Prefabs");
        EnsureFolder(Root, "Materials");
        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets/Scenes", "SpaceShooter");

        Material playerMaterial = CreateMaterial("PlayerBlue", new Color(0.12f, 0.62f, 1f), 0.35f);
        Material wingMaterial = CreateMaterial("PlayerWing", new Color(0.04f, 0.2f, 0.48f), 0.25f);
        Material enemyMaterial = CreateMaterial("EnemyRed", new Color(1f, 0.16f, 0.18f), 0.2f);
        Material enemyWingMaterial = CreateMaterial("EnemyDark", new Color(0.42f, 0.025f, 0.06f), 0.2f);
        Material laserMaterial = CreateMaterial("LaserCyan", new Color(0.15f, 1f, 0.95f), 0.7f, true);
        Material rocketMaterial = CreateMaterial("FireworksRocket", new Color(1f, 0.25f, 0.8f), 0.6f, true);
        Material starMaterial = CreateMaterial("StarWhite", new Color(0.75f, 0.88f, 1f), 0f, true);

        GameObject projectilePrefab = BuildProjectile(laserMaterial);
        GameObject rocketPrefab = BuildRocket(rocketMaterial);
        GameObject enemyPrefab = BuildEnemy(enemyMaterial, enemyWingMaterial);
        GameObject playerPrefab = BuildPlayer(playerMaterial, wingMaterial, projectilePrefab, rocketPrefab);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        scene.name = "SpaceShooter";

        Camera camera = CreateCamera();
        CreateStars(starMaterial);

        GameObject managerObject = new GameObject("Space Shooter Game Manager");
        SpaceShooterGameManager manager = managerObject.AddComponent<SpaceShooterGameManager>();

        Canvas canvas = CreateHud(out Text scoreText, out Text livesText, out Text gameOverText);
        manager.Configure(scoreText, livesText, gameOverText);

        GameObject playerObject = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        playerObject.name = "Player Ship";
        playerObject.transform.position = new Vector3(0f, -3.35f, 0f);
        playerObject.GetComponent<SpaceShooterPlayer>().Initialize(manager);

        GameObject spawnerObject = new GameObject("Enemy Spawner");
        SpaceShooterSpawner spawner = spawnerObject.AddComponent<SpaceShooterSpawner>();
        spawner.Configure(enemyPrefab, playerObject.transform, manager);

        GameObject lighting = new GameObject("Directional Light");
        Light light = lighting.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.7f;
        lighting.transform.rotation = Quaternion.Euler(35f, -25f, 0f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Space Shooter Created",
            "Created the SpaceShooter scene and its ship prefabs. Open Assets/Scenes/SpaceShooter/SpaceShooter.unity and press Play.",
            "Great");
    }

    private static GameObject BuildProjectile(Material material)
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        root.name = "Player Laser";
        root.transform.localScale = new Vector3(0.12f, 0.48f, 0.12f);
        root.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(root.GetComponent<Collider>());
        SphereCollider collider = root.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        root.AddComponent<SpaceShooterProjectile>();

        string path = PrefabFolder + "/PlayerLaser.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject BuildRocket(Material material)
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        root.name = "Fireworks Rocket";
        root.transform.localScale = new Vector3(0.2f, 0.38f, 0.2f);
        root.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(root.GetComponent<Collider>());
        SphereCollider collider = root.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        root.AddComponent<FireworksRocket>();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabFolder + "/FireworksRocket.prefab");
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject BuildPlayer(Material bodyMaterial, Material wingMaterial, GameObject projectilePrefab, GameObject rocketPrefab)
    {
        GameObject root = new GameObject("PlayerShip");
        AddVisual(root.transform, PrimitiveType.Sphere, "Hull", new Vector3(0f, 0f, 0f), new Vector3(0.52f, 1.05f, 0.28f), bodyMaterial);
        AddVisual(root.transform, PrimitiveType.Cube, "Left Wing", new Vector3(-0.62f, -0.18f, 0f), new Vector3(0.78f, 0.18f, 0.2f), wingMaterial, 0f, 0f, -18f);
        AddVisual(root.transform, PrimitiveType.Cube, "Right Wing", new Vector3(0.62f, -0.18f, 0f), new Vector3(0.78f, 0.18f, 0.2f), wingMaterial, 0f, 0f, 18f);
        AddVisual(root.transform, PrimitiveType.Sphere, "Nose", new Vector3(0f, 0.72f, 0f), new Vector3(0.3f, 0.55f, 0.25f), bodyMaterial);
        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = new Vector3(1.55f, 1.9f, 0.5f);
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        SpaceShooterPlayer player = root.AddComponent<SpaceShooterPlayer>();
        player.SetProjectilePrefab(projectilePrefab);
        player.SetFireworksRocketPrefab(rocketPrefab);

        string path = PrefabFolder + "/PlayerShip.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject BuildEnemy(Material bodyMaterial, Material wingMaterial)
    {
        GameObject root = new GameObject("EnemyShip");
        AddVisual(root.transform, PrimitiveType.Sphere, "Hull", Vector3.zero, new Vector3(0.62f, 0.72f, 0.3f), bodyMaterial);
        AddVisual(root.transform, PrimitiveType.Cube, "Left Wing", new Vector3(-0.58f, 0.08f, 0f), new Vector3(0.72f, 0.16f, 0.2f), wingMaterial, 0f, 0f, 20f);
        AddVisual(root.transform, PrimitiveType.Cube, "Right Wing", new Vector3(0.58f, 0.08f, 0f), new Vector3(0.72f, 0.16f, 0.2f), wingMaterial, 0f, 0f, -20f);
        AddVisual(root.transform, PrimitiveType.Sphere, "Tail", new Vector3(0f, -0.5f, 0f), new Vector3(0.28f, 0.4f, 0.22f), wingMaterial, 0f, 0f, 180f);
        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.isTrigger = true;
        collider.radius = 0.58f;
        collider.height = 1.45f;
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        root.AddComponent<SpaceShooterEnemy>();

        string path = PrefabFolder + "/EnemyShip.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static void AddVisual(Transform parent, PrimitiveType type, string objectName, Vector3 position,
        Vector3 scale, Material material, float xRotation = 0f, float yRotation = 0f, float zRotation = 0f)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = objectName;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        part.GetComponent<Renderer>().sharedMaterial = material;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -20f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.7f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.008f, 0.015f, 0.055f);
        cameraObject.AddComponent<AudioListener>();
        return camera;
    }

    private static void CreateStars(Material material)
    {
        Random.InitState(42);
        for (int i = 0; i < 145; i++)
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star " + (i + 1);
            star.transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-8f, 8f), 2.5f);
            float size = Random.Range(0.025f, 0.075f);
            star.transform.localScale = Vector3.one * size;
            star.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(star.GetComponent<Collider>());
        }
    }

    private static Canvas CreateHud(out Text scoreText, out Text livesText, out Text gameOverText)
    {
        GameObject canvasObject = new GameObject("HUD");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText = CreateLabel(canvasObject.transform, "Score", "SCORE  0", font,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(34f, -28f), new Vector2(440f, 70f), 30, TextAnchor.UpperLeft);
        livesText = CreateLabel(canvasObject.transform, "Hull", "HULL  3", font,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -28f), new Vector2(360f, 70f), 30, TextAnchor.UpperRight);
        gameOverText = CreateLabel(canvasObject.transform, "Game Over", "SHIP DESTROYED", font,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1000f, 260f), 54, TextAnchor.MiddleCenter);
        gameOverText.color = new Color(1f, 0.35f, 0.35f);
        gameOverText.gameObject.SetActive(false);

        CreateLabel(canvasObject.transform, "Controls", "MOVE WASD / ARROWS   LASER SPACE / CLICK   SPREAD 2   FIREWORKS ROCKET X",
            font, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 24f),
            new Vector2(1500f, 48f), 20, TextAnchor.MiddleCenter);

        return canvas;
    }

    private static Text CreateLabel(Transform parent, string objectName, string value, Font font,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject labelObject = new GameObject(objectName);
        labelObject.transform.SetParent(parent, false);
        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(anchorMin.x == 1f ? 1f : anchorMin.x, anchorMin.y == 1f ? 1f : anchorMin.y);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        Text label = labelObject.AddComponent<Text>();
        label.font = font;
        label.text = value;
        label.fontSize = fontSize;
        label.color = new Color(0.55f, 0.92f, 1f);
        label.alignment = alignment;
        label.raycastTarget = false;
        return label;
    }

    private static Material CreateMaterial(string materialName, Color color, float metallic, bool emission = false)
    {
        string path = MaterialFolder + "/" + materialName + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (emission && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.8f);
        }
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void EnsureFolder(string parent, string folderName)
    {
        string path = parent + "/" + folderName;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, folderName);
    }
}
