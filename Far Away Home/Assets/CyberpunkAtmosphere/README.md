# Cyberpunk atmosphere

This feature adds a runtime URP global volume to every gameplay scene without editing existing scene files or the project's render-pipeline assets.

## What it changes

- ACES tonemapping for a cinematic contrast curve
- Cooler cyan/teal shadows and restrained magenta highlights
- Subtle bloom to make emissive/neon lights glow
- A dark vignette, light chromatic aberration, and fine film grain
- Enables URP post-processing on gameplay cameras and includes the global volume layer in their volume masks

## Setup

No manual scene setup is required. The `CyberpunkAtmosphere` bootstrap creates one persistent global volume before the first scene loads and reapplies camera settings whenever a scene is loaded.

The effect is implemented for the project's Universal Render Pipeline (URP). It does not change materials, lights, gameplay, audio, or scene geometry.

## Test in Unity

1. Open the project in Unity 6.
2. Let scripts compile, then enter Play mode in the hub scene.
3. Check the hub and each arcade scene using the global scene selector.
4. Build and run a standalone player to confirm the same post-processing appears there.

If the grade is too strong for a particular scene, adjust the values in `CyberpunkAtmosphere.CreateGlobalVolume`. Bloom is intentionally moderate so ordinary white surfaces do not all become glowing.
