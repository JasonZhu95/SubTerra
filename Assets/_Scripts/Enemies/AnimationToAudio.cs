using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToAudio : MonoBehaviour
{
    [SerializeField]
    private EnemyFootsteps enemyFootsteps;
    public void AnimationEnemyWalkSFX()
    {
        enemyFootsteps.AnimationEnemyWalkSFX();
    }
}
