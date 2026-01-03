using Unity.Netcode;
using UnityEngine;

public class Respawnable : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Quaternion initialRotation;
    [SerializeField] private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            rb = GetComponent<Rigidbody>();
        }
    }

    public void Respawn()
    {
        if (!IsServer) return;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        Debug.Log($"{gameObject.name} has respawned!");
    }

    public void SetNewSpawnPoint(Vector3 pos, Quaternion rot)
    {
        if (IsServer)
        {
            initialPosition = pos;
            initialRotation = rot;
        }
    }
}