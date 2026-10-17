# Corner Bounce

A casual, one-touch arcade game built with Unity. Swipe to launch a square across the screen, let it ricochet off the walls, and try to land it in a corner. Every corner hit scores a point — but friction slowly drains the square's speed, and when it stops, the run is over.

## Gameplay

1. Press **Play** on the start screen.
2. **Swipe** (click-drag with the mouse or drag with a finger) anywhere on the play area. The swipe direction and length set the launch velocity.
3. The square bounces off the screen edges and slows down due to friction.
4. Hit a **corner** to score a point, trigger a particle burst and play a sound.
5. When the square's speed falls below the stop threshold, it's **game over**. Your best score is saved between sessions.

Swipes shorter than a minimum distance are ignored, and swipes that start on UI elements don't launch the square.

## Features

- Swipe-to-launch physics with wall bounces and friction
- Corner detection with a short cooldown to prevent double-scoring
- Persistent best score (`PlayerPrefs`)
- Settings panel: sound on/off and adjustable play-area padding (0–60 px)
- Safe-area aware layout for notched / rounded-corner mobile screens
- Particle effects on corner hits (UI FX Particle System)

## Requirements

- **Unity 6000.3.15f1** (Unity 6.3)
- Packages (see `Packages/manifest.json`):
  - Universal Render Pipeline 17.3.0 (2D renderer)
  - Input System 1.19.0
  - Unity UI (uGUI) 2.0.0
  - TextMesh Pro

## Getting Started

1. Clone the repository:
   ```bash
   git clone <repo-url>
   ```
2. Open the project folder in Unity Hub using the editor version above.
3. Open `Assets/Scenes/Main.unity`.
4. Press **Play**.

The project targets mobile (touch) but is fully playable in the editor and on desktop with the mouse.

## Project Structure

```
Assets/
├── Scenes/          Main.unity (game scene) and prefabs
├── Scripts/         Game code (see below)
├── Settings/        URP / 2D renderer assets
├── Particles/       UI FX particle pack (third party)
├── ThirdAssets/     Colorful UI sprite pack (third party)
└── TextMesh Pro/    TMP resources
```

### Scripts

| Script | Responsibility |
| --- | --- |
| `GameManager` | Game state, score, best score, start / restart / game over flow |
| `BouncePhysics` | Swipe input, movement, friction, wall bounces, corner detection |
| `SettingManager` (`SettingsManager`) | Sound and play-area padding, saved via `PlayerPrefs` |
| `UIManager` | Shows / hides panels, score display, corner particles and sound |
| `StartPanel`, `PlayingPanel`, `GameOverPanel`, `UISettingPanel` | Individual screen UI controllers |
| `SafeAreaController` | Fits UI to the device safe area |

## Tuning

`BouncePhysics` exposes these values in the Inspector:

| Field | Default | Effect |
| --- | --- | --- |
| `launchMultiplier` | 0.02 | Converts swipe length (pixels) into launch speed |
| `friction` | 0.995 | Per-frame velocity multiplier; lower stops sooner |
| `stopThreshold` | 0.05 | Speed below which the run ends |
| `minSwipeDistance` | 25 | Minimum swipe length in pixels |
| `cornerDistance` | 0.2 | How close to a corner counts as a hit |

## Credits

Third-party assets are included under their respective licenses:

- UI FX – Particle System
- Colorful UI (font: Digitalt, OFL license)
- TextMesh Pro (Liberation Sans, OFL license)
