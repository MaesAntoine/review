using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] private GameTheme defaultTheme;
    private EventChannel eventChannel;

    private void Awake()
    {
        eventChannel = FindAnyObjectByType<GameManager>().Events;
    }

    public Material GetDefaultPlatformMaterial(LevelConfig levelConfig = null)
    {
        if (levelConfig != null &&
            levelConfig.useCustomTheme &&
            levelConfig.themeOverrides.overridePlatformMaterials)
        {
            return levelConfig.themeOverrides.defaultPlatformMaterial;
        }
        return defaultTheme.defaultPlatformMaterial;
    }

    public Material GetVisitedPlatformMaterial(LevelConfig levelConfig = null)
    {
        if (levelConfig != null &&
            levelConfig.useCustomTheme &&
            levelConfig.themeOverrides.overridePlatformMaterials)
        {
            return levelConfig.themeOverrides.visitedPlatformMaterial;
        }
        return defaultTheme.visitedPlatformMaterial;
    }

    public Color GetSkyColor(LevelConfig levelConfig = null)
    {
        if (levelConfig != null &&
            levelConfig.useCustomTheme &&
            levelConfig.themeOverrides.overrideSkyColor)
        {
            return levelConfig.themeOverrides.skyColor;
        }
        return defaultTheme.skyColor;
    }
}