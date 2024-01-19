using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorMaster : MonoBehaviour
{
    private List<Composite> _comps = new();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void CreateComposite()
    {
        Composite comp = new Composite();
        comp.Generate();
        _comps.Add(comp);
    }
}
