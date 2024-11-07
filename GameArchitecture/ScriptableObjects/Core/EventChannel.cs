using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EventChannel", menuName = "Game/EventChannel")]
public class EventChannel : ScriptableObject
{
    // Core game flow
    public event Action OnGameInitialized;
    public event Action OnGameStarted;
    public event Action OnGamePaused;
    public event Action OnGameResumed;

    // Platform events
    public event Action<string> OnPlatformVisited;

    // Level management
    public event Action<int> OnLevelLoad;
    public event Action OnLevelStart;
    public event Action OnLevelComplete;

    // Input events
    public event Action<Vector2> OnCameraRotationInput;
    public event Action<Vector3> OnPlatformSelected;
    public event Action<Vector2> OnPointerClick;  // Vector2 for click position
    public event Action<Vector2> OnPointerHold;   // Optional - for drag operations
    public event Action OnPointerRelease;         // Optional - for drag operations

    // Methods to raise events
    public void RaiseGameInitialized() => OnGameInitialized?.Invoke();
    public void RaiseGameStarted() => OnGameStarted?.Invoke();
    public void RaiseGamePaused() => OnGamePaused?.Invoke();
    public void RaiseGameResumed() => OnGameResumed?.Invoke();

    public void RaisePlatformVisited(string guid) => OnPlatformVisited?.Invoke(guid);

    public void RaiseLevelLoad(int levelIndex) => OnLevelLoad?.Invoke(levelIndex);
    public void RaiseLevelStart() => OnLevelStart?.Invoke();
    public void RaiseLevelComplete() => OnLevelComplete?.Invoke();

    public void RaiseCameraRotationInput(Vector2 rotation) => OnCameraRotationInput?.Invoke(rotation);
    public void RaisePlatformSelected(Vector3 position) => OnPlatformSelected?.Invoke(position);
    public void RaisePointerClick(Vector2 position) => OnPointerClick?.Invoke(position);
    public void RaisePointerHold(Vector2 position) => OnPointerHold?.Invoke(position);
    public void RaisePointerRelease() => OnPointerRelease?.Invoke();
}