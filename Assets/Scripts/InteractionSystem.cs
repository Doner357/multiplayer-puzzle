using System;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : NetworkBehaviour
{
    [Header("Settings")]
    public float grabRange = 1.5f;
    public Transform holdPoint;
    public float throwForce = 2.5f;
    public Animator animator;

    [Header("Debug")]
    public Rigidbody grabbedObject;
    private FixedJoint joint;

    public bool grab = false;
    public bool throwObj = false;

    private PlayerController playerController;

    private bool isGrabbing = false;
    private bool grabbindPlayer = false;
    private RigidbodyConstraints originalConstraints;

    // Animation
    private string isHoldingStateName = "isHolding";

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerController = GetComponent<PlayerController>();
    }

    void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            Rigidbody targetRb = hit.collider.attachedRigidbody;

            if (targetRb != null && !targetRb.isKinematic)
            {
                Transform targetTr = targetRb.GetComponent<Transform>();
                TagController tagController = targetRb.GetComponent<TagController>();
                if (tagController != null && tagController.HasTag(GameTags.Player))
                {
                    if (playerController.isGrabbed)
                        return;
                    PlayerController pc = targetRb.GetComponent<PlayerController>();
                    pc.isGrabbed = true;
                    grabbindPlayer= true;   
                }
                Vector3 targetPos = targetTr.position;
                targetPos.y += 0.1f;
                targetTr.position = targetPos;
                grabbedObject = targetRb;
                CreateJoint(targetRb);
                animator.SetBool(isHoldingStateName, true);
            }
        }
    }

    void CreateJoint(Rigidbody targetRb)
    {
        originalConstraints = targetRb.constraints;

        targetRb.constraints = RigidbodyConstraints.None;

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = targetRb;
        joint.breakForce = Mathf.Infinity;

        if (targetRb.TryGetComponent<Collider>(out Collider targetCol))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), targetCol, true);
        }
    }

    void Release(float impulseForce = 0.0f)
    {
        if (joint != null)
        {
            if (grabbindPlayer)
            {
                PlayerController pc = grabbedObject.GetComponent<PlayerController>();
                pc.isGrabbed = false;
                grabbindPlayer = false;
            }

            joint.connectedBody = null;
            Destroy(joint);
            joint = null;

            grabbedObject.constraints = originalConstraints;

            grabbedObject.isKinematic = false;

            if (grabbedObject.TryGetComponent<Collider>(out Collider targetCol))
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), targetCol, false);
            }

            Vector3 v = transform.forward;
            v.y += 1.5f;
            v *= impulseForce;
            grabbedObject.AddForce(v, ForceMode.VelocityChange);
            grabbedObject = null;
            animator.SetBool(isHoldingStateName, false);
        }
    }


    // Grab Logics

    [Rpc(SendTo.Server)]
    public void GrabRpc(bool grabInput)
    {
        // is clicked left botton
        if (!grabInput)
            return;

        if (!isGrabbing)
        {
            isGrabbing = true;
            TryGrab();
        }
        else
        {
            isGrabbing = false;
            Release();
        }
    }
    public void Grab(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        grab = context.performed;
        Debug.Log($"Grabbing {grab}");
        GrabRpc(grab);
    }

    // Throw Logics
    [Rpc(SendTo.Server)]
    public void ThrowRpc(bool throwInput)
    {
        if (isGrabbing && !throwInput)
        {
            isGrabbing = false;
            Release(throwForce);
        }
    }
    public void Throw(InputAction.CallbackContext context)
    {
        if (!IsOwner)
            return;
        throwObj = context.performed;
        Debug.Log($"Throwing {throwObj}");
        ThrowRpc(throwObj);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * grabRange);
    }
}