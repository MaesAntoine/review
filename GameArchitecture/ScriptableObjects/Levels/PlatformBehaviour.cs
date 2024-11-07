using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    public PlatformDefinition platformType;
    public string instanceId;
    private bool isVisited;
    private MeshRenderer meshRenderer;
    private EventChannel eventChannel;

    // ThemeManager no longer needed here - will get it from GameConfiguration
    public void Initialize(EventChannel channelRef)
    {
        eventChannel = channelRef;
        meshRenderer = GetComponent<MeshRenderer>();

        UpdateMaterial();
    }

    public void Visit()
    {
        if (!isVisited)
        {
            isVisited = true;
            UpdateMaterial();
            eventChannel.RaisePlatformVisited(instanceId);
        }
    }

    private void UpdateMaterial()
    {
        var themeManager = GameManager.Instance.Config.themeManagerPrefab; // Get from GameConfig

        if (platformType.useCustomMaterial)
        {
            meshRenderer.material = platformType.customMaterial;
        }
        else
        {
            meshRenderer.material = isVisited ?
                themeManager.GetVisitedPlatformMaterial() :
                themeManager.GetDefaultPlatformMaterial();
        }
    }
}