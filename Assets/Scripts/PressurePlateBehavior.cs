using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PressurePlateMovement : MonoBehaviour
{
    [Header("Settings")]
    public Transform movingPart;
    public float downDistance = 0.2f;
    public float speed = 2.0f;
    public string targetTag = "Player";

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private Vector3 bottomPos;
    private bool isAtBottom = false;
    private int objectsOnPlate = 0;

    void Start()
    {
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
            onPressed?.Invoke();
        }
        else
        {
            onReleased?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TagController tagController = other.GetComponent<TagController>();
        if (tagController != null)
        {
            if (tagController.HasTag(GameTags.Heavy))
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
        TagController tagController = other.GetComponent<TagController>();
        if (tagController != null)
        {
            if (tagController.HasTag(GameTags.Heavy))
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
