using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("CAMERA CONTROLLER SETTINGS")]
    [Space(10)]
    [SerializeField] [Range(0, 250)] private int sensitivity;
    [Space(10)]
    [SerializeField] [Range(-90, 0)] private int xRotationMin;
    [SerializeField] [Range(0, 90)] private int xRotationMax;
    [Space(10)]
    [SerializeField] private bool invertY;


    private float targetFieldOfView;
    private float fieldOfViewSpeed;
    private float targetCameraHeight;
    private float targetCameraHeightSpeed;
    private float xRotation;


    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateCameraLook();
        UpdateCameraFOV();
        UpdateCameraHeight();
    }

    private void Initialize()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UpdateCameraLook()
    {
        // calculate and store look input //
        float lookX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float lookY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        // use invertY to give options to look up/down //
        if (invertY)
        {
            xRotation += lookY;
        }
        else
        {
            xRotation -= lookY;
        }

        // clamp camera rotation on X axis //
        xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);

        // rotate the camera to look up and down //
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate the player to look left and right //
        transform.parent.Rotate(Vector3.up * lookX);
    }

    private void UpdateCameraFOV()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFieldOfView, Time.deltaTime * fieldOfViewSpeed);
    }

    private void UpdateCameraHeight()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, targetCameraHeight, 0), Time.deltaTime * targetCameraHeightSpeed);
    }

    public void SetFieldOfView(float fov, float speed)
    {
        targetFieldOfView = fov;
        fieldOfViewSpeed = speed;
    }

    public void SetCameraHeight(float height, float speed)
    {
        targetCameraHeight = height;
        targetCameraHeightSpeed = speed;
    }
}
