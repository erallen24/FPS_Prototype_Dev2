using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        // use invertY to give options to look up/down
        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }

        // clamp camera on X axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate the camera to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate player to look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
