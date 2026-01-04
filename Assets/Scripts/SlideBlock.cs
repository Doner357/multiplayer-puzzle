using Unity.Netcode;
using UnityEngine;

public class SlideBlock : NetworkBehaviour 
{
    [Header("Block Settings")]
    public float slideDistance = 3.0f;
    public float slideSpeed = 2.0f;
    public Vector3 moveDirection = Vector3.up;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource movingAudioSource;
    [SerializeField] private AudioSource impactAudioSource;
    [SerializeField] private AudioClip stopClip;

    private NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false);

    private Vector3 openPosition;
    private Vector3 closedPosition;
    private Vector3 targetPosition;

    public override void OnNetworkSpawn()
    {
        openPosition = transform.position + moveDirection * slideDistance; 
        closedPosition = transform.position;
        targetPosition = closedPosition;

        isMoving.OnValueChanged += HandleMovementStateChange;
    }

    public override void OnNetworkDespawn()
    {
        isMoving.OnValueChanged -= HandleMovementStateChange;
    }

    void Update()
    {
        if (!IsServer) return; 

        float distance = Vector3.Distance(transform.position, targetPosition);
        bool currentlyMoving = distance > 0.01f;

        if (currentlyMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
        }

        if (isMoving.Value != currentlyMoving)
        {
            isMoving.Value = currentlyMoving;
        }
    }

    private void HandleMovementStateChange(bool previousState, bool isNowMoving)
    {
        if (isNowMoving)
        {
            if (movingAudioSource != null && !movingAudioSource.isPlaying)
            {
                movingAudioSource.Play();
            }
        }
        else
        {
            if (movingAudioSource != null)
            {
                movingAudioSource.Stop();
            }

            if (impactAudioSource != null && stopClip != null && previousState == true)
            {
                impactAudioSource.PlayOneShot(stopClip);
            }
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
    
    private void OnDrawGizmosSelected()
    {
        if (moveDirection == Vector3.zero) return;
        Gizmos.color = Color.cyan;
        Vector3 startPos = transform.position;
        Vector3 direction = moveDirection.normalized; 
        Vector3 endPos = startPos + direction * slideDistance;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireCube(endPos, transform.localScale);
        Gizmos.DrawSphere(endPos, 0.1f);
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawLine(startPos, endPos);
    }
}
