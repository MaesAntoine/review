using UnityEngine;

[CreateAssetMenu(fileName = "GameTheme", menuName = "Game/Theme/GameTheme")]
public class GameTheme : ScriptableObject
{
    [Header("Platform Materials")]
    public Material defaultPlatformMaterial;
    public Material visitedPlatformMaterial;

    [Header("Environment")]
    public Color skyColor = Color.black;
    public Color ambientLight = Color.white;

    // Add other global theme settings as needed
}