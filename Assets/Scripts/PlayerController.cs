using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public Animator animator;
    [SerializeField] private bool shouldFaceMoveDirection = true;
    [SerializeField] private float moveSpeed = 0.25f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundedThreshold = 1.15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpCooldown = 0.2f;

    // Input States
    private Vector2 moveInput;
    private bool jumpInput;

    // Physics States
    public bool isGrabbed { get; set;}
    private Rigidbody rigid;
    private Vector3 serverMoveDirection;
    private bool serverJumpInput;
    private float lastJumpTime;
    private Quaternion serverFaceRotation;

    // Animation
    private string isRunningStateName = "isRunning";
    
    // Components
    public override void OnNetworkSpawn()
    {
        rigid = GetComponent<Rigidbody>();
        isGrabbed = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 targetDirection = forward * moveInput.y + right * moveInput.x;

        if (shouldFaceMoveDirection && targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            serverFaceRotation = Quaternion.Slerp(serverFaceRotation, toRotation, 10.0f * Time.deltaTime);
        }

        serverMoveDirection = targetDirection;
        serverMoveDirection.Normalize();
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            SubmitMovementServerRpc(serverMoveDirection, serverFaceRotation, jumpInput);
        }

        if (IsServer)
        {
            ApplyMovement();
            ApplyJump();
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmitMovementServerRpc(Vector3 moveDir, Quaternion rotation, bool isJumping)
    {
        if (isGrabbed)
            return;
        serverMoveDirection = moveDir;
        transform.rotation = rotation;
        serverJumpInput = isJumping;
    }

    private void ApplyMovement()
    {
        Vector3 velocity = serverMoveDirection * moveSpeed;
        rigid.AddForce(velocity, ForceMode.Impulse);
        if (rigid.linearVelocity.sqrMagnitude > 0.25f)
        {
            animator.SetBool(isRunningStateName, true);
        }
        else
        {
            animator.SetBool(isRunningStateName, false);
        }
    }

    private void ApplyJump()
    {
        if (serverJumpInput && IsGrounded() && Time.time >= lastJumpTime + jumpCooldown)
        {
            Vector3 currentVel = rigid.linearVelocity;
            currentVel.y = 0; 
            rigid.linearVelocity = currentVel;
            
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;
            Debug.Log("Jump executed on server.");
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundedThreshold);
    }

    // Input System Callbacks
    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move input received: {moveInput}");
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        jumpInput = context.performed;
        Debug.Log($"Jump input received: {jumpInput}");
    }
}