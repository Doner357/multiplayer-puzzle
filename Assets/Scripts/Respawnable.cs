using Unity.Netcode;
using UnityEngine;

public class Respawnable : NetworkBehaviour
{
    [Header("Settings")]
    public NetworkVariable<Vector3> initialPosition = new NetworkVariable<Vector3>(
            default, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<Quaternion> initialRotation = new NetworkVariable<Quaternion>(
        default,
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    [SerializeField] private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            initialPosition.Value = transform.position;
            initialRotation.Value = transform.rotation;
            rb = GetComponent<Rigidbody>();
        }
    }

    [Rpc(SendTo.Server)]
    public void RespawnRpc()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = initialPosition.Value;
        transform.rotation = initialRotation.Value;
        
        Debug.Log($"{gameObject.name} has respawned!");
    }

    public void Respawn()
    {
        RespawnRpc();
    }
    public void SetNewSpawnPoint(Vector3 pos, Quaternion rot)
    {
        if (IsServer)
        {
            initialPosition.Value = pos;
            initialRotation.Value = rot;
        }
    }
}