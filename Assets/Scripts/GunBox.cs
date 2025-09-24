using System.Collections;
using UnityEngine;

public class GunBox : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject pivotPoint;
    [SerializeField] GameObject gunToDrop;
    [SerializeField] Vector3 gunDropOffset;
    [SerializeField] Vector3 rotDir;
    [SerializeField] int rotSpeed;


    Quaternion targetRot;

    Collider interactCollider;
    bool isRotating = false;
    bool used;

    void Start()
    {
        interactCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (isRotating)
        {
            pivotPoint.transform.localRotation = Quaternion.RotateTowards(pivotPoint.transform.localRotation, targetRot, rotSpeed * Time.deltaTime);

            if (pivotPoint.transform.localRotation == targetRot)
            {
                isRotating = false;
            }
        }

        
    }

    public void Interact()
    {
        targetRot = Quaternion.Euler(rotDir);
        isRotating = true;
        interactCollider.enabled = false;
        StartCoroutine(SpawnGun());

    }

    IEnumerator SpawnGun()
    {
        yield return new WaitForSeconds(1);
        if(gunToDrop != null)
        {
            Instantiate(gunToDrop, transform.position + gunDropOffset, Quaternion.identity);
        }
    }


   
}
