# Main Hub FPS enemy variety and Fireworks Rocket

This feature is scoped to `Assets/Main_Scene.unity` and the FPS framework. It does not modify the FPS sample scene or either arcade shooter.

## Setup

1. Open `Far Away Home/Assets/Main_Scene.unity` in Unity.
2. Use **Tools → Far Away Home → Set Up Main Hub FPS Variety + Fireworks Rocket**.
3. Review the generated prefabs under `Assets/FPS/Prefabs` and the placed pickup in the scene.
4. Enter Play Mode and verify the enemy archetypes, weapon pickup, projectile collision, bot defeat, and fireworks particles.

The editor command creates a rocket from the existing launcher and disc projectile assets, then adds a pickup beside the existing launcher pickup. It also assigns Scout, Heavy, and Veteran stat profiles to alternating HoverBots. The untouched HoverBots and existing Turrets remain as baseline enemy types.

## Gameplay

- **Scout:** lower health, faster movement, quicker firing, and slightly wider detection.
- **Heavy:** substantially higher health, slower movement, and slower firing.
- **Veteran:** increased health, movement, detection, and faster firing.
- **Fireworks Rocket:** three-round launcher pickup. On impact it triggers a colorful particle burst and defeats all active FPS bots in the scene. It does not damage the player.

## Notes

The setup command requires `Main_Scene` to be the active scene so that it cannot accidentally edit the FPS sample scene or an arcade scene. Run the command only once; it checks for an existing fireworks pickup and avoids placing a duplicate. The generated prefab assets can be inspected and adjusted before committing.

Unity Editor / Play Mode testing is required. This repository-editing environment cannot run the Unity Editor or validate a player build.
