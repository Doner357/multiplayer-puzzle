using Unity.Netcode;
using UnityEngine;

public class PlatformStickiness : NetworkBehaviour
{
    private Rigidbody rb;
    private Transform currentPlatform;
    private Vector3 lastPlatformPos;
    private Quaternion lastPlatformRot;

    private bool CanControl => IsServer; 

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!CanControl || currentPlatform == null) return;

        Vector3 posDelta = currentPlatform.position - lastPlatformPos;
        
        if (posDelta.sqrMagnitude > 0.00001f)
        {
            Vector3 targetPos = rb.position + posDelta;
            rb.MovePosition(targetPos);
        }

        lastPlatformPos = currentPlatform.position;
        lastPlatformRot = currentPlatform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!CanControl) return;

        Rigidbody platformRb = collision.collider.attachedRigidbody;

        if (platformRb != null)
        {
            TagsController tags = platformRb.GetComponent<TagsController>();

            if (tags != null && tags.HasTag(GameTag.MovingPlatform)) 
            {
                currentPlatform = platformRb.transform;
                lastPlatformPos = currentPlatform.position;
                lastPlatformRot = currentPlatform.rotation;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!CanControl) return;

        Rigidbody platformRb = collision.collider.attachedRigidbody;
        
        if (platformRb != null && platformRb.transform == currentPlatform)
        {
            currentPlatform = null;
        }
    }
}