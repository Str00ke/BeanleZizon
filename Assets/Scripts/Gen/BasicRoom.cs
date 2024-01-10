using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoom
{
    private Vector2Int _pos;

    private List<Room> _connectedRooms = new();

    private List<BasicRoom> _nextRooms = new();
    public List<BasicRoom> NextRooms => _nextRooms;

    public BasicRoom(Vector2Int pos)
    {
        _pos = pos;
    }

    public void Init()
    {

    }
}
