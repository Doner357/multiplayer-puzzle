using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PressurePlateBehaviour : NetworkBehaviour
{
    [Header("Settings")]
    public Transform movingPart;
    public float downDistance = 1.0f;
    public float speed = 4.0f;
    public GameTags targetTag = GameTags.Heavy;
    public NetworkSignal linkedSignal;

    // Position Relating
    private Vector3 initialPos;
    private Vector3 targetPos;
    private Vector3 bottomPos;

    // State Relating
    private int objectsOnPlate = 0;

    public override void OnNetworkSpawn()
    {
        // 自動查找 NetworkSignal（在父物件 PressurePlate 上）
        if (linkedSignal == null)
        {
            linkedSignal = GetComponentInParent<NetworkSignal>();
        }

        // 自動查找 movingPart（就是父物件 PlateBase）
        if (movingPart == null)
        {
            movingPart = transform.parent;
        }

        if (movingPart != null)
        {
            initialPos = movingPart.localPosition;
            targetPos = initialPos;
            bottomPos = initialPos - new Vector3(0, downDistance, 0);
        }
        else
        {
            Debug.LogError("Please specify the moving part for pressure plate!");
        }
    }

    void Update()
    {
        if (!IsServer)
        {
            Debug.Log("Not the server, exiting Update.");
            return;
        }

        if (movingPart != null)
        {
            movingPart.localPosition = Vector3.MoveTowards(
                movingPart.localPosition, 
                targetPos, 
                speed * Time.deltaTime
            );
            UpdatePressurePlateStatusRpc(movingPart.localPosition == bottomPos);
        }
    }

    private void UpdatePressurePlateStatusRpc(bool pressed)
    {
        if (pressed)
        {
            linkedSignal.SetState(true);
            Debug.Log("Pressure Plate Pressed");
        }
        else
        {
            linkedSignal.SetState(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            Debug.Log("Not the server, exiting OnTriggerEnter.");
            return;
        }
        TagController tagController = other.attachedRigidbody.GetComponent<TagController>();
        if (tagController != null)
        {
            if (tagController.HasTag(targetTag))
            {
                objectsOnPlate++;
                if (objectsOnPlate >= 1)
                {
                    targetPos = initialPos - new Vector3(0, downDistance, 0);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer)
        {
            Debug.Log("Not the server, exiting OnTriggerExit.");
            return;
        }
        TagController tagController = other.attachedRigidbody.GetComponent<TagController>();
        if (tagController != null)
        {
            if (tagController.HasTag(targetTag))
            {
                objectsOnPlate--;
                if (objectsOnPlate <= 0)
                {
                    objectsOnPlate = 0;
                    targetPos = initialPos;
                }
            }
        }
    }
}
