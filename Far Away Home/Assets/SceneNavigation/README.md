# Global Scene Navigation

A small **SCENES** button is created automatically whenever a scene loads. It opens a panel that lets the player switch between the hub and available arcade games.

## Keyboard and pointer controls

- **F1** — open the scene navigation panel.
- **F2** — move the selection to the next available scene.
- **Enter** — load the currently selected scene.
- **Escape** — close the panel.
- **SCENES button / destination buttons** — open the panel or choose a destination with the mouse or touchscreen.

F1 and F2 are used instead of Tab so they do not conflict with the game's existing Tab control. F2 cycles through available scene destinations; Enter confirms the highlighted destination.

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
6. Press Play in any of the three scenes. Press **F1** to open the menu, **F2** to cycle destinations, and **Enter** to load the highlighted scene.

The menu is bootstrapped at runtime, so it does not need to be manually placed in each scene. It uses a normal single-scene load, so the previous game is unloaded when switching.

## Adding future arcade games

1. Add the new scene filename to `AvailableScenes` in `Assets/Scripts/SceneNavigationMenu.cs`.
2. Add the matching name to `RequiredSceneNames` in `Assets/Editor/SceneNavigationBuildSettings.cs`.
3. Run **Configure Scene Navigation** again and commit the updated Build Settings.

The menu only enables destinations that Unity can load from Build Settings. This prevents a button from appearing usable when its scene is not included.

## Notes

- The menu uses Unity's built-in UI, legacy runtime font, and EventSystem. It does not modify the player controller or individual mini-game scripts.
- This change has not been run in the Unity Editor from this workspace. Please verify it in each scene and check the Console before merging.
