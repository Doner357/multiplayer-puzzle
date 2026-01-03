using Unity.Netcode;
using UnityEngine;

public class VoidZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var targetRb = other.attachedRigidbody;
        
        if (targetRb != null)
        {
            var respawnable = targetRb.GetComponent<Respawnable>();
            if (respawnable != null)
            {
                respawnable.Respawn();
            }
            else
            {
                targetRb.GetComponent<NetworkObject>()?.Despawn();
            }
        }
    }
}