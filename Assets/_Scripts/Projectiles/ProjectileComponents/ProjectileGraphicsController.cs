using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGraphicsController : ProjectileComponent<ProjectileGraphicsControllerData>
{
    private SpriteRenderer sr;

    public override void SetReferences()
    {
        base.SetReferences();

        sr = transform.Find("Graphics").GetComponent<SpriteRenderer>();
        Data = Projectile.Data.GetComponentData<ProjectileGraphicsControllerData>();
        sr.sprite = Data.projectileSprite;
    }
}

public class ProjectileGraphicsControllerData : ProjectileComponentData
{
    public ProjectileGraphicsControllerData()
    {
        ComponentDependencies.Add(typeof(ProjectileGraphicsController));
    }

    public Sprite projectileSprite;
}
