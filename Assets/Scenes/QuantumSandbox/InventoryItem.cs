using UnityEngine;

public class InventoryItem : MonoBehaviour
{

    // enum for item types
    public enum ItemType
    {
        AdrenalineShot,
        Shield,
        EMP,
        CloakingDevice,
        StunGrenade,
        Bomb,
        Weapon,
        Armor,
        Accessory,
        Misc
    }

    public ItemType Type;
    public string Name;
    public Sprite Icon;
    public string Description;

}
