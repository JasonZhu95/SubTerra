using UnityEngine;
using Project.Combat.Interfaces;

public class EnemyCollision : CoreComponent
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    private Death Death { get => death ?? core.GetCoreComponent(ref death); }
    private Death death;

    [SerializeField]
    private Vector2 knockbackAngle;
    [SerializeField]
    private float knockbackStrength;

    private Vector3 collisionDirection;
    private DamageData damageData;

    private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var data = new KnockbackData(knockbackAngle, knockbackStrength, -Movement.FacingDirection, transform.parent.parent.gameObject);
            GameObject.Find("Combat").GetComponent<IKnockbackable>().Knockback(data);
            damageData.SetData(transform.parent.parent.gameObject, 10f);
            GameObject.Find("Combat").GetComponent<IDamageable>().Damage(damageData);
        }

        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Death.Die();
        }
    }

}
