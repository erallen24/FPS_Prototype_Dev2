using Player.Utilities;
using UnityEngine;

public class PlayerAnimationController 
{
    private readonly AdvancedPlayerController playerController;
    private readonly PlayerInputController inputController;
    private readonly Animator animator;
    private readonly Transform rightHandIK;
    private readonly Transform leftHandIK;
    private readonly Transform leftHandIKTarget;
    private readonly Transform animatorLookAt;
    private float animatorLookXAngle;

    public PlayerAnimationController(AdvancedPlayerController player, PlayerAnimationControllerSettings settings)
    {
        playerController = player;
        inputController = player.InputController;
        animator = settings.animator;
        rightHandIK = settings.rightHandIKTransform;
        leftHandIK = settings.leftHandIKTransform;
        leftHandIKTarget = settings.leftHandIKTargetTransform;
        animatorLookAt = settings.animatorLookAtTransform;
    }

    public void Update()
    {
        UpdateAnimator();
    }
    public void LateUpdate()
    {
        LateUpdateHandIK();
    }
    public void OnAnimatorIK()
    {
        UpdateAnimatorLookIK();
        UpdateAnimatorHandIK();
    }


    private void UpdateAnimator()
    {
        animator.SetBool("Grounded", playerController.GroundedState == PlayerGroundedState.Grounded);

        animator.SetFloat("Movement X", inputController.MoveX, 0.05f, Time.deltaTime);
        animator.SetFloat("Movement Y", inputController.MoveY, 0.05f, Time.deltaTime);

        float lookX = inputController.LookX;
        float lookY = inputController.LookY * (playerController.CameraSensitivity.y / 2) * Time.deltaTime;

        animatorLookXAngle += lookY;
        animatorLookXAngle = Mathf.Clamp(animatorLookXAngle, playerController.CameraRotationClamp.x, playerController.CameraRotationClamp.y);

        animator.SetFloat("Look X Angle", animatorLookXAngle, 0.1f, Time.deltaTime);
        animator.SetFloat("Look Y Angle", lookX, 0.1f, Time.deltaTime);

        float sprintingWeight = inputController.SprintHeld ? 1f : 0f;
        animator.SetFloat("Sprinting Weight", sprintingWeight, 0.1f, Time.deltaTime);
    }

    private void LateUpdateHandIK()
    {
        leftHandIK.SetPositionAndRotation(leftHandIKTarget.position, leftHandIKTarget.rotation);
    }


    private void UpdateAnimatorLookIK()
    {
        animator.SetLookAtWeight(1f, 0.1f, 1f);
        animator.SetLookAtPosition(animatorLookAt.position);
    }
    private void UpdateAnimatorHandIK()
    {
        float rightWeight = animator.GetFloat("Right Hand IK Weight");
        float leftWeight = animator.GetFloat("Left Hand IK Weight");

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
    }
}
