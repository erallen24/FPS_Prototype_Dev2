using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IGuns : MonoBehaviour
{
    enum gunType { Handgun, Shotgun, AutoRifle, Submachinegun, SniperRifle }

    [SerializeField] private gunType currGunType;
    [SerializeField] private int shootDamage;
    [SerializeField] private int shootDistance;
    [SerializeField] private float shootRate;
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime;
    [SerializeField] TMP_Text playerAmmo;
    [SerializeField] LayerMask ignoreLayer;

    private float shootTimer;
    public int currBulletsInMag;
    private bool isReloading;
    public GameObject impactEffect;

    public float impactForce;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currBulletsInMag = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShoot();
        GameManager.instance.updatePlayerAmmo(currBulletsInMag, magazineSize);

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }
    }

    private void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && CheckIfGunCanShoot() && shootTimer >= shootRate)
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
        performShoot();

    }

    private void performShoot()
    {
        // performing shoot raycast //
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, shootDistance, ~ignoreLayer))
        {
            // logging the collider the raycast hit //
            Debug.Log(hit.collider.name);

            // if the collider has the IDamage interface, we store it in 'target'
            IDamage target = hit.collider.GetComponent<IDamage>();

            // null check on the target. if target is not null, we call 'TakeDamage'
            target?.TakeDamage(shootDamage);

            if (impactEffect != null) 
            {
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
            
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
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
        yield return new WaitForSeconds(reloadTime);
        currBulletsInMag = magazineSize;
        isReloading = false;
    }

    private void AttemptReload()
    {
        if (isReloading || currBulletsInMag >= magazineSize)
            return;

        StartCoroutine(ReloadSequence());
    }


}
