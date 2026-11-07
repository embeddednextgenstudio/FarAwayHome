using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates a reusable, low-poly desktop computer prefab.
/// Run from Tools > Far Away Home > Create Matrix Computer Prefab.
/// </summary>
public static class MatrixComputerPrefabBuilder
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const string MaterialFolder = "Assets/Materials/MatrixComputer";
    private const string PrefabPath = PrefabFolder + "/MatrixComputer.prefab";

    [MenuItem("Tools/Far Away Home/Create Matrix Computer Prefab")]
    public static void CreatePrefab()
    {
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets/Materials", "MatrixComputer");

        Material caseMaterial = GetOrCreateMaterial(
            MaterialFolder + "/Computer_Case.mat",
            new Color(0.075f, 0.09f, 0.11f), 0.12f);
        Material trimMaterial = GetOrCreateMaterial(
            MaterialFolder + "/Computer_Trim.mat",
            new Color(0.19f, 0.22f, 0.25f), 0.22f);
        Material keyMaterial = GetOrCreateMaterial(
            MaterialFolder + "/Keyboard_Keys.mat",
            new Color(0.035f, 0.045f, 0.055f), 0.08f);
        Material screenMaterial = GetOrCreateMaterial(
            MaterialFolder + "/Screen_Glass.mat",
            new Color(0.004f, 0.018f, 0.012f), 0.08f);

        GameObject root = new GameObject("Matrix Computer");
        root.transform.position = Vector3.zero;

        // Monitor housing and inset glass.
        CreateCube(root.transform, "Monitor Housing",
            new Vector3(0f, 1.48f, 0.02f), new Vector3(1.92f, 1.42f, 0.16f), caseMaterial);
        CreateCube(root.transform, "Monitor Bezel",
            new Vector3(0f, 1.49f, -0.071f), new Vector3(1.78f, 1.12f, 0.025f), trimMaterial);

        GameObject screen = new GameObject("Matrix Screen");
        screen.transform.SetParent(root.transform, false);
        screen.transform.localPosition = new Vector3(0f, 1.49f, -0.09f);
        CreateCube(screen.transform, "Dark Screen Surface",
            Vector3.zero, new Vector3(1.66f, 0.99f, 0.012f), screenMaterial);

        AddMatrixColumns(screen.transform);
        screen.AddComponent<MatrixRainDisplay>();

        // Monitor stand and weighted foot.
        CreateCube(root.transform, "Stand Neck",
            new Vector3(0f, 0.78f, 0.12f), new Vector3(0.24f, 0.58f, 0.22f), trimMaterial);
        CreateCube(root.transform, "Stand Foot",
            new Vector3(0f, 0.16f, -0.02f), new Vector3(0.92f, 0.12f, 0.58f), caseMaterial);

        // Keyboard body, key rows and space bar.
        CreateCube(root.transform, "Keyboard Body",
            new Vector3(0f, 0.19f, -0.58f), new Vector3(1.52f, 0.09f, 0.48f), caseMaterial);

        for (int row = 0; row < 4; row++)
        {
            int keysInRow = row == 3 ? 10 : 14;
            float startX = row == 3 ? -0.57f : -0.66f;
            float spacing = row == 3 ? 0.127f : 0.096f;

            for (int column = 0; column < keysInRow; column++)
            {
                float x = startX + column * spacing;
                float z = -0.43f - row * 0.105f;
                CreateCube(root.transform, "Key " + row + "-" + column,
                    new Vector3(x, 0.25f, z),
                    new Vector3(row == 3 ? 0.105f : 0.075f, 0.025f, 0.065f),
                    keyMaterial);
            }
        }

        CreateCube(root.transform, "Space Bar",
            new Vector3(0f, 0.25f, -0.78f), new Vector3(0.48f, 0.025f, 0.065f), keyMaterial);

        // Mouse.
        GameObject mouse = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mouse.name = "Mouse";
        mouse.transform.SetParent(root.transform, false);
        mouse.transform.localPosition = new Vector3(1.02f, 0.24f, -0.57f);
        mouse.transform.localScale = new Vector3(0.19f, 0.09f, 0.29f);
        SetMaterial(mouse, trimMaterial);
        RemoveCollider(mouse);

        // Small status light on the lower bezel.
        CreateCube(root.transform, "Power LED",
            new Vector3(0.78f, 0.91f, -0.085f), new Vector3(0.035f, 0.035f, 0.012f),
            GetOrCreateMaterial(MaterialFolder + "/Power_LED.mat",
                new Color(0.08f, 1f, 0.32f), 1.5f));

        // A single root collider makes the complete prop easy to target/select.
        BoxCollider rootCollider = root.AddComponent<BoxCollider>();
        rootCollider.center = new Vector3(0.05f, 1.05f, -0.12f);
        rootCollider.size = new Vector3(2.75f, 2.2f, 1.35f);

        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
        Debug.Log("Matrix Computer prefab created at " + PrefabPath);
    }

    private static void AddMatrixColumns(Transform screen)
    {
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        const int columnCount = 17;
        for (int i = 0; i < columnCount; i++)
        {
            GameObject columnObject = new GameObject("Matrix Column " + i.ToString("00"));
            columnObject.transform.SetParent(screen, false);
            columnObject.transform.localPosition =
                new Vector3(-0.75f + i * 0.094f, 0.43f, -0.018f);
            columnObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            TextMesh text = columnObject.AddComponent<TextMesh>();
            text.text = MakePreviewText();
            text.fontSize = 24;
            text.characterSize = 0.043f;
            text.lineSpacing = 0.78f;
            text.anchor = TextAnchor.UpperCenter;
            text.alignment = TextAlignment.Center;
            text.color = new Color(0.25f, 1f, 0.48f);
            if (font != null)
                text.font = font;

            MeshRenderer renderer = columnObject.GetComponent<MeshRenderer>();
            if (renderer != null && font != null)
                renderer.sharedMaterial = font.material;
        }
    }

    private static string MakePreviewText()
    {
        const string characters = "01ABCDEF#$%&*+-=<>";
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        for (int i = 0; i < 11; i++)
        {
            builder.Append(characters[Random.Range(0, characters.Length)]);
            if (i < 10)
                builder.Append('\n');
        }

        return builder.ToString();
    }

    private static GameObject CreateCube(Transform parent, string name,
        Vector3 position, Vector3 scale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        SetMaterial(part, material);
        RemoveCollider(part);
        return part;
    }

    private static void SetMaterial(GameObject target, Material material)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = material;
    }

    private static void RemoveCollider(GameObject target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);
    }

    private static Material GetOrCreateMaterial(string path, Color color, float emission)
    {
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
            return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.name = Path.GetFileNameWithoutExtension(path);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0.32f);
        if (emission > 0f && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emission);
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
