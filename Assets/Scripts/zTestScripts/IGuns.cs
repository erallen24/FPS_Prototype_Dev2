using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IGuns : MonoBehaviour
{
    [SerializeField] private WeaponData WeaponData;
    [SerializeField] TMP_Text playerAmmo;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] public LayerMask ignoreLayer;

    private float shootTimer;
    public int currBulletsInMag;
    private bool isReloading;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //currBulletsInMag = weaponData.magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShoot();
        GameManager.instance.updatePlayerAmmo(currBulletsInMag, WeaponData.ammoMax);

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }
    }

    private void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && CheckIfGunCanShoot() && shootTimer >= WeaponData.shootRate)
        {
            Shoot();

        }
        else if (currBulletsInMag <= 0 && !isReloading)
        {
            AttemptReload();
        }
    }

    private void Shoot()
    {
        // resetting the shoot timer //
        shootTimer = 0;
        currBulletsInMag--;
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
        if (currBulletsInMag <= 0)
            return false;

        if (isReloading)
            return false;

        return true;
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;

        if (reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(reloadSound, transform.position);
        }

        yield return new WaitForSeconds(WeaponData.reloadTime);
        currBulletsInMag = WeaponData.ammoMax;
        isReloading = false;
    }

    private void AttemptReload()
    {
        if (isReloading || currBulletsInMag >= WeaponData.ammoMax)
            return;

        StartCoroutine(ReloadSequence());
    }

    private void Recoil()
    {
        int recoil = WeaponData.recoil;
    }


}
