# Perspective matters

Platformer where the mechanics is about you, the player, looking from the right position at the right time.

note: I'm currently working on the architecture for a modular and lightweight experience.
I'm opting for single point entry with an event driven architecture (with centralized channel) and scriptable objects (both as data and asset).
Nothing is settled, and I'm also discovering some patterns as I go, which is why it's not as clean as it could be yet.

## Architecture flow at start (on going)

```mermaid
sequenceDiagram
    participant Launch
    participant GameManager
    participant EventChannel
    participant Components
    participant LevelManager
    participant UI

    Launch->>GameManager: 1. Initialize Core
    Note right of GameManager: Singleton setup and validation

    GameManager->>EventChannel: 2. Setup Event Channel
    Note right of EventChannel: Core event system ready

    GameManager->>Components: 3. Initialize Components
    Note right of Components: ThemeManager, InputManager initialized

    Components->>EventChannel: 4. Subscribe to Events
    Note right of EventChannel: Components listening for events

    GameManager->>UI: 5. Initialize UI Components
    Note right of UI: UI systems ready

    UI->>EventChannel: 6. Subscribe to UI Events
    Note right of EventChannel: UI listening for game events

    GameManager->>EventChannel: 7. Trigger Game Start
    Note right of EventChannel: RaiseGameInitialized & RaiseGameStarted

    EventChannel->>LevelManager: 8. Load Initial Level
    Note right of LevelManager: Load Level 0, instantiate platforms
```
