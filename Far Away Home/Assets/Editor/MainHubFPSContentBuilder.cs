using System;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarAwayHome.EditorTools
{
    public static class MainHubFPSContentBuilder
    {
        const string MainScenePath = "Assets/Main_Scene.unity";
        const string ProjectileSourcePath = "Assets/FPS/Prefabs/Projectiles/Projectile_Disc.prefab";
        const string WeaponSourcePath = "Assets/FPS/Prefabs/Weapons/Weapon_Launcher.prefab";
        const string PickupSourcePath = "Assets/FPS/Prefabs/Pickups/Pickup_Launcher.prefab";
        const string ProjectileOutputPath = "Assets/FPS/Prefabs/Projectiles/Projectile_FireworksRocket.prefab";
        const string WeaponOutputPath = "Assets/FPS/Prefabs/Weapons/Weapon_FireworksRocket.prefab";
        const string PickupOutputPath = "Assets/FPS/Prefabs/Pickups/Pickup_FireworksRocket.prefab";

        [MenuItem("Tools/Far Away Home/Set Up Main Hub FPS Variety + Fireworks Rocket")]
        static void BuildMainHubFPSContent()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.path != MainScenePath)
            {
                EditorUtility.DisplayDialog("Main Hub FPS Setup",
                    "Open Far Away Home/Assets/Main_Scene.unity first. This setup intentionally edits only Main_Scene.",
                    "OK");
                return;
            }

            try
            {
                ProjectileBase rocketProjectile = CreateRocketProjectile();
                WeaponController rocketWeapon = CreateRocketWeapon(rocketProjectile);
                WeaponPickup rocketPickup = CreateRocketPickup(rocketWeapon);

                PlaceRocketPickup(scene, rocketPickup);
                ApplyEnemyVariants(scene);

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Main Hub FPS Setup Complete",
                    "Added Scout, Heavy, and Veteran variants to selected HoverBots and placed the Fireworks Rocket pickup near the existing launcher pickup. Review the scene and test in Play Mode.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Main Hub FPS Setup Failed",
                    "The setup stopped with an error. Check the Console before continuing.\n\n" + exception.Message,
                    "OK");
            }
        }

        static ProjectileBase CreateRocketProjectile()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(ProjectileSourcePath);
            try
            {
                ProjectileStandard standard = root.GetComponent<ProjectileStandard>();
                if (standard == null)
                    throw new InvalidOperationException("The Disc projectile prefab has no ProjectileStandard component.");

                AudioClip impactSound = standard.ImpactSfxClip;
                UnityEngine.Object.DestroyImmediate(standard, true);

                FireworksRocketProjectile rocket = root.AddComponent<FireworksRocketProjectile>();
                rocket.Speed = 18f;
                rocket.Radius = 0.12f;
                rocket.MaxLifetime = 8f;
                rocket.FireworksLifetime = 2f;
                rocket.FireworksParticleCount = 90;
                rocket.ExplosionSfx = impactSound;

                PrefabUtility.SaveAsPrefabAsset(root, ProjectileOutputPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.ImportAsset(ProjectileOutputPath);
            ProjectileBase result = AssetDatabase.LoadAssetAtPath<ProjectileBase>(ProjectileOutputPath);
            if (result == null)
                throw new InvalidOperationException("Could not load the generated fireworks projectile prefab.");
            return result;
        }

        static WeaponController CreateRocketWeapon(ProjectileBase projectile)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(WeaponSourcePath);
            try
            {
                WeaponController weapon = root.GetComponent<WeaponController>();
                if (weapon == null)
                    throw new InvalidOperationException("The launcher prefab has no WeaponController.");

                weapon.WeaponName = "Fireworks Rocket";
                weapon.ProjectilePrefab = projectile;
                weapon.MaxAmmo = 3;
                weapon.ClipSize = 3;
                weapon.DelayBetweenShots = 1.2f;
                weapon.AutomaticReload = true;
                PrefabUtility.SaveAsPrefabAsset(root, WeaponOutputPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.ImportAsset(WeaponOutputPath);
            WeaponController result = AssetDatabase.LoadAssetAtPath<WeaponController>(WeaponOutputPath);
            if (result == null)
                throw new InvalidOperationException("Could not load the generated fireworks weapon prefab.");
            return result;
        }

        static WeaponPickup CreateRocketPickup(WeaponController weapon)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PickupSourcePath);
            try
            {
                WeaponPickup pickup = root.GetComponent<WeaponPickup>();
                if (pickup == null)
                    throw new InvalidOperationException("The launcher pickup prefab has no WeaponPickup.");

                pickup.WeaponPrefab = weapon;
                PrefabUtility.SaveAsPrefabAsset(root, PickupOutputPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.ImportAsset(PickupOutputPath);
            WeaponPickup result = AssetDatabase.LoadAssetAtPath<WeaponPickup>(PickupOutputPath);
            if (result == null)
                throw new InvalidOperationException("Could not load the generated fireworks pickup prefab.");
            return result;
        }

        static void PlaceRocketPickup(Scene scene, WeaponPickup pickupPrefab)
        {
            if (GameObject.Find("Pickup_FireworksRocket") != null)
                return;

            GameObject launcherPickup = GameObject.Find("Pickup_Launcher");
            GameObject player = GameObject.Find("Player");
            Vector3 position = launcherPickup != null
                ? launcherPickup.transform.position + Vector3.right * 2f
                : player != null
                    ? player.transform.position + player.transform.forward * 2f
                    : Vector3.zero;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(pickupPrefab.gameObject, scene);
            instance.name = "Pickup_FireworksRocket";
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.identity;
        }

        static void ApplyEnemyVariants(Scene scene)
        {
            GameObject[] objects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int hoverBotIndex = 0;
            foreach (GameObject candidate in objects)
            {
                if (candidate.scene != scene || !candidate.name.StartsWith("Enemy_HoverBot", StringComparison.Ordinal))
                    continue;

                // Leave every other HoverBot untouched, preserving the original baseline enemy population.
                if (hoverBotIndex % 2 == 0)
                {
                    MainHubEnemyVariant variant = candidate.GetComponent<MainHubEnemyVariant>();
                    if (variant == null)
                        variant = Undo.AddComponent<MainHubEnemyVariant>(candidate);

                    int archetype = (hoverBotIndex / 2) % 3;
                    variant.Variant = (MainHubEnemyVariant.VariantType)archetype;
                    EditorUtility.SetDirty(candidate);
                }

                hoverBotIndex++;
            }
        }
    }
}
