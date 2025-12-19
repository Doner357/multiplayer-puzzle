using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{

    [Header("Settings")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] private bool shouldFaceMoveDirection = false;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float moveInAirAttenuation = 0.2f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float groundedThreshold = 1.05f;

    public Vector2 move = Vector2.zero;
    public bool jump = false;

    private Rigidbody rigid;
    private Vector3 playerInput;
    private Vector3 moveDirection;
    private Vector3 velocity;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsOwner)
        {
            sendTransformRpc(playerInput, cameraTransform.forward, cameraTransform.right);
        }
    }

    [Rpc(SendTo.Server)]
    private void sendTransformRpc(Vector3 inputDir, Vector3 camForward, Vector3 camRight)
    {
        Vector3 forward = camForward;
        Vector3 right = camRight;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * inputDir.z + right * inputDir.x;

        if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10.0f * Time.deltaTime);
        }

        moveDirection.y = inputDir.y;
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            bool grounded =IsGrounded();
            velocity = moveDirection;
            velocity.Normalize();
            velocity = moveSpeed * velocity;
            if (!grounded)
            {
                velocity *= moveInAirAttenuation;
            }
            velocity.y = Convert.ToSingle(grounded) * jumpHeight * moveDirection.y;
            rigid.AddForce(velocity, ForceMode.Impulse);
            //Debug.Log($"Velocity: {velocity}");
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundedThreshold);
    }

    // Move Logics
    // [Rpc(SendTo.Server)]
    public void MoveRpc(Vector2 moveInput)
    {
        playerInput.x = moveInput.x;
        playerInput.z = moveInput.y;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        move = context.ReadValue<Vector2>();
        MoveRpc(move);
    }

    // Jump Logics
    // [Rpc(SendTo.Server)]
    public void JumpRpc(bool jumpInput)
    {
        // Debug.Log($"Jumpping {jumpInput}");
        playerInput.y = Convert.ToSingle(jumpInput);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        jump = context.performed;
        JumpRpc(jump);
    }
}