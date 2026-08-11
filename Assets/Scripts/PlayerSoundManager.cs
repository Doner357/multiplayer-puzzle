using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSoundManager : NetworkBehaviour
{
    [Header("Audio Clips")]
    public AudioClip jumpClip;
    public float jumpVolume = 1f;
    public AudioClip walkClip;
    public float walkVolume = 1f;

    public void OnJump()
    {
        if (IsOwner)
        {
            AudioManager.Instance.Play3DAt(jumpClip, transform.position, jumpVolume);
        }
        PlayJumpSoundServerRpc();
    }

    [ServerRpc]
    private void PlayJumpSoundServerRpc()
    {
        List<ulong> targetIds = new List<ulong>();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != OwnerClientId)
            {
                targetIds.Add(clientId);
            }
        }

        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = targetIds.ToArray()
            }
        };

        PlayJumpSoundClientRpc(rpcParams);
    }

    [ClientRpc]
    private void PlayJumpSoundClientRpc(ClientRpcParams clientRpcParams = default)
    {
        AudioManager.Instance.Play3DAt(jumpClip, transform.position, jumpVolume);
    }

    public void OnWalk()
    {
        if (IsOwner)
        {
            AudioManager.Instance.Play3DAt(walkClip, transform.position, walkVolume);
            Debug.Log("Walk sound triggered.");
        }
        PlayWalkSoundServerRpc();
    }

    [ServerRpc]
    private void PlayWalkSoundServerRpc()
    {
        List<ulong> targetIds = new List<ulong>();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId != OwnerClientId)
            {
                targetIds.Add(clientId);
            }
        }

        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = targetIds.ToArray()
            }
        };

        PlayWalkSoundClientRpc(rpcParams);
    }

    [ClientRpc]
    private void PlayWalkSoundClientRpc(ClientRpcParams clientRpcParams = default)
    {
        AudioManager.Instance.Play3DAt(walkClip, transform.position, walkVolume);
    }
}
