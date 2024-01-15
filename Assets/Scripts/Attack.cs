using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Attack is an hitbox script that destroy itself after a given lifetime or when triggered. When
/// hitting player or enemy, applies damages and knockback to hit entity. See Player and Enemy
/// "OnTriggerEnter2D" method for more details.
/// </summary>
public class Attack : MonoBehaviour
{
    public int damages = 1;
    public bool hasInfiniteLifetime = false;
    [HideIf("hasInfiniteLifetime")]
    public float lifetime = 0.3f;
    public float knockbackSpeed = 3;
    public float knockbackDuration = 0.5f;
    public LayerMask destroyOnHit;
    public Element element = Element.None;

    [System.NonSerialized]
    public GameObject owner;

    private void Update()
    {
        if (hasInfiniteLifetime)
            return;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ExplosiveBarrel barrel))
        {
            barrel.ApplyHit(this);
        }
    }
}