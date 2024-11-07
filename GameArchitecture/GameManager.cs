using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("Core Systems")]
    [SerializeField] private EventChannel eventChannel;
    [SerializeField] private Camera mainCameraPrefab;

    [Header("Configuration")]
    [SerializeField] private GameConfiguration gameConfig;

    private InputManager inputManager;
    private LevelManager levelManager;
    private Camera mainCamera;

    #region Initialization

    private void Awake()
    {
        InitializeSingleton();
        InitializeCoreSystems();
        InitializeEventSystem();
        InitializeComponents();
    }

    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCoreSystems()
    {
        InitializeCamera();
        InitializeLevelManager();
    }

    private void InitializeCamera()
    {
        mainCamera = Instantiate(mainCameraPrefab, transform);
    }

    private void InitializeLevelManager()
    {
        levelManager = gameObject.AddComponent<LevelManager>();
        levelManager.Initialize(
            eventChannel,
            gameConfig.levels,
            gameConfig.themeManagerPrefab
        );
    }

    private void InitializeComponents()
    {
        InitializeInputManager();
        SignalInitializationComplete();
    }

    private void InitializeInputManager()
    {
        if (gameConfig?.inputManagerPrefab != null)
        {
            inputManager = Instantiate(gameConfig.inputManagerPrefab, transform);
            inputManager.Initialize(eventChannel);
        }
        else
        {
            Debug.LogError("InputManager prefab not assigned in GameConfiguration!");
        }
    }

    private void SignalInitializationComplete()
    {
        eventChannel.RaiseGameInitialized();
    }

    private void InitializeEventSystem()
    {
        eventChannel.OnGameInitialized += HandleGameInitialized;
        eventChannel.OnGameStarted += HandleGameStarted;
        eventChannel.OnGamePaused += HandleGamePaused;
        eventChannel.OnGameResumed += HandleGameResumed;
    }

    #endregion

    #region Event Handlers

    private void HandleGameInitialized()
    {
        Debug.Log("Game Initialized");
        eventChannel.RaiseGameStarted();
    }

    private void HandleGameStarted()
    {
        Debug.Log("Game Started");
        eventChannel.RaiseLevelLoad(0);
    }

    private void HandleGamePaused()
    {
        Debug.Log("Game Paused");
        Time.timeScale = 0f;
    }

    private void HandleGameResumed()
    {
        Debug.Log("Game Resumed");
        Time.timeScale = 1f;
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        if (eventChannel != null)
        {
            eventChannel.OnGameInitialized -= HandleGameInitialized;
            eventChannel.OnGameStarted -= HandleGameStarted;
            eventChannel.OnGamePaused -= HandleGamePaused;
            eventChannel.OnGameResumed -= HandleGameResumed;
        }

        if (instance == this)
        {
            instance = null;
        }
    }

    #endregion

    #region Public Access

    public EventChannel Events => eventChannel;
    public GameConfiguration Config => gameConfig;

    #endregion
}