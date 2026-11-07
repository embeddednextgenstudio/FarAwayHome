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
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(
            EditorBuildSettings.scenes);
        HashSet<string> knownPaths = new HashSet<string>();
        foreach (EditorBuildSettingsScene scene in scenes)
            knownPaths.Add(scene.path);

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

            if (knownPaths.Contains(matchingPath))
            {
                for (int i = 0; i < scenes.Count; i++)
                {
                    if (scenes[i].path == matchingPath)
                    {
                        scenes[i] = new EditorBuildSettingsScene(matchingPath, true);
                        break;
                    }
                }
            }
            else
            {
                scenes.Add(new EditorBuildSettingsScene(matchingPath, true));
                knownPaths.Add(matchingPath);
            }
        }

        EditorBuildSettings.scenes = scenes.ToArray();

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
