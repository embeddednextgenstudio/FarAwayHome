using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Finds the known hub/arcade scenes by filename and enables them in Build Settings.
/// Existing Build Settings entries are preserved.
/// </summary>
public static class SceneNavigationBuildSettings
{
    private static readonly string[] RequiredSceneNames =
    {
        "Main_Scene",
        "Arcade_Mini_Game",
        "Space_Shooter_Arcade_Game"
    };

    [MenuItem("Tools/Far Away Home/Configure Scene Navigation")]
    public static void Configure()
    {
        Dictionary<string, EditorBuildSettingsScene> scenesByPath =
            new Dictionary<string, EditorBuildSettingsScene>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            scenesByPath[scene.path] = scene;

        List<string> missing = new List<string>();

        foreach (string sceneName in RequiredSceneNames)
        {
            string[] guids = AssetDatabase.FindAssets(sceneName + " t:Scene");
            string matchingPath = null;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == sceneName)
                {
                    matchingPath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(matchingPath))
            {
                missing.Add(sceneName);
                continue;
            }

            scenesByPath[matchingPath] =
                new EditorBuildSettingsScene(matchingPath, true);
        }

        EditorBuildSettings.scenes = new List<EditorBuildSettingsScene>(
            scenesByPath.Values).ToArray();

        if (missing.Count > 0)
        {
            Debug.LogWarning("Scene Navigation: these scenes were not found in the project and were not added: " +
                string.Join(", ", missing.ToArray()) +
                ". Make sure each scene asset is saved under Assets.");
        }

        EditorUtility.DisplayDialog(
            "Scene Navigation Configured",
            "Enabled the available hub and arcade scenes in Build Settings. " +
            "Missing scenes, if any, are listed in the Console. " +
            "Save/commit the updated ProjectSettings/EditorBuildSettings.asset file.",
            "OK");
    }
}
