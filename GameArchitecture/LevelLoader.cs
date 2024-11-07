using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private GameManager gameManager;
    private EventChannel eventChannel;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        eventChannel = gameManager.Events;

        eventChannel.OnLevelLoad += LoadLevel;
    }

    public void LoadLevel(int levelIndex)
    {
        // Basic level loading
        Debug.Log($"Loading level {levelIndex}");
        eventChannel.RaiseLevelLoad(levelIndex);
    }
}
