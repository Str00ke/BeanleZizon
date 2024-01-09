using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bomb collectible
/// </summary>
public class BombCollectible : ACollectible
{
    protected override void OnCollect()
    {
        Player.Instance.BombCount++;
    }
}
