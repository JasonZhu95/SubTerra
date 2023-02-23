using System;
using UnityEngine;
public class TimerNotifier
{
    public event Action OnTimerDone;

    private float startTime;
    private float targetTime;

    private bool canTriggerEvent;

    public TimerNotifier(float duration, bool repeat)
    {
        SetTimes(duration);

        canTriggerEvent = true;

        if (!repeat)
        {
        OnTimerDone += () => canTriggerEvent = false;
        }
        else
        {
        OnTimerDone += () => SetTimes(duration);
        }
    }

    private void SetTimes(float duration)
    {
        startTime = Time.time;
        targetTime = startTime + duration;
    }

    public void Tick()
    {
        if (canTriggerEvent && Time.time >= targetTime)
        {
        OnTimerDone?.Invoke();
        }
    }

    public void Stop()
    {
        canTriggerEvent = false;
    }
}