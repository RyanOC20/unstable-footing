# Unstable Footing

A 3D multiplayer arena melee fighting game built in Unity. Players battle on a dynamically shifting arena, and the last player standing wins.

---

## Gameplay

- **Combat** — Click to swing a melee attack with a 2.5m range and directional knockback.
- **Movement** — WASD to move, Space to jump. Ice, mud, and fire tiles alter movement and add hazards.
- **Stocks** — Each player starts with 3 stocks. Fall into the water and lose a stock. Lose all stocks and you're eliminated.
- **Arena Chaos** — The arena's platforms shift, lower, and raise over time. Chaos accelerates as the match progresses.

---

## Networking

Built on [PurrNet](https://github.com/PurrNet/PurrNet).

---

## Getting Started

1. Clone the repository.
2. Open the project in Unity (2022.3 LTS or later recommended).
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press Play.

To test multiplayer locally, use **Unity Multiplayer Play Mode** (included as a package dependency) to run multiple editor instances, or build and run a standalone client alongside the editor.

---

## Project Structure

```
Assets/
├── Animations/       # Animator controller and animation clips
├── Materials/        # Surface and terrain materials
├── Models/           # 3D model assets
├── Prefabs/          # Player, arena block, and UI prefabs
├── Scenes/           # MainMenu, LobbySelection, WaitingRoom, Arena
├── Scripts/          # All game logic (see below)
├── Sprites/          # UI and crosshair graphics
└── Stylized Lava Materials/  # Lava/water hazard visuals
```

### Scripts

| Script | Responsibility |
|---|---|
| `PlayerMovement.cs` | WASD movement, jump, gravity, knockback |
| `PlayerAttack.cs` | Melee attack raycasting and cooldown |
| `PlayerAnimator.cs` | Animation parameter sync |
| `PlayerLook.cs` | Mouse-look camera controller |
| `PlayerModel.cs` | Hides own body mesh in first-person |
| `PlayerRespawn.cs` | Fall detection and respawn positioning |
| `PlayerStocks.cs` | Stock tracking and elimination |
| `PlayerSpawner.cs` | Spawns player prefabs at network join |
| `ArenaManager.cs` | Platform chaos system and match timer |
| `blockMover.cs` | Per-tile terrain and animation logic |
| `GameManager.cs` | Game-over state and winner determination |
| `StocksHUD.cs` | In-match stock icon display |
| `Crosshair.cs` | Generates the on-screen crosshair overlay |
| `GameOverUI.cs` | Win/lose screen and scene cleanup |
| `MainMenuUI.cs` | Main menu button handlers |
| `LobbySelectionUI.cs` | Host/join flow and connection state |
| `WaitingRoomUI.cs` | Lobby player list and start button |
| `PlayerSlotUI.cs` | Individual player slot in the lobby |
| `NetworkUI.cs` | Connection feedback and scene transition |
| `SingletonEventSystem.cs` | Prevents duplicate EventSystem on scene load |

---

## Credits

### Player 3D Model

The player character 3D model is used under the [Creative Commons Attribution 4.0 International (CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/) license.

> **[Model Name]** by **[Author Name]**  
> Source: [Link to original asset](https://sketchfab.com/3d-models/proportional-low-poly-man-free-download-0bfd0e2b49a348a4b64b20cc8196e3b3)
> License: [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)  
> Changes were made to the original model.

---

## License

This project's source code is provided as-is. Third-party assets retain their respective licenses as noted in the Credits section above.
