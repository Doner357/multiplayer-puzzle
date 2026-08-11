using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LogicGate : NetworkBehaviour
{
    [Header("Condition List")]
    [Tooltip("Drag all the condition need to check")]
    public List<Condition> requiredCondition;

    [Header("Result Events")]
    public UnityEvent OnAllConditionsMet;
    public UnityEvent OnConditionsNotMet;

    private bool wasOpen = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var condition in requiredCondition)
            {
                if (condition.signal != null)
                {
                    condition.signal.IsActive.OnValueChanged += (prev, curr) => CheckAllConditions();
                }
            }
        }
    }

    private void CheckAllConditions()
    {
        if (!IsServer) return;

        bool allMet = true;

        foreach (var condition in requiredCondition)
        {
            if (condition.signal != null && condition.signal.IsActive.Value != condition.expectedState)
            {
                allMet = false;
                break;
            }
        }

        if (allMet && !wasOpen)
        {
            wasOpen = true;
            TriggerOpenClientRpc();
        }
        else if (!allMet && wasOpen)
        {
            wasOpen = false;
            TriggerCloseClientRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOpenClientRpc()
    {
        OnAllConditionsMet?.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerCloseClientRpc()
    {
        OnConditionsNotMet?.Invoke();
    }
}
