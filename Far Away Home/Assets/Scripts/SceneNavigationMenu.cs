using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Adds a small, always-available scene switcher to every loaded game scene.
/// Add new scene names to AvailableScenes as the arcade grows.
/// </summary>
public sealed class SceneNavigationMenu : MonoBehaviour
{
    private static readonly string[] AvailableScenes =
    {
        "Main_Scene",
        "Arcade_Mini_Game",
        "Space_Shooter_Arcade_Game"
    };

    private const string MenuTitle = "TRAVEL / ARCADE";
    private Canvas canvas;
    private GameObject panel;
    private Button[] sceneButtons;
    private int selectedSceneIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterForSceneLoads()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FindObjectOfType<SceneNavigationMenu>() != null) return;

        GameObject menuObject = new GameObject("Global Scene Navigation");
        menuObject.AddComponent<SceneNavigationMenu>();
    }

    private void Awake()
    {
        BuildInterface();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            bool shouldOpen = !panel.activeSelf;
            panel.SetActive(shouldOpen);
            if (shouldOpen)
                SelectFirstAvailableScene();
        }

        if (!panel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F2))
            SelectNextScene();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (sceneButtons != null && sceneButtons[selectedSceneIndex].interactable)
                sceneButtons[selectedSceneIndex].onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            panel.SetActive(false);
    }

    private void BuildInterface()
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject("Scene Navigation Canvas");
        canvasObject.transform.SetParent(transform, false);
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Button toggle = CreateButton(canvasObject.transform, "SCENES", font,
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-24f, -24f), new Vector2(190f, 58f), 22);
        toggle.onClick.AddListener(() =>
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
                SelectFirstAvailableScene();
        });

        panel = new GameObject("Scene Navigation Panel");
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-24f, -96f);
        panelRect.sizeDelta = new Vector2(390f, 390f);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.015f, 0.035f, 0.085f, 0.96f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 16, 16);
        layout.spacing = 10f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateHeading(panel.transform, MenuTitle, font);
        CreateHeading(panel.transform, "F2: SELECT   ENTER: LOAD", font);

        sceneButtons = new Button[AvailableScenes.Length];
        for (int i = 0; i < AvailableScenes.Length; i++)
        {
            string sceneName = AvailableScenes[i];
            string label = GetDisplayName(sceneName);
            if (SceneManager.GetActiveScene().name == sceneName)
                label += "  •  CURRENT";

            Button sceneButton = CreatePanelButton(panel.transform, label, font);
            string targetScene = sceneName;
            sceneButton.interactable = Application.CanStreamedLevelBeLoaded(targetScene);
            sceneButton.onClick.AddListener(() => LoadScene(targetScene));
            sceneButtons[i] = sceneButton;
        }

        Button closeButton = CreatePanelButton(panel.transform, "CLOSE", font);
        closeButton.onClick.AddListener(() => panel.SetActive(false));

        panel.SetActive(false);
    }

    private void SelectFirstAvailableScene()
    {
        selectedSceneIndex = 0;
        SelectNextInteractable(1, true);
    }

    private void SelectNextScene()
    {
        SelectNextInteractable(1, false);
    }

    private void SelectNextInteractable(int direction, bool includeCurrent)
    {
        if (sceneButtons == null || sceneButtons.Length == 0) return;

        int start = includeCurrent ? selectedSceneIndex - 1 : selectedSceneIndex;
        for (int step = 1; step <= sceneButtons.Length; step++)
        {
            int candidate = (start + step * direction + sceneButtons.Length) % sceneButtons.Length;
            if (!sceneButtons[candidate].interactable) continue;

            selectedSceneIndex = candidate;
            EventSystem.current?.SetSelectedGameObject(sceneButtons[candidate].gameObject);
            sceneButtons[candidate].Select();
            return;
        }
    }

    private static void LoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Scene Navigation: '" + sceneName +
                "' is not enabled in Build Settings. Run Tools > Far Away Home > Configure Scene Navigation.");
            return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private static string GetDisplayName(string sceneName)
    {
        switch (sceneName)
        {
            case "Main_Scene": return "MAIN HUB";
            case "Arcade_Mini_Game": return "ARCADE MINI GAME";
            case "Space_Shooter_Arcade_Game": return "SPACE SHOOTER";
            default: return sceneName.Replace('_', ' ').ToUpperInvariant();
        }
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;

        GameObject eventObject = new GameObject("Scene Navigation EventSystem");
        eventObject.AddComponent<EventSystem>();
        eventObject.AddComponent<StandaloneInputModule>();
    }

    private static void CreateHeading(Transform parent, string value, Font font)
    {
        GameObject labelObject = new GameObject(value);
        labelObject.transform.SetParent(parent, false);
        labelObject.AddComponent<RectTransform>();
        Text label = labelObject.AddComponent<Text>();
        label.font = font;
        label.text = value;
        label.fontSize = 20;
        label.fontStyle = FontStyle.Bold;
        label.color = new Color(0.45f, 0.9f, 1f);
        label.alignment = TextAnchor.MiddleCenter;
        LayoutElement element = labelObject.AddComponent<LayoutElement>();
        element.preferredHeight = 34f;
    }

    private static Button CreatePanelButton(Transform parent, string value, Font font)
    {
        Button button = CreateButton(parent, value, font, Vector2.zero, Vector2.one,
            Vector2.zero, new Vector2(0f, 48f), 18);
        LayoutElement element = button.gameObject.AddComponent<LayoutElement>();
        element.preferredHeight = 48f;
        return button;
    }

    private static Button CreateButton(Transform parent, string value, Font font,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, int fontSize)
    {
        GameObject buttonObject = new GameObject(value);
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin == Vector2.one ? Vector2.one : new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.035f, 0.19f, 0.31f, 0.98f);
        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.16f, 0.55f, 0.72f);
        colors.highlightedColor = new Color(0.25f, 0.78f, 0.92f);
        colors.pressedColor = new Color(0.08f, 0.35f, 0.52f);
        colors.disabledColor = new Color(0.18f, 0.2f, 0.24f, 0.65f);
        button.colors = colors;

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 3f);
        labelRect.offsetMax = new Vector2(-8f, -3f);
        Text label = labelObject.AddComponent<Text>();
        label.font = font;
        label.text = value;
        label.fontSize = fontSize;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        return button;
    }
}
