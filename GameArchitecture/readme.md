# Perspective matters

Platformer where the mechanics is about you, the player, looking from the right position at the right time.

note: I'm currently working on the architecture for a modular and lightweight experience.
I'm opting for single point entry with an event driven architecture (with centralized channel) and scriptable objects (both as data and asset).
Nothing is settled, and I'm also discovering some patterns as I go, which is why it's not as clean as it could be yet.

## Architecture flow at start (on going)

```mermaid
sequenceDiagram
    participant GameManager
    participant EventChannel
    participant LevelManager
    participant PlatformBehaviour

    %% Core Initialization
    GameManager->>LevelManager: Initialize(events, levels, prefab, theme)
    LevelManager->>EventChannel: Subscribe OnLevelLoad

    %% Game Start
    GameManager->>EventChannel: RaiseGameInitialized
    GameManager->>EventChannel: RaiseGameStarted
    GameManager->>EventChannel: RaiseLevelLoad(0)

    %% Level Setup
    EventChannel->>LevelManager: LoadLevel(0)
    LevelManager->>LevelManager: Clear Current Level
    loop Each Platform in Level
        LevelManager->>PlatformBehaviour: Instantiate & Initialize
        PlatformBehaviour->>EventChannel: Subscribe Events
    end

    %% Game Loop
    Note over GameManager,PlatformBehaviour: Game Running
    PlatformBehaviour->>EventChannel: RaisePlatformVisited
    EventChannel->>LevelManager: Check Win Condition
```
