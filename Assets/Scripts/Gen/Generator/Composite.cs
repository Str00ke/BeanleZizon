using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composite
{
    private CompositeType _compositeType;

    private Bounds _bounds;
    public Bounds Bounds => _bounds;

    private List<BasicRoom> _compRooms;

    private CompositeRenderer _renderer;

    private Vector2 _position;

    public void Generate()
    {
        _bounds = new Bounds();
        _bounds.extents = new Vector3Int(Random.Range(5, 15) / 2, Random.Range(5, 15) / 2, 1);
        _renderer = new GameObject("Composite").AddComponent<CompositeRenderer>();
        _renderer.Init(this);
    }
}
