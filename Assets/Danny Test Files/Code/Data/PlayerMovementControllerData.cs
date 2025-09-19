using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName = "New Player Movement Controller Data", menuName = "Player/Data/Movement Controller Data")]
    public class PlayerMovementControllerData : ScriptableObject
    {
        [Header("Controller Properties")]
        [Space(10)]
        [Range(0f, 5f)] public float defaultMovementSpeed;
        [Space(5)]
        [Range(0, 10f)] public float sprintMovementSpeed;
        [Space(10)]
        [Range(0, 25f)] public float gravityForce;
        [Space(10)]
        [Range(1, 15)] public int jumpForce;
        [Range(1, 5)] public int maxJumpCount;
        [Space(10)]
        [Range(0.01f, 0.5f)] public float groundCheckRadius;
        [Space(5)]
        public Vector3 groundCheckRadiusOffset;
        [Space(5)]
        public LayerMask groundLayer;
    }
}