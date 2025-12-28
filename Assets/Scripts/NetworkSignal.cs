using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkSignal : NetworkBehaviour
{
    [HideInInspector]public NetworkVariable<bool> IsActive = new NetworkVariable<bool>(false);

    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    public override void OnNetworkSpawn()
    {
        IsActive.OnValueChanged += OnStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        IsActive.OnValueChanged -= OnStateChanged;
    }

    private void OnStateChanged(bool previous, bool current)
    {
        if (current) OnActivate?.Invoke();
        else OnDeactivate?.Invoke();
    }

    public void SetState(bool state)
    {
        if (IsServer)
        {
            IsActive.Value = state;
        }
    }
}
