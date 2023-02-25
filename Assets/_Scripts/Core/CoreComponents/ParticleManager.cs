using UnityEngine;

public class ParticleManager : CoreComponent
{
  private Transform particleContainer;

  private Movement movement;
  public Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

  protected override void Awake()
  {
    base.Awake();

    particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;
  }

  public GameObject StartParticlesWithRandomRotation(GameObject particlesPrefab)
  {
    var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    return StartParticles(particlesPrefab, transform.position, randomRotation);
  }
  
  public GameObject StartParticlesWithRandomRotation(GameObject particlesPrefab, Vector2 offset)
  {
    var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    return StartParticles(particlesPrefab, FindOffset(offset), randomRotation);
  }

  public GameObject StartParticles(GameObject particlesPrefab)
  {
    return StartParticles(particlesPrefab, transform.position, Quaternion.identity);
  }

  public GameObject StartParticles(GameObject particlesPrefab, Vector2 offset)
  {
    return StartParticles(particlesPrefab, FindOffset(offset), Quaternion.identity);
  }

  public GameObject StartParticles(GameObject particlesPrefab, Vector2 position, Quaternion rotation)
  {
    return Instantiate(particlesPrefab, position, rotation, particleContainer);
  }

  private Vector2 FindOffset(Vector2 offset)
  {
    offset.x *= Movement.FacingDirection;

    return transform.position + (Vector3)offset;
  }
}
