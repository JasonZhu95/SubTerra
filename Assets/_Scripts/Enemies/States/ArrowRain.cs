using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Projectiles;
using System.Threading;

public class ArrowRain : AttackState
{
    protected D_ArrowRainState stateData;

    protected GameObject projectile;
    protected Projectile projectileScript;

    private int arrowsSpawned = 0;
    private float lastSpawnTime = 1000000000f;

    public ArrowRain(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_ArrowRainState stateData) : base(etity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if ((arrowsSpawned < stateData.number_of_arrows) && (Time.time - lastSpawnTime >= Random.Range(stateData.delayMin, stateData.delayMax)))
        {
            SpawnArrow();
            lastSpawnTime = Time.time;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        lastSpawnTime = Time.time;
    }

    public void SpawnArrow()
    {
        float randomX = Random.Range(attackPosition.position.x - stateData.xVariation, attackPosition.position.x + stateData.xVariation);
        projectile = GameObject.Instantiate(stateData.projectile, attackPosition.position + new Vector3(randomX, stateData.spawnHeightOffset, 0f), attackPosition.rotation);
        projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.CreateProjectile(stateData.projectileData);
        projectileScript.Init(entity.gameObject);
        
        arrowsSpawned++;
    }
}
