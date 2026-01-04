using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class PlayerSpawner : NetworkBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag your Player Prefab here")]
    public GameObject playerPrefab;

    [Tooltip("Drag the Transform")]
    public List<Transform> spawnPoints;

    private Dictionary<ulong, int> occupiedSpawns = new Dictionary<ulong, int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

            if (IsHost)
            {
                if (!occupiedSpawns.ContainsKey(NetworkManager.Singleton.LocalClientId))
                {
                    OnClientConnected(NetworkManager.Singleton.LocalClientId);
                }
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        int spawnIndex = GetAvailableSpawnIndex();

        if (spawnIndex == -1) 
        {
            Debug.LogWarning($"No enough space for player {clientId}! Set the repawn point to 0¡C");
            spawnIndex = 0; 
        }

        occupiedSpawns[clientId] = spawnIndex;

        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        var respawnable = playerInstance.GetComponent<Respawnable>();
        if (respawnable != null)
        {
            respawnable.SetNewSpawnPoint(spawnPoint.position, spawnPoint.rotation);
        }

        var netObj = playerInstance.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);
        
        Debug.Log($"Player {clientId} spawned at index {spawnIndex} (Randomly Selected)");
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (occupiedSpawns.ContainsKey(clientId))
        {
            int freedIndex = occupiedSpawns[clientId];
            occupiedSpawns.Remove(clientId);
            Debug.Log($"Player {clientId} disconnected. Spawn point {freedIndex} is now free.");
        }
    }

    private int GetAvailableSpawnIndex()
    {
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            availableIndices.Add(i);
        }

        foreach (int occupiedIndex in occupiedSpawns.Values)
        {
            if (availableIndices.Contains(occupiedIndex))
            {
                availableIndices.Remove(occupiedIndex);
            }
        }

        if (availableIndices.Count == 0)
        {
            return -1;
        }

        int randomIndex = Random.Range(0, availableIndices.Count);
        return availableIndices[randomIndex];
    }
}
