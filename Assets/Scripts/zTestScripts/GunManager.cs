using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    [Header("SETTINGS")]
    [Space(10)]
    [SerializeField] private Renderer weaponModel;
    [SerializeField] private WeaponData currentWeaponData;
    [Space(10)]
    [SerializeField] public LayerMask shootIgnoreLayer;
    [Space(10)]
    public List<WeaponData> gunList = new List<WeaponData>();

    private int gunListPos;
    private float shootTimer;
    private bool isReloading;

    private Vector3 targetRecoilRotation;
    private Vector3 targetRecoilPosition;

    private AdvancedPlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<AdvancedPlayerController>();
        HUDManager.instance.DeactivateAmmoUI();
    }
    private void Update()
    {
        UpdateShoot();
        SelectGun();
        UpdateCurrentWeaponAmmoUI();

        if (Input.GetKeyDown(KeyCode.R))
        {
            AttemptReload();
        }
    }
    private void LateUpdate()
    {
        LateUpdateRecoil(playerController.WeaponRecoilPivot, playerController.MasterIK);
    }


    public void GetGunStats(WeaponData gunStat, inventoryItem gun)
    {
        if (playerController.HasItem(gun)) { return; }

        gunList.Add(gunStat);
        gunListPos = gunList.Count - 1;

        playerController.AddItem(gun);
        HUDManager.instance.ActivateAmmoUI();

        ChangeGun();
    }
    private void ChangeGun()
    {
        currentWeaponData = gunList[gunListPos];

        weaponModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        SoundManager.instance.soundSource.PlayOneShot(gunList[gunListPos].pickUpSound);
    }
    private void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();
        }

    }

    private bool CheckIfGunCanShoot()
    {
        return currentWeaponData.ammoCur > 0 && !isReloading;

        //if (WeaponData.ammoCur <= 0)
        //    return false;

        //if (isReloading)
        //    return false;

        //return true;
    }
    private void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (gunList.Count > 0)
        {
            if (Input.GetButton("Fire1") && CheckIfGunCanShoot() && shootTimer >= currentWeaponData.shootRate)
            {
                Shoot();

            }
            else if (currentWeaponData.ammoCur <= 0 && !isReloading)
            {
                AttemptReload();
            }
        }
    }
    private void Shoot()
    {
        // resetting the shoot timer //
        shootTimer = 0;
        currentWeaponData.ammoCur--;
        PerformShoot();
        SetTargetRecoilMultipliers();
    }
    private void PerformShoot()
    {
        // performing shoot raycast //
        if (Physics.Raycast(playerController.PlayerCamera.transform.position, playerController.PlayerCamera.transform.forward, out RaycastHit hit, currentWeaponData.shootDistance, ~shootIgnoreLayer))
        {
            // logging the collider the raycast hit //
            Debug.Log(hit.collider.name);

            // if the collider has the IDamage interface, we store it in 'target'
            IDamage target = hit.collider.GetComponent<IDamage>();

            // null check on the target. if target is not null, we call 'TakeDamage'
            target?.TakeDamage(currentWeaponData.shootDamage);

            if (currentWeaponData.impactEffect != null)
            {
                //ParticleSystem impactGO = Instantiate(WeaponData.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(impactGO, 2f);
                //The  Impact Effect is created in the WeaponData Scriptable Object as a particle System
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * currentWeaponData.impactForce);
            }
        }
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;

        if (currentWeaponData.reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(currentWeaponData.reloadSound, transform.position);
        }

        yield return new WaitForSeconds(currentWeaponData.reloadTime);
        currentWeaponData.ammoCur = currentWeaponData.ammoMax;
        isReloading = false;
    }
    public void AttemptReload()
    {
        if (gunList.Count > 0)
        {
            if (isReloading || currentWeaponData.ammoCur >= currentWeaponData.ammoMax)
                return;

            StartCoroutine(ReloadSequence());
        }
    }

    private void UpdateCurrentWeaponAmmoUI()
    {
        if (gunList.Count > 0)
        {
            HUDManager.instance.updatePlayerAmmo(currentWeaponData.ammoCur, currentWeaponData.ammoMax);
        }
    }

    private void LateUpdateRecoil(Transform recoilPivot, Transform masterIK)
    {
        if (gunList.Count > 0)
        {
            float positionX = recoilPivot.localPosition.x + currentWeaponData.recoilXPositionCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilPosition.x;
            float positionY = recoilPivot.localPosition.y + currentWeaponData.recoilYPositionCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilPosition.y;
            float positionZ = recoilPivot.localPosition.z + currentWeaponData.recoilZPositionCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilPosition.z;

            float rotationX = recoilPivot.localRotation.x + currentWeaponData.recoilXRotationCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilRotation.x;
            float rotationY = recoilPivot.localRotation.y + currentWeaponData.recoilYRotationCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilRotation.y;
            float rotationZ = recoilPivot.localRotation.z + currentWeaponData.recoilZRotationCurve.Evaluate(shootTimer * currentWeaponData.recoilPlayRate) * targetRecoilRotation.z;

            Vector3 recoilPosition = new(positionX, positionY, positionZ);
            Vector3 recoilRotation = new(rotationX, rotationY, rotationZ);

            recoilPivot.localPosition = Vector3.Lerp(masterIK.localPosition, recoilPosition, Time.deltaTime * currentWeaponData.recoilPositionSpeed);
            recoilPivot.localRotation = Quaternion.Slerp(masterIK.localRotation, Quaternion.Euler(recoilRotation), Time.deltaTime * currentWeaponData.recoilRotationSpeed);
        }
    }
    private void SetTargetRecoilMultipliers()
    {
        targetRecoilRotation = new(currentWeaponData.recoilXRotationMultiplier, Random.Range(-currentWeaponData.recoilYRotationMultiplier, currentWeaponData.recoilYRotationMultiplier), Random.Range(-currentWeaponData.recoilZRotationMultiplier, currentWeaponData.recoilZRotationMultiplier));
        targetRecoilPosition = new(Random.Range(-currentWeaponData.recoilXPositionMultiplier, currentWeaponData.recoilXPositionMultiplier), currentWeaponData.recoilYPositionMultiplier, currentWeaponData.recoilZPositionMultiplier);
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
