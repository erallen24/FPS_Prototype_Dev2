using UnityEngine;
using Player.Utilities;

public class PlayerMovementController
{
    private readonly AdvancedPlayerController playerController;
    private readonly PlayerInputController inputController;
    private readonly CharacterController characterController;
    private Vector3 playerVelocity;
    private int jumpCount;
    private float movementSpeed;

    public PlayerMovementController(AdvancedPlayerController player, PlayerMovementControllerSettings settings)
    {
        playerController = player;
        inputController = player.InputController;
        characterController = settings.controller;
    }

    public void Update()
    {
        UpdatePlayerGroundedState();
        UpdatePlayerLocomotionState();

        UpdatePlayerMovement();
        UpdatePlayerRotation();
        UpdatePlayerJump();
        UpdatePlayerGravity();
    }
    public void DrawGizmos()
    {
        DrawDebugGizmos();
    }

    private void UpdatePlayerGroundedState()
    {
        if (Physics.CheckSphere(playerController.transform.position + playerController.GroundCheckRadiusOffset, playerController.GroundCheckRadius, playerController.GroundLayer))
        {
            playerController.GroundedState = PlayerGroundedState.Grounded;
        }
        else
        {
            playerController.GroundedState = PlayerGroundedState.Airborne;
        }
    }
    private void UpdatePlayerLocomotionState()
    {
        if (inputController.SprintHeld && playerController.GroundedState == PlayerGroundedState.Grounded)
        {
            playerController.LocomotionState = PlayerLocomotionState.Sprinting;
        }
        else
        {
            playerController.LocomotionState = PlayerLocomotionState.Default;
        }
    }

    private void UpdatePlayerMovement()
    {
        float targetMovementSpeed = playerController.LocomotionState == PlayerLocomotionState.Sprinting ? playerController.SprintMovementSpeed : playerController.DefaultMovementSpeed;
        movementSpeed = Mathf.Lerp(movementSpeed, targetMovementSpeed, Time.deltaTime * 5f);

        Vector3 movementVector = inputController.MovementDirection * movementSpeed;

        characterController.Move(movementVector * Time.deltaTime);
    }
    private void UpdatePlayerRotation()
    {
        float rotation = inputController.LookX * (playerController.CameraSensitivity.x * 0.25f) * Time.deltaTime;

        playerController.transform.Rotate(playerController.transform.up * rotation);
    }
    private void UpdatePlayerJump()
    {
        if (playerController.GroundedState == PlayerGroundedState.Grounded)
        {
            jumpCount = 0;
        }

        if (inputController.JumpPressed && jumpCount < playerController.MaxJumpCount)
        {
            playerVelocity.y = playerController.JumpForce;
            jumpCount++;
        }
    }
    private void UpdatePlayerGravity()
    {
        if (playerController.GroundedState == PlayerGroundedState.Grounded && !inputController.JumpPressed)
        {
            playerVelocity.y = 0;
        }
        else
        {
            playerVelocity.y -= playerController.GravityForce * Time.deltaTime;

            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void DrawDebugGizmos()
    {
        Gizmos.color = playerController.GroundedState == PlayerGroundedState.Grounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(playerController.transform.position + playerController.GroundCheckRadiusOffset, playerController.GroundCheckRadius);
    }
}
