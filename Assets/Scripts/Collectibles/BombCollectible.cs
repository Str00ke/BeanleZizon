using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollectible : ACollectible
{
    protected override void OnCollect()
    {
        Player.Instance.BombCount++;
    }
}
