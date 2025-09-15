using UnityEngine;

[CreateAssetMenu(fileName = "NewXReModule", menuName = "XRe/Module")]
public class XReModule : ScriptableObject
{
    [Header("Module Identity")]
    public string moduleName;
    public string description;
    public Sprite icon;

    [Header("Minimap Layer Activation")]
    public string[] layersToEnable;

    [Header("Overlay Modules")]
    public GameObject[] overlayObjects;

    [Header("Lore Feedback")]
    [TextArea]
    public string activationMessage;
    public AudioClip activationSound;
}

