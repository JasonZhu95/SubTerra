using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;

    private void AnimationFinishedTrigger() => OnFinish?.Invoke();
}
