using Unity.Netcode;
using UnityEngine;

public class SlideBlock : NetworkBehaviour 
{
    [Header("Block Settings")]
    public float slideDistance = 3.0f;
    public float slideSpeed = 2.0f;
    public Vector3 moveDirection = Vector3.up;

    private Vector3 openPosition;
    private Vector3 closedPosition;
    private Vector3 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        openPosition = transform.position + moveDirection * slideDistance; 
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return; 
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
        }
    }

    public void Activate()
    {
        if (!IsServer) return; 
        targetPosition = openPosition;
    }

    public void Deactivate()
    {
        if (!IsServer) return; 
        targetPosition = closedPosition;
    }
}
