using UnityEngine;
using System.Collections;

public class PickUpWeapons : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gun, camera;


    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Update()
    {
        //Check if in range and "E" is pressed
        Vector3 distToPlayer = player.position - transform.position;
        if (!equipped && distToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Drop and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }




    private void PickUp ()
    {
        equipped = true;
        slotFull = true;

        //Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;

        
    }

    private void Drop()
    {

    }

}
