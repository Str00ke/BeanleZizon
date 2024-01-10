using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeRenderer : MonoBehaviour
{
    private Composite _data;
    private BoxCollider2D _boxCollider;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Render()
    {

    }

    public void Init(Composite data)
    {
        _data = data;
        _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        _boxCollider.size = _data.Bounds.size;
    }
}
