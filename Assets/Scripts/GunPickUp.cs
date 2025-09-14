using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    public WeaponData gun;

    public float rotateSpeed = 50f; // Speed at which the pickup rotates for visibility
    public float pulseSpeed = 2f; // Speed of the pulsing effect
    public float pulseMagnitude = 0.1f; // Magnitude of the pulsing effect
    private float pulseTimer = 0f; // Timer for pulsing effect


    private Vector3 originalPosition;
    private Vector3 initialScale; // Initial scale of the pickup for pulsing effect
    private Vector3 initialRotation;
    private Quaternion rotation;

    private void Start()
    {
        originalPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.eulerAngles;
        rotation = Quaternion.Euler(initialRotation);
    }

    private void Update()
    {
        // Optional: Add any rotation or animation to the pickup object for visual effect
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime); // Rotate around the Y-axis
        // Pulsing effect with up and down movement
        pulseTimer += Time.deltaTime * pulseSpeed;
        float scaleFactor = 1 + Mathf.Sin(pulseTimer) * pulseMagnitude;
        transform.localScale = initialScale * scaleFactor;
        transform.position = originalPosition + new Vector3(0, Mathf.Sin(pulseTimer) * pulseMagnitude, 0); // Adjust the Y position for pulsing effect


    }
    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null)
        {
            if (gun != null)
            {

                //if (pickupable.hasGun(gun)))
                //    return; // Player already has this gun, do not pick up again
                pickupable.GetGunStats(gun);
                gun.ammoCur = gun.ammoMax; // Refill ammo when picked up

            }

            //else if (playerUpgrade != null)
            //{
            //    pickupable.GetPickUp(playerUpgrade); // Call the getPickUp method on the pickupable object
            //}


        }
        Destroy(gameObject); // Destroy the pickup object after being picked up
    }
}