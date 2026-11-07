# Space Shooter Scene

This is an isolated arcade-style space-combat prototype for Far Away Home. It does not replace or modify the existing game scenes.

## Create the scene in Unity

1. Switch to the `feature/space-shooter-scene` branch and pull the latest changes.
2. Open the project in Unity 6.0 (the project version is 6000.6.4f1).
3. Wait for Unity to finish compiling.
4. In the Unity menu, choose **Tools → Far Away Home → Create Space Shooter Scene**.
5. Open `Assets/Scenes/SpaceShooter/SpaceShooter.unity` and press Play.

The editor builder generates the ship prefabs and materials under `Assets/SpaceShooter/`. Those generated assets are created by your local Unity Editor, not by GitHub.

## Controls

- **Move:** WASD or arrow keys
- **Fire:** Space bar or hold the left mouse button

## Prototype features

- A blue player ship with bounded movement
- Repeating cyan laser shots
- Red enemy ships that spawn above the play area and chase the player
- Enemy destruction and score increments
- Three hull points and a game-over message
- Starfield, camera, lighting, and a simple HUD

## Notes

- The scene is intentionally built from Unity primitive meshes, so it does not need external art assets.
- The scene builder creates a standalone test scene. Add it to Build Settings only when you decide you want it included in a build.
- This change has not been run in the Unity Editor from this workspace. Please test the generated scene locally and share any Console errors before merging it into your main branch.
