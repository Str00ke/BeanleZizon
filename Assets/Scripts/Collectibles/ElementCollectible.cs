using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollectible : ACollectible
{
    public Element element = Element.None;
    public ElementCollectible[] allEllementCollectible;

    private void Start()
    {
        allEllementCollectible = FindObjectsOfType<ElementCollectible>();
    }

    protected override void OnCollect()
    {
        Player.Instance.element = element;
        FindObjectOfType<BasicGeneratorMaster>().OnElementCollected(element);

        for (int i = allEllementCollectible.Length - 1; i >= 0; --i)
        {
            if (allEllementCollectible[i] == this)
                continue;

            Destroy(allEllementCollectible[i].gameObject);
        }
    }
}