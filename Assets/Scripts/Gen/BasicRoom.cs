using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoom
{
    private Vector2Int _pos;

    private List<Room> _connectedRooms = new();

    private List<BasicRoom> _nextRooms = new();
    public List<BasicRoom> NextRooms => _nextRooms;

    private Cardinals _fromPrevRoomOffset; //This room will spawn NSEW from previous room

    private GameObject _goRoom;
    private Room _tilemapRoom;

    public BasicRoom(Vector2Int pos, GameObject room, Cardinals fromPrevRoomOffset)
    {
        _pos = pos * room.GetComponent<Room>().Size * 10;
        _fromPrevRoomOffset = fromPrevRoomOffset;
        _goRoom = GameObject.Instantiate(room, (Vector3Int)_pos, Quaternion.identity);
        _tilemapRoom = _goRoom.GetComponent<Room>();
    }

    public void Init()
    {

    }
}
