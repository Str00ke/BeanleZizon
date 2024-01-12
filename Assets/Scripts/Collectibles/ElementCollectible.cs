using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollectible : ACollectible
{
    public Element element = Element.None;
    protected override void OnCollect()
    {
        Player.Instance.element = element;
    }
}
