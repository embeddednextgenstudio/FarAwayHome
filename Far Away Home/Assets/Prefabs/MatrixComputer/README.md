# Matrix Computer prefab

This is a standalone prop. It does not modify scenes, player scripts, input, or the house-building code.

## Create the prefab

1. Open the Unity project and let Unity finish importing the new scripts.
2. In the top menu, choose **Tools > Far Away Home > Create Matrix Computer Prefab**.
3. Unity creates `Assets/Prefabs/MatrixComputer.prefab` and its materials under `Assets/Materials/MatrixComputer/`.
4. Find `MatrixComputer.prefab` in the Project window and drag it into any scene.
5. Position and rotate it like any other prefab.

## What is included

- Low-poly monitor, stand, keyboard, individual keycaps and mouse.
- Dark screen with animated green random characters.
- A single root BoxCollider for straightforward object targeting.
- A separate `MatrixRainDisplay` component. It uses Unity's built-in `TextMesh`, so TextMeshPro is not required.

## Notes

- The prefab builder is an Editor-only tool. It runs only when you choose the menu command; it does not execute automatically in gameplay.
- The animated text runs when the prefab is active in Play Mode.
- The model is built from Unity primitive meshes, so there are no external model downloads or imported mesh dependencies.
- If the project uses a custom font, you can change the font assigned in `MatrixComputerPrefabBuilder.cs`.
