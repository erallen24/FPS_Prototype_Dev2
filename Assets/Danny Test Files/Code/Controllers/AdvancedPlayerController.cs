using UnityEngine;
using Player.Utilities;

public class AdvancedPlayerController : MonoBehaviour
{
    #region PRIVATE PROPERTIES

    [SerializeField] private PlayerMovementControllerSettings movementControllerSettings;
    [Space(10)]
    [SerializeField] private PlayerCameraControllerSettings cameraControllerSettings;
    [Space(10)]
    [SerializeField] private PlayerAnimationControllerSettings animationControllerSettings;

    #endregion

    #region PROPERTY GETTERS

    public CharacterController CharacterController => movementControllerSettings.controller;
    public float DefaultMovementSpeed => movementControllerSettings.data.defaultMovementSpeed;
    public float SprintMovementSpeed => movementControllerSettings.data.sprintMovementSpeed;
    public float GravityForce => movementControllerSettings.data.gravityForce;
    public int JumpForce => movementControllerSettings.data.jumpForce;
    public int MaxJumpCount => movementControllerSettings.data.maxJumpCount;
    public float GroundCheckRadius => movementControllerSettings.data.groundCheckRadius;
    public Vector3 GroundCheckRadiusOffset => movementControllerSettings.data.groundCheckRadiusOffset;
    public LayerMask GroundLayer => movementControllerSettings.data.groundLayer;

    public PlayerGroundedState GroundedState { get { return movementControllerSettings.groundedState; } set { movementControllerSettings.groundedState = value; } }
    public PlayerLocomotionState LocomotionState { get { return movementControllerSettings.locomotionState; } set { movementControllerSettings.locomotionState = value; } }

    public Transform CameraRig => cameraControllerSettings.cameraRigTransform;
    public Vector2 CameraRotationClamp => cameraControllerSettings.data.cameraRotationClamp;
    public Vector2 CameraSensitivity => cameraControllerSettings.data.cameraSensitivity * 100;

    public Animator Animator => animationControllerSettings.animator;
    public Transform AnimatorLookAt => animationControllerSettings.animatorLookAtTransform;
    public Transform MasterIK => animationControllerSettings.masterIKTransform;
    public Transform RightHandIK => animationControllerSettings.rightHandIKTransform;
    public Transform LeftHandIK => animationControllerSettings.leftHandIKTransform;
    public Transform LeftHandIKTarget => animationControllerSettings.leftHandIKTargetTransform;

    #endregion

    #region CONTROLLERS

    public PlayerInputController InputController { get; private set; }
    public PlayerMovementController MovementController { get; private set; }
    public PlayerCameraController CameraController { get; private set; }
    public PlayerAnimationController AnimationController { get; private set; }

    #endregion

    #region METHODS
    private void InitializeControllers()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputController = new PlayerInputController(this);
        MovementController = new PlayerMovementController(this, movementControllerSettings);
        CameraController = new PlayerCameraController(this, cameraControllerSettings);
        AnimationController = new PlayerAnimationController(this, animationControllerSettings);
    }
    private void UpdateControllers()
    {
        InputController?.Update();
        MovementController?.Update();
        CameraController?.Update();
        AnimationController?.Update();
    }
    private void LateUpdateControllers()
    {
        CameraController?.LateUpdate();
        AnimationController?.LateUpdate();
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Start()
    {
        InitializeControllers();
    }

    private void Update()
    {
        UpdateControllers();
    }

    private void LateUpdate()
    {
        LateUpdateControllers();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        AnimationController?.OnAnimatorIK();
    }

    private void OnDrawGizmos()
    {
        MovementController?.DrawGizmos();
    }

    #endregion
}
