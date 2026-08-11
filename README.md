# MultiplayerPuzzle

MultiplayerPuzzle is a Unity 6 prototype for a physics-based puzzle game. The project currently focuses on a controllable player character, rigidbody movement, jumping, and a simple grab interaction for pushing, carrying, or manipulating puzzle objects.

The codebase is set up as a Unity project using the Universal Render Pipeline (URP), the Unity Input System package, and Unity's multiplayer tooling package as a foundation for future cooperative puzzle work.

## Screenshots

| Player Movement | Puzzle Interaction |
| --- | --- |
| ![Player movement](docs/screenshots/player-movement.png) | ![Puzzle interaction](docs/screenshots/puzzle-interaction.png) |

## Features

- Rigidbody-based player movement.
- Player rotation toward movement direction.
- Jumping with a grounded velocity check.
- Grab interaction using a forward raycast and `FixedJoint`.
- URP rendering setup.
- Sample scenes for experimenting with puzzle objects and player interaction.

## Requirements

- Unity Editor `6000.2.10f1`
- Unity Hub
- Git

Unity will restore package dependencies from `Packages/manifest.json` when the project is opened.

## Getting Started

1. Clone the repository.

   ```bash
   git clone https://github.com/Doner357/multiplayer-puzzle.git
   cd multiplayer-puzzle
   ```

2. Open the project in Unity Hub.

   - Select **Add project from disk**.
   - Choose this repository folder.
   - Open it with Unity `6000.2.10f1`.

3. Wait for Unity to import assets and restore packages.

4. Open a scene.

   - `Assets/Scenes/SampleScene.unity`
   - `Assets/_Scenes/Playground.unity`

5. Press **Play** in the Unity Editor.

## Controls

| Action | Input |
| --- | --- |
| Move | `WASD` or arrow keys |
| Jump | `Space` |
| Grab object | Hold `E` |
| Release object | Release `E` |

## Project Structure

```text
Assets/
  Prefabs/              Player and gameplay prefabs
  Scenes/               Main Unity scenes
  _Scenes/              Experimental playground scenes
  Scripts/              Runtime gameplay scripts
  Materials/            Project materials
  Settings/             URP and rendering assets
Packages/
  manifest.json         Unity package dependencies
ProjectSettings/
  ProjectSettings.asset Unity project configuration
```

## Core Scripts

- `Assets/Scripts/PlayerController.cs` handles movement, rotation, and jumping.
- `Assets/Scripts/GrabSystem.cs` handles grabbing rigidbody objects with a raycast and fixed joint.

## Building

1. Open the project in Unity.
2. Go to **File > Build Profiles**.
3. Select the target platform.
4. Add the scenes you want to include.
5. Click **Build**.

## Notes

- Multiplayer-specific gameplay is not yet fully implemented in the visible runtime scripts.
- The project is currently best treated as a foundation for cooperative puzzle mechanics and physics interaction experiments.
