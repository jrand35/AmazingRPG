using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour{
    public StatusEffect StatusEffect { get; private set; }

    public Status()
    {
        StatusEffect = StatusEffect.None;
    }
    public bool SetStatus(StatusEffect newStatus)
    {
        if (StatusEffect == StatusEffect.Defeated)
        {
            return false;
        }
        switch (newStatus)
        {
            case StatusEffect.None:
                StatusEffect = newStatus;
                return true;

            case StatusEffect.Poison:
                StatusEffect = newStatus;
                return true;

            case StatusEffect.Burn:
                StatusEffect = newStatus;
                return true;

            case StatusEffect.Confusion:
                StatusEffect = newStatus;
                return true;

            case StatusEffect.Defeated:
                StatusEffect = newStatus;
                return true;
            default:
                return true;
        }
    }
}
