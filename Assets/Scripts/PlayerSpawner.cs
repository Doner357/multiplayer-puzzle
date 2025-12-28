using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag your Player Prefab here (This must also be registered in the list of NetworkManager)")]
    public GameObject playerPrefab;

    [Tooltip("Drag the Transform")]
    public List<Transform> spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            if (IsHost)
            {
                OnClientConnected(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        int index = (int)(clientId % (ulong)spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        var netObj = playerInstance.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);
        
        Debug.Log($"Player {clientId} has been spawned at point {index}");
    }
}
