using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class NetworkPlayerSettings : NetworkBehaviour
{
    [Header("Binding")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionSystem interactionSystem;

    [Header("CinemaMachineCamera")]
    [SerializeField] private Transform cameraRoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        playerInput.enabled = false;
        playerController.enabled = true;
        interactionSystem.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            playerInput.enabled = true;
            SetupCamera();
        }

        if (IsServer)
        {
            playerController.enabled = true;
            interactionSystem.enabled = true;
        }
    }

    private void SetupCamera()
    {
        if (IsOwner)
        {
            CinemachineCamera cam = FindAnyObjectByType<CinemachineCamera>();

            if (cam != null)
            {
                cam.Follow = this.transform;
                cam.LookAt = this.transform;

                PlayerController controller = GetComponent<PlayerController>();
                controller.cameraTransform = cam.transform;
            }
            else
            {
                Debug.LogWarning("can't find Cinemachine Virtual Camera¡I");
            }
        }
    }
}
