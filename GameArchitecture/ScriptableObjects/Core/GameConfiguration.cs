using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "Game/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [Header("Core Systems")]
    public InputManager inputManagerPrefab;

    [Header("Settings")]
    public ThemeManager themeManagerPrefab;

    [Header("Level Settings")]
    public List<LevelConfig> levels;
}
