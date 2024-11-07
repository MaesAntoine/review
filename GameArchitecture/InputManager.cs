using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameInputs gameInputs;
    private EventChannel eventChannel;
    private Camera mainCamera;

    public void Initialize(EventChannel channel)
    {
        Debug.Log("InputManager Initialize called");
        eventChannel = channel;
        eventChannel.OnGameStarted += HandleGameStarted;

        mainCamera = Camera.main;

        // Create and setup input actions
        gameInputs = new GameInputs();

        Debug.Log("Setting up input actions...");
        // Subscribe to input actions
        gameInputs.Player.Click.performed += HandleClick;

        // Enable the action map
        gameInputs.Player.Enable();
        Debug.Log("Input actions enabled");
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        Debug.Log("HandleClick called");

        Vector2 pointerPosition = Mouse.current.position.ReadValue();
        Debug.Log($"Click position: {pointerPosition}");

        eventChannel.RaisePointerClick(pointerPosition);
    }

    private void OnDestroy()
    {
        if (eventChannel != null)
        {
            eventChannel.OnGameStarted -= HandleGameStarted;
        }

        if (gameInputs != null)
        {
            gameInputs.Player.Click.performed -= HandleClick;
            gameInputs.Player.Disable();
            Debug.Log("Input actions disabled");
        }
    }

    private void HandleGameStarted()
    {
        Debug.Log("Input Manager ready");
    }
}