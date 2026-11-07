# Space Shooter Scene

Standalone arcade shooter prototype. Create or rebuild it with **Tools → Far Away Home → Create Space Shooter Scene** in Unity 6.

## Controls
- Move: WASD or arrow keys
- Standard laser: hold Space or left mouse button
- Spread-shot gun: press **2** to switch to a three-laser fan
- Single laser: press **1**
- Fireworks super-rocket: press **X** (7-second cooldown)

## Enemy classes
- **Scout (orange):** small, fast, one-hit enemy with weaving movement
- **Hunter (purple):** two-hit enemy that strafes while pursuing the player
- **Tank (cyan):** large, slow, four-hit enemy worth more points

## Fireworks super-rocket
The X rocket launches upward and detonates near the top of the playfield (or when it reaches an enemy). Its colourful fireworks burst destroys every currently active enemy and awards bonus score for each one. The rocket has a seven-second cooldown.

The scene is generated from Unity primitives and runtime particle effects; no external art or sound assets are required.

This feature has not been run in the Unity Editor in this workspace. Rebuild the scene using the menu item above and check the Unity Console and Play mode before merging.
