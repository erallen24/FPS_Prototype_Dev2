using UnityEngine;

public class DropOff : MonoBehaviour
{
    [SerializeField] private Transform dropOffPoint;
    public Transform DropOffPoint => dropOffPoint;
    [SerializeField] private int dropOffDuration;
    [SerializeField] private GameObject parachute;



    private bool isDroppedOff = false;
    public bool IsDroppedOff => isDroppedOff;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDroppedOff)
        {
            moveTowardsDropOffPoint();
            checkForGround();
        }

    }

    private void checkForGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            // Slow down the object
            transform.position = hit.point;
            Destroy(parachute);

        }
    }
    private void moveTowardsDropOffPoint()
    {

        transform.position = Vector3.MoveTowards(transform.position, dropOffPoint.position, Time.deltaTime * dropOffDuration);
        if (Vector3.Distance(transform.position, dropOffPoint.position) < 0.1f)
        {
            isDroppedOff = true;

        }
    }

}
