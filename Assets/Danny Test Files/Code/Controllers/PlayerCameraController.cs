using Player.Utilities;
using UnityEngine;

public class PlayerCameraController
{
    private readonly AdvancedPlayerController playerController;
    private readonly PlayerInputController inputController;
    private readonly Transform cameraRig;
    private float cameraRigXRotation;

    public PlayerCameraController(AdvancedPlayerController player, PlayerCameraControllerSettings settings)
    {
        playerController = player;  
        inputController = player.InputController;
        cameraRig = settings.cameraRigTransform;
    }

    public void Update()
    {
        UpdateCameraRigRotation();
    }
    public void LateUpdate()
    {
        LateUpdateCameraRigPosition();
    }


    private void UpdateCameraRigRotation()
    {
        float lookY = inputController.LookY * (playerController.CameraSensitivity.y / 2) * Time.deltaTime;

        cameraRigXRotation += lookY;
        cameraRigXRotation = Mathf.Clamp(cameraRigXRotation, playerController.CameraRotationClamp.x, playerController.CameraRotationClamp.y);

        cameraRig.localRotation = Quaternion.Euler(-cameraRigXRotation, 0f, 0f);
    }
    private void LateUpdateCameraRigPosition()
    {
        cameraRig.position = playerController.MasterIK.position;
    }
}
