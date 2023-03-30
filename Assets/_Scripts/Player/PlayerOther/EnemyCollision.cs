using UnityEngine;
using Project.Interfaces;
using System.Collections;

// Script Responsible for handling Knockack and Damage On Enemy COLLISION.
public class EnemyCollision : CoreComponent
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    private Death Death { get => death ?? core.GetCoreComponent(ref death); }
    private Death death;

    private PlayerInventory PlayerInventory { get => playerInventory ?? core.GetCoreComponent(ref playerInventory); }
    private PlayerInventory playerInventory;

    private CoinValueSet coin;

    [SerializeField]
    private Vector2 knockbackAngle;
    [SerializeField]
    private float knockbackStrength;
    [SerializeField]
    private float collisionDamage;

    private DamageData damageData;
    public bool CanSetDeathZoneCollision { get; set; }

    protected override void Awake()
    {
        CanSetDeathZoneCollision = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (core.Parent.name == "Player")
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Help");
                var data = new KnockbackData(knockbackAngle, knockbackStrength, -Movement.FacingDirection, core.Parent);
                core.Parent.transform.GetChild(0).Find("Combat").GetComponent<IKnockbackable>().Knockback(data);
                damageData.SetData(core.Parent, collisionDamage);
                core.Parent.transform.GetChild(0).Find("Combat").GetComponent<IDamageable>().Damage(damageData);
            }

            if (collision.gameObject.CompareTag("Collectible"))
            {
                coin = collision.gameObject.GetComponent<CoinValueSet>();
                if (coin.CanCollect)
                {
                    PlayerInventory.IncreaseCoins(coin.CoinValue);
                    Destroy(collision.gameObject);
                }
            }
        }

        if (collision.gameObject.CompareTag("DeathZone"))
        {
            if (CanSetDeathZoneCollision)
            {
                CanSetDeathZoneCollision = false;
                Death.DeathZoneDamage();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (core.Parent.name == "Player")
        {
            if (collision.gameObject.CompareTag("Collectible"))
            {
                coin = collision.gameObject.GetComponent<CoinValueSet>();
                if (coin.CanCollect)
                {
                    PlayerInventory.IncreaseCoins(coin.CoinValue);
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
