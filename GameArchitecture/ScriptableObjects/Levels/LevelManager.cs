using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private EventChannel eventChannel;
    private List<GameObject> currentPlatforms = new List<GameObject>();
    private List<LevelConfig> levels;
    private GameObject platformPrefab;
    private ThemeManager themeManager; // Will be set from GameConfig

    public void Initialize(EventChannel channel, List<LevelConfig> levelConfigs, ThemeManager themeManagerRef)
    {
        eventChannel = channel;
        levels = levelConfigs;
        themeManager = themeManagerRef;

        eventChannel.OnLevelLoad += LoadLevel;
    }

    private void LoadLevel(int levelIndex)
    {
        ClearCurrentLevel();

        LevelConfig levelConfig = null;
        if (levelConfig == null) return;

        foreach (var platformInstance in levelConfig.platformInstances)
        {
            if (platformInstance.platformType != null && platformInstance.platformType.prefab != null)
            {
                GameObject platform = Instantiate(platformInstance.platformType.prefab);
                platform.transform.position = platformInstance.position;
                platform.transform.rotation = Quaternion.Euler(platformInstance.rotation);
                platform.transform.localScale = platformInstance.scale;

                var behaviour = platform.GetComponent<PlatformBehaviour>();
                if (behaviour != null)
                {
                    behaviour.platformType = platformInstance.platformType;
                    behaviour.instanceId = platformInstance.instanceId;
                    behaviour.Initialize(eventChannel); // No longer passing ThemeManager here
                }

                currentPlatforms.Add(platform);
            }
        }

        // Apply theme using themeManager reference from GameConfig
        Camera.main.backgroundColor = themeManager.GetSkyColor(levelConfig);
    }

    private void ClearCurrentLevel()
    {
        foreach (var platform in currentPlatforms)
        {
            if (platform != null)
                Destroy(platform);
        }
        currentPlatforms.Clear();
    }

    private void OnDestroy()
    {
        if (eventChannel != null)
        {
            eventChannel.OnLevelLoad -= LoadLevel;
        }
    }
}