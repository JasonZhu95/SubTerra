using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class where each player state will inherit from
public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;         // Checks Unity Animation completion

    protected float startTime;                  // Keeps track of time as we enter a state

    private string animBoolName;                // String that feeds state into the animator

    // Public constructor 
    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    // FUNCTION: occurs when a state is entered
    public virtual void Enter()
    {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationFinished = false;
    }

    // Function that occurs when a state is exited
    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
    }

    // FUNCTION: Mirrors Unity Update()
    public virtual void LogicUpdate()
    {

    }

    // FUNCTION: Mirrors Unity FixedUpdate()
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    // FUNCTION: Continually looks for ground or enemies etc
    public virtual void DoChecks()
    {

    }

    // FUNCTION:
    public virtual void AnimationTrigger()
    {

    }

    // FUNCTION: Changes the animation completion bool is accessed through a unity Animation event
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
