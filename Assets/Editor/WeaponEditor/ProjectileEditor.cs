using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Project.Projectiles;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    private Projectile projectile;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        projectile = target as Projectile;

        if (projectile == null || projectile.Data == null) return;

        if (GUILayout.Button("Set Up Weapon"))
        {
            projectile.CreateProjectile(projectile.Data);
        }

        if (GUILayout.Button("Clear All Components"))
        {
            foreach (ProjectileComponent component in projectile.gameObject.GetComponents<ProjectileComponent>())
            {
                DestroyImmediate(component);
            }
        }
    }
}