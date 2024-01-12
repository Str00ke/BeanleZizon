using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BasicRoom
{
    private Vector2Int _pos;


    private List<BasicRoom> _nextRooms = new();
    public List<BasicRoom> NextRooms => _nextRooms;

    private List<Cardinals> _connections = new();
    public List <Cardinals> Connections => _connections;

    private Cardinals _fromPrevRoomOffset; //This room will spawn NSEW from previous room

    private GameObject _goRoom;
    private Room _tilemapRoom;
    public Room TilemapRoom => _tilemapRoom;

    public BasicRoom(Vector2Int pos, Cardinals fromPrevRoomOffset, BasicRoom prevRoom, bool renderRooms)
    {
        _pos = pos;
        _fromPrevRoomOffset = fromPrevRoomOffset;
    }

    public void SpawnRoom(GameObject room)
    {
        //_goRoom = GameObject.Instantiate(room, (Vector3Int)pos, Quaternion.identity);
        //_tilemapRoom = _goRoom.GetComponent<Room>();

        //Vector2Int prevPos = prevRoom._pos;
        //switch (_fromPrevRoomOffset)
        //{
        //    case Cardinals.NORTH:
        //        _pos.x = prevPos.x;
        //        _pos.y = prevPos.y + (prevRoom._goRoom.GetComponent<Room>().TotalTiledSize.y);
        //        break;

        //    case Cardinals.SOUTH:
        //        _pos.x = prevPos.x;
        //        _pos.y = prevPos.y - (prevRoom._goRoom.GetComponent<Room>().TotalTiledSize.y);
        //        break;

        //    case Cardinals.EAST:
        //        _pos.x = prevPos.x + (prevRoom._goRoom.GetComponent<Room>().TotalTiledSize.x);
        //        _pos.y = prevPos.y;
        //        break;

        //    case Cardinals.WEST:
        //        _pos.x = prevPos.x - (prevRoom._goRoom.GetComponent<Room>().TotalTiledSize.x);
        //        _pos.y = prevPos.y;
        //        break;
        //}
        _goRoom.transform.position = (Vector3Int)_pos;
    }
}
