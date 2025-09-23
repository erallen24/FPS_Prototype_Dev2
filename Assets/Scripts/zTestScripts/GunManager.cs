using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    [SerializeField] private WeaponData WeaponData;
    [SerializeField] TMP_Text playerAmmo;
    [SerializeField] public LayerMask ignoreLayer;

    public List<WeaponData> gunList = new List<WeaponData>();
    private int gunListPos;
    private float shootTimer;
    private bool isReloading;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        WeaponData.ammoCur = WeaponData.ammoMax;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShoot();
        //SelectGun();
        HUDManager.instance.updatePlayerAmmo(WeaponData.ammoCur, WeaponData.ammoMax);

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }
    }

    public void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && CheckIfGunCanShoot() && shootTimer >= WeaponData.shootRate)
        {
            Shoot();

        }
        else if (WeaponData.ammoCur <= 0 && !isReloading)
        {
            AttemptReload();
        }
    }

    private void Shoot()
    {
        // resetting the shoot timer //
        shootTimer = 0;
        WeaponData.ammoCur--;
        Recoil();
        performShoot();

    }

    private void performShoot()
    {
        // performing shoot raycast //
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, WeaponData.shootDistance, ~ignoreLayer))
        {
            // logging the collider the raycast hit //
            Debug.Log(hit.collider.name);

            // if the collider has the IDamage interface, we store it in 'target'
            IDamage target = hit.collider.GetComponent<IDamage>();

            // null check on the target. if target is not null, we call 'TakeDamage'
            target?.TakeDamage(WeaponData.shootDamage);

            if (WeaponData.impactEffect != null)
            {
                //ParticleSystem impactGO = Instantiate(WeaponData.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(impactGO, 2f);
                //The  Impact Effect is created in the WeaponData Scriptable Object as a particle System
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * WeaponData.impactForce);
            }
        }
    }

    private bool CheckIfGunCanShoot()
    {
        return WeaponData.ammoCur > 0 && !isReloading;

        //if (WeaponData.ammoCur <= 0)
        //    return false;

        //if (isReloading)
        //    return false;

        //return true;
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;

        if (WeaponData.reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(WeaponData.reloadSound, transform.position);
        }

        yield return new WaitForSeconds(WeaponData.reloadTime);
        WeaponData.ammoCur = WeaponData.ammoMax;
        isReloading = false;
    }

    public void AttemptReload()
    {
        if (isReloading || WeaponData.ammoCur >= WeaponData.ammoMax)
            return;

        StartCoroutine(ReloadSequence());
    }

    private void Recoil()
    {
        int recoil = WeaponData.recoil;
    }


    //public void ChangeGun()
    //{
    //   WeaponData.shootDamage = gunList[gunListPos].shootDamage;
    //    WeaponData.shootDistance = gunList[gunListPos].shootDistance;
    //    WeaponData.shootRate = gunList[gunListPos].shootRate;

    //    WeaponData.gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
    //    WeaponData.gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

    //    SoundManager.instance.soundSource.PlayOneShot(gunList[gunListPos].pickUpSound);


    //}

    //public void SelectGun()
    //{
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
    //    {
    //        gunListPos++;
    //        ChangeGun();
    //    }
    //    else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
    //    {
    //        gunListPos--;
    //        ChangeGun();
    //    }

    //}


}
