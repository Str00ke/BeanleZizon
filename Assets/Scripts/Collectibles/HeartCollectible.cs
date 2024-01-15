using UnityEngine;

/// <summary>
/// Heart collectible
/// </summary>
public class HeartCollectible : ACollectible
{
    [SerializeField] private ParticleSystem _particleSystem;

    protected override void OnCollect()
    {
        Player.Instance.life++;

        _particleSystem.gameObject.SetActive(true);
        _particleSystem.transform.parent = null;
    }
}