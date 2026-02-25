This documentation provides a comprehensive technical overview of the Unity project, covering its architecture, systems, and implementation details.

## 1. Project Description
This project is a 2D action game where the player controls a ship that must survive waves of incoming enemies. The core mechanic revolves around an energy-based **Barrier** system that allows the player to reflect enemy bullets back at them. The experience is defined by high-intensity dodging and strategic energy management.

**Core Pillars:**
*   **Reflective Combat:** Turning enemy projectiles into the player's primary weapon.
*   **Energy Management:** Balancing barrier protection with movement and cooldowns.
*   **Physics-Based Movement:** Smooth, momentum-driven player control relative to the cursor.

## 2. Gameplay Flow / User Loop
1.  **Boot/Initialization:** The `Main.unity` scene loads, initializing the `PlayerController` and `EnemySpawner`.
2.  **Core Loop:**
    *   **Movement:** Player moves towards the mouse cursor using physics-based forces.
    *   **Spawning:** `EnemySpawner` generates enemies at the screen edges.
    *   **Combat:** Enemies fire projectiles at the player.
    *   **Defense/Offense:** Player activates the `Barrier` (consuming energy) to reflect bullets. Reflected bullets speed up and can destroy enemies.
3.  **Energy Recovery:** Energy depletes when the barrier is active and recovers when the player is stationary and the barrier is off.
4.  **Failure State:** If a non-reflected bullet or an enemy touches the player, they explode and the scene restarts after a short delay.

## 3. Architecture
The project follows a component-based architecture where individual entities handle their own logic, coordinated through tags and triggers.

*   **Input Handling:** Uses the Legacy Input Manager to track mouse position and button states.
*   **Physics Interaction:** Relies heavily on `Rigidbody2D` and `Collider2D` (Triggers) for collision detection and movement.
*   **Communication:** Systems interact primarily through Unity's `OnTriggerEnter2D` and tag-based filtering (`"Bullet"`, `"ReflectedBullet"`, `"Enemy"`, `"Player"`).
*   **State Management:** Localized within individual scripts (e.g., `isDead` in `PlayerController`, `isReflected` in `Bullet`).

## 4. Game Systems & Domain Concepts

### Movement System
Handles player and enemy traversal using 2D physics.
*   `PlayerController`: Applies forces towards the cursor with acceleration and rotation smoothing.
*   `Enemy`: Simple transform/velocity-based tracking towards the player.
*   **Pattern:** Force-based acceleration.
`Location: Assets/Scripts/`

### Combat & Reflection System
The primary interaction mechanic involving projectile redirection.
*   `Bullet`: Moves in a set direction; tracks its "reflected" state to determine what it can damage.
*   `Barrier`: A child object of the player that calculates reflection vectors using surface normals when hit by bullets.
*   `Enemy`: Fires bullets with a configurable spread at regular intervals.
`Location: Assets/Scripts/`

### Energy System
Governs the usage and recharge of the player's defensive capabilities.
*   `PlayerController`: Manages `currentEnergy`, depletion rates, and recovery conditions (stationary status).
*   `UIController`: Visualizes the energy state using the UITK framework.
`Location: Assets/Scripts/`

## 5. Scene Overview
*   **Main.unity:** The primary gameplay scene. It contains the environment, UI overlay, player prefab, and spawner.
*   **Scene Flow:** Currently, the project uses a single-scene loop. On player death, `PlayerController` calls `SceneManager.LoadScene` to restart the level.

## 6. UI System
The project utilizes **UI Toolkit (UITK)** for its interface.
*   `Main.uxml`: Defines the visual structure (progress bars, energy labels).
*   `UIController`: Queries the UXML tree to find elements (like an energy bar) and updates them every frame based on the `PlayerController`'s state.
*   `DefaultPanel.asset` / `DefaultTheme.tss`: Manage the rendering settings and styling for the UI.
`Location: Assets/UI/`

## 7. Asset & Data Model
*   **Prefabs:** Entities like `Bullet`, `Enemy`, and `Explosion` are stored as prefabs for dynamic instantiation during runtime.
*   **Sprites:** 2D textures used for all game entities, rendered via `SpriteRenderer`.
*   **URP Config:** Uses the Universal Render Pipeline with a `2D Renderer` asset to handle lighting and post-processing (if applicable).
*   **Naming Convention:** PascalCase for scripts and assets; descriptive names for prefabs.

## 8. Project Structure
```text
Assets/
├── Prefabs/        # Instantiable game entities (Enemy, Bullet, etc.)
├── Scripts/        # C# Logic and controllers
├── Sprites/        # Visual 2D assets
├── UI/             # UITK assets (uxml, tss, panel settings)
└── URP/            # Universal Render Pipeline configuration files
```

## 9. Notes, Caveats & Gotchas
*   **Reflection Logic:** When a bullet is reflected, its speed is doubled and its tag is changed to `ReflectedBullet`. This is crucial for avoiding self-collisions and allowing it to damage enemies.
*   **Energy Recovery:** The player *must* be stationary (velocity < 0.5) to recover energy. This forces a risk-reward trade-off between dodging and recharging.
*   **Physics Stability:** The `PlayerController` uses a `maxForce` cap in `MoveToCursor` to prevent "physics explosions" or jitter when the mouse is very far from the player.
*   **Cleanup:** Bullets and enemies have `OnBecameInvisible` logic to destroy themselves when off-screen to prevent memory leaks and performance degradation.