using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Applies a restrained cyan-and-magenta cyberpunk grade to every gameplay scene.
/// The volume is created at runtime so existing scenes and render-pipeline assets
/// do not need to be edited.
/// </summary>
public sealed class CyberpunkAtmosphere : MonoBehaviour
{
    private static CyberpunkAtmosphere instance;

    private Volume globalVolume;
    private VolumeProfile runtimeProfile;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
            return;

        var atmosphereObject = new GameObject("Cyberpunk Atmosphere");
        instance = atmosphereObject.AddComponent<CyberpunkAtmosphere>();
        DontDestroyOnLoad(atmosphereObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        CreateGlobalVolume();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ApplyPostProcessingToCameras();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyPostProcessingToCameras();
    }

    private void CreateGlobalVolume()
    {
        runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        runtimeProfile.name = "Runtime Cyberpunk Atmosphere";

        var tonemapping = runtimeProfile.Add<Tonemapping>(true);
        tonemapping.mode.value = TonemappingMode.Neutral;

        var colorAdjustments = runtimeProfile.Add<ColorAdjustments>(true);
        colorAdjustments.postExposure.value = 1.0f;
        colorAdjustments.contrast.value = 5f;
        colorAdjustments.saturation.value = 10f;
        colorAdjustments.colorFilter.value = new Color(0.92f, 0.97f, 1f, 1f);

        var splitToning = runtimeProfile.Add<SplitToning>(true);
        splitToning.shadows.value = new Color(0.10f, 0.18f, 0.24f, 1f);
        splitToning.highlights.value = new Color(0.38f, 0.18f, 0.40f, 1f);
        splitToning.balance.value = 15f;

        var bloom = runtimeProfile.Add<Bloom>(true);
        bloom.threshold.value = 1.3f;
        bloom.intensity.value = 0.65f;
        bloom.scatter.value = 0.72f;
        bloom.tint.value = new Color(0.62f, 0.84f, 1f, 1f);

        var vignette = runtimeProfile.Add<Vignette>(true);
        vignette.color.value = new Color(0.015f, 0.01f, 0.045f, 1f);
        vignette.intensity.value = 0.06f;
        vignette.smoothness.value = 0.48f;
        vignette.rounded.value = true;

        var chromaticAberration = runtimeProfile.Add<ChromaticAberration>(true);
        chromaticAberration.intensity.value = 0.015f;

        var filmGrain = runtimeProfile.Add<FilmGrain>(true);
        filmGrain.intensity.value = 0.06f;
        filmGrain.response.value = 0.8f;

        globalVolume = gameObject.AddComponent<Volume>();
        globalVolume.isGlobal = true;
        globalVolume.priority = 100f;
        globalVolume.weight = 1f;
        globalVolume.sharedProfile = runtimeProfile;
        globalVolume.blendDistance = 0f;
        globalVolume.gameObject.layer = 0;
    }

    private void ApplyPostProcessingToCameras()
    {
        // The global volume is on the Default layer. Make sure every gameplay
        // camera can see it and has URP post-processing enabled.
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera gameplayCamera in cameras)
        {
            if (gameplayCamera.cameraType != CameraType.Game)
                continue;

            UniversalAdditionalCameraData cameraData =
                gameplayCamera.GetComponent<UniversalAdditionalCameraData>();

            if (cameraData == null)
                cameraData = gameplayCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();

            cameraData.renderPostProcessing = true;
            cameraData.volumeLayerMask |= 1 << gameObject.layer;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;

        if (runtimeProfile != null)
            Destroy(runtimeProfile);
    }
}
