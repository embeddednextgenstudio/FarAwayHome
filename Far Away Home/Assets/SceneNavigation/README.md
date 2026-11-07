# Global Scene Navigation

A small **SCENES** button is created automatically whenever a scene loads. It opens a panel that lets the player switch between the hub and available arcade games.

## Controls

- **F1** — open or close the scene navigation panel.
- **SCENES button** — open or close the panel with the mouse or touchscreen.
- **Escape** — close the panel.

F1 is used instead of Tab so it does not conflict with the game's existing Tab control.

## Included destinations

- `Main_Scene` — main hub
- `Arcade_Mini_Game`
- `Space_Shooter_Arcade_Game`

## Setup in Unity

1. Pull this branch and let Unity compile.
2. Confirm all three scene assets are saved somewhere under `Assets/` and their filenames match the names above exactly.
3. Choose **Tools → Far Away Home → Configure Scene Navigation**.
4. Check the Console for any scene names that could not be found.
5. Save/commit the updated `ProjectSettings/EditorBuildSettings.asset`.
6. Press Play in any of the three scenes. The **SCENES** button appears in the upper-right corner. Press **F1** to open the menu.

The menu is bootstrapped at runtime, so it does not need to be manually placed in each scene. It uses a normal single-scene load, so the previous game is unloaded when switching.

## Adding future arcade games

1. Add the new scene filename to `AvailableScenes` in `Assets/Scripts/SceneNavigationMenu.cs`.
2. Add the matching name to `RequiredSceneNames` in `Assets/Editor/SceneNavigationBuildSettings.cs`.
3. Run **Configure Scene Navigation** again and commit the updated Build Settings.

The menu only enables destinations that Unity can load from Build Settings. This prevents a button from appearing usable when its scene is not included.

## Notes

- The menu uses Unity's built-in UI, legacy runtime font, and EventSystem. It does not modify the player controller or individual mini-game scripts.
- This change has not been run in the Unity Editor from this workspace. Please verify it in each scene and check the Console before merging.
