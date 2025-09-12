using UnityEngine;
public interface IPickup
{
    public void GetGunStats(WeaponData gun);
    public void GetPickUp(Pickup upgrade);
}