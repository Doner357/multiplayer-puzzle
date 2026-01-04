using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] public Transform cameraTransform;
    [SerializeField] public Animator animator;
    [SerializeField] private bool shouldFaceMoveDirection = true;
    [SerializeField] private float moveSpeed = 0.25f;
    [SerializeField] private float maxSpeed = 5f; // New: Maximum horizontal speed
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundedThreshold = 1.15f;

    [Header("Air Control")]
    [UnityEngine.Range(0f, 1f)]
    [SerializeField] private float airControlFactor = 0.5f; // New: Multiplier for movement when in air

    [Header("Jump Settings")]
    [SerializeField] private float jumpCooldown = 0.2f;

    [Header("Network Settings")]
    [Tooltip("How many times per second to send data to server.")]
    [SerializeField] private float networkSendRate = 30f;

    [Header("Audio Settings")]
    [Tooltip("How many times per second to trigger the walk event.")]
    [SerializeField] private float footstepRate = 2f;

    [Header("Events Trigger")]
    public UnityEvent onJump;
    public UnityEvent onWalk;

    // Input States
    private Vector2 moveInput;
    private bool jumpInput;

    // Physics States
    public bool isGrabbed { get; set; }
    private Rigidbody rigid;
    private Vector3 serverMoveDirection;
    private bool serverJumpInput;
    private float lastJumpTime;
    private Quaternion serverFaceRotation;

    // Timers
    private float networkTimer;
    private float walkTimer;

    // Animation
    private string isRunningStateName = "isRunning";

    // Components
    public override void OnNetworkSpawn()
    {
        rigid = GetComponent<Rigidbody>();
        isGrabbed = false;
        networkTimer = 0f;
        walkTimer = 0f;
    }

    void Update()
    {
        if (!IsOwner) return;

        // 1. Calculate Rotation Locally
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
            transform.rotation = serverFaceRotation;
        }

        // 2. Network Frequency Trigger (High Frequency)
        networkTimer += Time.deltaTime;
        if (networkTimer >= 1f / networkSendRate)
        {
            networkTimer = 0f;
            SubmitMovementServerRpc(targetDirection.normalized, serverFaceRotation, jumpInput);
        }

        // 3. Footstep Frequency Trigger (Low Frequency)
        if (moveInput.sqrMagnitude > 0.01f && IsGrounded())
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= 1f / footstepRate)
            {
                walkTimer = 0f;
                onWalk?.Invoke();
            }
        }
        else
        {
            walkTimer = 1f / footstepRate; 
        }
    }

    void FixedUpdate()
    {
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
        bool isGrounded = IsGrounded();

        // 1. Calculate Force with Air Control
        float currentForceMultiplier = isGrounded ? 1.0f : airControlFactor;
        Vector3 velocityForce = serverMoveDirection * moveSpeed * currentForceMultiplier;

        // 2. Apply Impulse Force
        rigid.AddForce(velocityForce, ForceMode.Impulse);

        // 3. Clamp Horizontal Speed (Max Speed Limit)
        Vector3 horizontalVelocity = new Vector3(rigid.linearVelocity.x, 0, rigid.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 clampedVelocity = horizontalVelocity.normalized * maxSpeed;
            rigid.linearVelocity = new Vector3(clampedVelocity.x, rigid.linearVelocity.y, clampedVelocity.z);
        }

        // 4. Animation Logic Update
        // Condition A: Has Input (serverMoveDirection is derived from input)
        bool hasInput = serverMoveDirection.sqrMagnitude > 0.01f;

        // Condition B: Actually Moving AND on Ground
        // We re-calculate horizontal magnitude squared to avoid sqrt operation cost
        bool isMovingPhysically = horizontalVelocity.sqrMagnitude > 0.1f;
        
        bool shouldAnimate = hasInput || (isMovingPhysically && isGrounded);

        animator.SetBool(isRunningStateName, shouldAnimate);
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

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        jumpInput = context.performed;

        if (jumpInput && IsGrounded())
        {
            onJump?.Invoke();
        }
    }
}