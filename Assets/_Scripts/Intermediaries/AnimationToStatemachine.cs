using UnityEngine;
using System;

public class AnimationToStatemachine : MonoBehaviour
{
    public AttackState attackState;
    public DeadState deadState;
    public TransformToMummyState transformToMummyState;
    public TransformToHumanState transformToHumanState;
    public DashFlurryState dashFlurryState;
    public TransformToDemonKingState transformToDemonKingState;

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

    private void TransformationToMummyFinished()
    {
        transformToMummyState.TransformationToMummyFinished();
    }

    private void TransformationToHumanFinished()
    {
        transformToHumanState.TransformationToHumanFinished();
    }

    private void TransformationToDemonKingFinished()
    {
        transformToDemonKingState.TransformationToDemonKingFinished();
    }

    private void TeleportToPlayer()
    {
        dashFlurryState.TeleportToPlayer();
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
