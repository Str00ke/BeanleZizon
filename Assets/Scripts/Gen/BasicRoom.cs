using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BasicRoom
{

    public enum State
    {
        BASIC = 0,
        KEY = 1,
        BOSS = 2
    }
    private State _state;
    public State state;

    private Vector2Int _pos;
    private Vector2Int _graphPos;
    public Vector2Int GraphPos => _graphPos;


    private List<BasicRoom> _nextRooms = new();
    public List<BasicRoom> NextRooms => _nextRooms;

    private List<Cardinals> _connections = new();
    public List <Cardinals> Connections => _connections;

    private Cardinals _fromPrevRoomOffset; //This room will spawn NSEW from previous room

    private GameObject _goRoom;
    public GameObject GoRoom => _goRoom;
    private Room _tilemapRoom;
    public Room TilemapRoom => _tilemapRoom;

    private List<Cardinals> _closedDoors = new();
    public List<Cardinals> ClosedDoors => _closedDoors;

    public BasicRoom(Vector2Int pos, Cardinals fromPrevRoomOffset, BasicRoom prevRoom, bool renderRooms)
    {
        _pos = pos;
        _graphPos = pos;
        _fromPrevRoomOffset = fromPrevRoomOffset;
    }

    public void SpawnRoom(GameObject room, BasicRoom prevRoom)
    {
        _goRoom = GameObject.Instantiate(room, (Vector3Int)_pos, Quaternion.identity);
        _tilemapRoom = _goRoom.GetComponent<Room>();
        _tilemapRoom.Position_DoNotEdit = _graphPos;
        if(prevRoom != null)
        {
            Vector2Int prevPos = prevRoom._pos;
            switch (_fromPrevRoomOffset)
            {
                case Cardinals.NORTH:
                    _pos.x = prevPos.x;
                    _pos.y = prevPos.y + (_goRoom.GetComponent<Room>().TotalTiledSize.y);
                    break;

                case Cardinals.SOUTH:
                    _pos.x = prevPos.x;
                    _pos.y = prevPos.y - (_goRoom.GetComponent<Room>().TotalTiledSize.y);
                    break;

                case Cardinals.EAST:
                    _pos.x = prevPos.x - (_goRoom.GetComponent<Room>().TotalTiledSize.x);
                    _pos.y = prevPos.y;
                    break;

                case Cardinals.WEST:
                    _pos.x = prevPos.x + (_goRoom.GetComponent<Room>().TotalTiledSize.x);
                    _pos.y = prevPos.y;
                    break;
            }

            //if(_goRoom.GetComponent<Room>().TotalTiledSize.y == 18 && _fromPrevRoomOffset == Cardinals.NORTH)
            //{
            //    _pos.y -= 9;
            //}
        }
        
        _goRoom.transform.position = (Vector3Int)_pos;

        foreach(var door in _tilemapRoom.GetDoors())
        {
            if (!Connections.Contains(UtilsConverter.OrientToCard(door.Orientation)))
            {
                TilesetSpriteSwapper.TilesetSprite tSpr;
                WallSpriteSetter setter = BasicGeneratorMaster.Instance.WallSpriteSetter;

                WallSpriteSetter.ElementSprSet val;
                if (_tilemapRoom.Element == Element.None)
                {
                    val = setter.Sets.Find(x => x.elementName == "None");
                    tSpr.variationId = "None";
                }
                else
                {
                    val = setter.Sets.Find(x => x.element == BasicGeneratorMaster.Instance.CurrElement);
                    tSpr.variationId = val.elementName;
                    door.wallGo.GetComponent<TilesetSpriteSwapper>().spritePerTileset.RemoveAll(x => x.variationId == val.elementName);
                }

                door.wallGo.transform.rotation = Quaternion.identity;

                switch (UtilsConverter.OrientToCard(door.Orientation))
                {
                    case Cardinals.NORTH:
                        tSpr.sprite = val.NorthSpr.sprite;
                        break;
                    case Cardinals.SOUTH:
                        door.wallGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        tSpr.sprite = val.SouthSpr.sprite;
                        break;
                    case Cardinals.EAST:
                        door.wallGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                        tSpr.sprite = val.EastSpr.sprite;
                        break;
                    case Cardinals.WEST:
                        door.wallGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                        tSpr.sprite = val.WestSpr.sprite;
                        break;
                    default:
                        tSpr.sprite = val.NorthSpr.sprite;
                        break;
                }
                door.SetState(Door.STATE.WALL);
                door.wallGo.GetComponent<TilesetSpriteSwapper>().spritePerTileset.Add(tSpr);
                door.wallGo.GetComponent<TilesetSpriteSwapper>().SetVariation(val.elementName);

            }
            else if(ClosedDoors.Contains(UtilsConverter.OrientToCard(door.Orientation))) door.SetState(Door.STATE.CLOSED);
            else door.SetState(Door.STATE.OPEN);
        }
    }

}
