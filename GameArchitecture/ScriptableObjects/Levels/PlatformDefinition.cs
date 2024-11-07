using UnityEngine;

[CreateAssetMenu(fileName = "Platform", menuName = "Game/Platform Definition")]
public class PlatformDefinition : ScriptableObject
{
    public GameObject prefab;
    public bool useCustomMaterial;
    public Material customMaterial;
    // Add any platform-specific gameplay properties here
}