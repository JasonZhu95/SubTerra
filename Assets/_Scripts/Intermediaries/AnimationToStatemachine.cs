using UnityEngine;
using System;

public class AnimationToStatemachine : MonoBehaviour
{
    public AttackState attackState;
    public DeadState deadState;

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

    private void TriggerDeathParticles()
    {
        deadState.TriggerDeathParticles();
    }

    private void DieAnimationFinished()
    {
        deadState.DieAnimationFinished();
    }
}

public enum TrueFalse
{
    False,
    True,
}
