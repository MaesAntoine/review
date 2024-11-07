using UnityEngine;

[System.Serializable]
public class LevelThemeOverrides
{
    public bool overridePlatformMaterials;
    public Material defaultPlatformMaterial;
    public Material visitedPlatformMaterial;

    public bool overrideSkyColor;
    public Color skyColor;
}