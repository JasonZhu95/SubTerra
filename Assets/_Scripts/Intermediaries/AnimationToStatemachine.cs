using UnityEngine;
using System;

public class AnimationToStatemachine : MonoBehaviour
{
    public AttackState attackState;

    private void TriggerAttack()
    {
        attackState.TriggerAttack();
    }

    private void SetParryWindows(TrueFalse value)
    {
        attackState.SetParryWindowActive(Convert.ToBoolean(value));
    }

    private void FinishAttack()
    {
        attackState.FinishAttack();
    }
}

public enum TrueFalse
{
    False,
    True,
}
