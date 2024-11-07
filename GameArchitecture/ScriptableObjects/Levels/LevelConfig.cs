using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Level Settings")]
    public string levelName;
    public int levelIndex;
    public int platformsRequiredToWin;

    [Header("Theme")]
    public bool useCustomTheme;
    public LevelThemeOverrides themeOverrides;

    [Header("Platform Layout")]
    public List<PlatformInstance> platformInstances = new List<PlatformInstance>();
}