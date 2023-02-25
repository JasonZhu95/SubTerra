using UnityEngine;
using Project.Combat.Interfaces;

public class CombatTestDummy : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] private GameObject hitParticles;

    private Animator anim;
    private Rigidbody2D rb;

    public void Damage(DamageData data)
    {
        Instantiate(hitParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        anim.SetTrigger("damage");
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Knockback(KnockbackData data)
    {
        data.Angle.Normalize();
        rb.velocity = new Vector2(data.Strength * data.Angle.x * data.Direction, data.Strength * data.Angle.y);
    }
}
