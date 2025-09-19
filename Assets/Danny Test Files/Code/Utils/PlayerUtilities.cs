using Player.Data;
using UnityEngine;

namespace Player.Utilities
{
    public enum PlayerGroundedState
    {
        Airborne,
        Grounded
    }
    public enum PlayerLocomotionState
    {
        Default = 1,
        Sprinting = 2,
    }
    public enum PlayerAimingState
    {
        Inactive,
        Active
    }


    [System.Serializable]
    public struct PlayerMovementControllerSettings
    {
        [Header("PROPERTIES")]
        [Space(10)]
        public PlayerMovementControllerData data;
        [Space(5)]
        public CharacterController controller;
        [Space(20)]

        [Header("STATES")]
        [Space(10)]
        public PlayerGroundedState groundedState;
        [Space(5)]
        public PlayerLocomotionState locomotionState;
    }

    [System.Serializable]
    public struct PlayerCameraControllerSettings
    {
        [Header("PROPERTIES")]
        [Space(10)]
        public PlayerCameraControllerData data;
        [Space(5)]
        public Camera camera;
        [Space(5)]
        public Transform cameraRigTransform;
    }

    [System.Serializable]
    public struct PlayerAnimationControllerSettings
    {
        [Header("PROPERTIES")]
        [Space(10)]
        public PlayerAnimationControllerData data;
        [Space(5)]
        public Animator animator;
        [Space(5)]
        public Transform animatorLookAtTransform;
        [Space(5)]
        public Transform masterIKTransform;
        [Space(5)]
        public Transform handIKTransform;
        public Transform rightHandIKTransform;
        public Transform leftHandIKTransform;
        [Space(5)]
        public Transform leftHandIKTargetTransform;
    }


    public static class PlayerUtilities { }
}
