using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Projectiles;
using Project.Interfaces;

public class Enemy4 : MonoBehaviour
{
    [SerializeField]
    private AIPath aiPath;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private D_RangedAttackState stateData;

    [SerializeField]
    private float maxHealth = 20f;

    private Animator animator;

    private float currentHealth;
    private float lastProjectileTime;
    private float distanceToPlayer;

    private bool isFlying = false;
    private bool isChasing = false;

    private GameObject projectile;
    private Projectile projectileScript;
    
    // public
    
    public float projectileCooldown = 5f;
    public float projectileRange = 7f;


    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        lastProjectileTime = Time.time;

        animator.SetBool("idle", true);
        animator.SetBool("isFlying", false);
        animator.SetBool("ceilingOut", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChasing)
        {
            isChasing = true;
            animator.SetBool("idle", false);
            animator.SetBool("ceilingOut", true);
        }
    }

    private void Update()
    {
        aiPath.canMove = isFlying;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }

        if (isFlying)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < projectileRange && Time.time > lastProjectileTime + projectileCooldown)
            {
                Vector3 direction = playerTransform.position - transform.position;
                Quaternion rotationOfAttack = Quaternion.FromToRotation(Vector3.up, direction);
                rotationOfAttack *= Quaternion.Euler(0f, 0f, 90f);

                projectile = GameObject.Instantiate(stateData.projectile, transform.position, rotationOfAttack);
                projectileScript = projectile.GetComponent<Projectile>();

                projectileScript.CreateProjectile(stateData.projectileData);
                projectileScript.Init(gameObject);

                lastProjectileTime = Time.time;
            }
        }
    }

    public void CeilingOutAnimationDone()
    {
        animator.SetBool("ceilingOut", false);
        animator.SetBool("isFlying", true);
        isFlying = true;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, projectileRange);
    }
}
