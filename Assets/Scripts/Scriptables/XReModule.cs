using UnityEngine;

[CreateAssetMenu(fileName = "NewXReModule", menuName = "XRe/Module")]
public class XReModule : ScriptableObject
{
    public enum ModuleType
    {
        None,
        Tactical,
        Environmental,
        Support,
        Offensive,
        Defensive,
        Recon,
        Temporal,
        Stabilization,
        Experimental
    }

    [Header("Module Identity")]
    public GameObject modelPrefab; // Reference to the 3D model prefab
    public ModuleType moduleType;
    public string moduleName;
    public string description;
    public Sprite icon;
    public Sprite reticalImage;




    [Header("Lore Feedback")]
    [TextArea]
    public string activationMessage;
    public AudioClip activationSound;
}

