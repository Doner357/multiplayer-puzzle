using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public float grabRange = 1.5f;
    public Transform holdPoint;
    public float throwForce = 2.5f;

    [Header("Debug")]
    public Rigidbody grabbedObject;
    private FixedJoint joint;

    private bool isGrabbing = false;

    void Update()
    {

    }

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
        joint = gameObject.AddComponent<FixedJoint>();

        joint.connectedBody = targetRb;

        joint.breakForce = Mathf.Infinity;
    }

    void Release(float impulseForce = 0.0f)
    {
        if (joint != null)
        {
            // 銷毀關節組件，斷開連結
            Destroy(joint);
            joint = null;
            Vector3 v = transform.forward;
            v.y += 1.5f;
            v *= impulseForce;
            grabbedObject.AddForce(v, ForceMode.Impulse);
            grabbedObject = null;
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.performed)
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

    public void Throw(InputAction.CallbackContext context)
    {
        if (isGrabbing && !context.performed)
        {
            isGrabbing = false;
            Release(throwForce);
        }
    }

    // 在編輯器中畫出輔助線，方便調整距離
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * grabRange);
    }
}