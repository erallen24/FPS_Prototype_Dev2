using UnityEngine;

[CreateAssetMenu(fileName = "gun", menuName = "Guns/GunData")]

public class WeaponData : ScriptableObject
{
    public GameObject model;
    [SerializeField]
    public enum GunType { Handgun, Shotgun, AutoRifle, Submachinegun, SniperRifle }
    public AudioClip pickUpSound;

    public AudioClip emptyClipSound;
    public AudioClip reloadSound;
    public AudioClip[] shootSounds;
    [Range(0, 1)] public float shootVolume = 0.5f; // Volume of the shooting sound

    public GunType gunType;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCur;
    [Range(5, 50)] public int ammoMax; // Maximum ammo capacity
    public float reloadTime;
    public ParticleSystem impactEffect;
    public float impactForce;


    public int recoil;


}
