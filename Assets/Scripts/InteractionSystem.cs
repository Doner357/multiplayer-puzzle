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

    [Header("Debug")]
    public Rigidbody grabbedObject;
    private FixedJoint joint;

    public bool grab = false;
    public bool throwObj = false;

    private bool isGrabbing = false;
    private RigidbodyConstraints originalConstraints;


    void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            Rigidbody targetRb = hit.collider.GetComponent<Rigidbody>();
            Transform targetTr = targetRb.GetComponent<Transform>();

            if (targetRb != null && !targetRb.isKinematic)
            {
                Vector3 targetPos = targetTr.position;
                targetPos.y += 0.1f;
                targetTr.position = targetPos;
                grabbedObject = targetRb;
                CreateJoint(targetRb);
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
            grabbedObject.AddForce(v, ForceMode.Impulse);
            grabbedObject = null;
        }
    }


    // Grab Logics

    [Rpc(SendTo.Server)]
    public void GrabRpc(bool grabInput)
    {
        if (grabInput)
        {
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

    // 在編輯器中畫出輔助線，方便調整距離
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * grabRange);
    }
}