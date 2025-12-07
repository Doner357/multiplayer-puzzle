using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool shouldFaceMoveDirection = false;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float moveInAirAttenuation = 0.2f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float groundedThreshold = 1.05f;

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
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * playerInput.z + right * playerInput.x;

        if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10.0f * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        bool grounded =IsGrounded();
        velocity = moveDirection;
        velocity.Normalize();
        velocity = moveSpeed * velocity;
        if (!grounded)
        {
            velocity *= moveInAirAttenuation;
        }
        velocity.y = Convert.ToSingle(grounded) * jumpHeight * playerInput.y;
        rigid.AddForce(velocity, ForceMode.Impulse);
        Debug.Log($"Velocity: {velocity}");
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundedThreshold);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        playerInput.x = moveInput.x;
        playerInput.z = moveInput.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jumpping {context.performed}");
        playerInput.y = Convert.ToSingle(context.performed);
    }
}