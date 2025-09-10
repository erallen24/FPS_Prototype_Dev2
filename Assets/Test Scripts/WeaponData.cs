using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Guns/GunData")]

public class WeaponData : ScriptableObject
{
    public enum GunType { Handgun, Shotgun, AutoRifle, Submachinegun, SniperRifle }

    public GunType gunType;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int magazineSize;
    public float reloadTime;
    public GameObject impactEffect;
    public float impactForce;
   

    public int recoil;

}
