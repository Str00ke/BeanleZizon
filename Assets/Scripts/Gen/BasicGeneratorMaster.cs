using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class BasicGeneratorMaster : MonoBehaviour
{
    [SerializeField]
    Sprite _tmpRoomImg;

    [SerializeField] private Vector2Int _gridSize;

    [SerializeField] private int _mapWalkthroughMaxSize = 10;

    private Dictionary<Vector2Int, BasicRoom> _rooms = new();
    private Dictionary<Vector2Int, BasicRoom> _sideRooms = new();
 
    private Vector2Int _startPos = Vector2Int.zero;

    bool _endRoomGen = false;
    [SerializeField] private int _connectionChanceBeforeEnd;
    [SerializeField] private int _connectionChanceAfterEnd;

    [SerializeField] bool _renderDebug = false;
    [SerializeField] bool _renderRooms = false;

    private GameObject _debugRenderHolder;
    [SerializeField] private GameObject _dungeonRenderHolder;
    private GameObject _dungeonRenderHolderInstance;

    [SerializeField][Range(1, 5)] int _minSidePathRooms;
    [SerializeField][Range(1, 5)] int _maxSidePathRooms;

    [SerializeField][Range(1, 5)] int _minSidePathNbr;
    [SerializeField][Range(1, 5)] int _maxSidePathNbr;

    [SerializeField] DungeonData _data;

    private Element _currElement;
    private BasicRoom _startRoom;

    public BasicRoom room(Vector2Int value)
    {
        return _rooms[value];
    }

    public BasicRoom sideRoom(Vector2Int value)
    {
        return _sideRooms[value];
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_renderDebug)
            _debugRenderHolder = new GameObject("DebugRenderHolder");
        if (_renderRooms)
            _dungeonRenderHolderInstance = Instantiate(_dungeonRenderHolder);

        Random.InitState(System.DateTime.Now.Millisecond);
        //Generate();
        //UnitTests();
        _startRoom = new BasicRoom(_startPos, Cardinals.NORTH, null, _renderRooms);
        _rooms.Add(_startPos, _startRoom);
        _startRoom.Connections.Add(GetInvertedCardinal(Cardinals.NORTH, true, false));
        if (_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(_startPos.x, _startPos.y, 0);
            tmp.transform.parent = _debugRenderHolder.transform;
        }
        CreateCorridor(_startPos, Cardinals.NORTH, Color.green);
        _startRoom.SpawnRoom(_data.Rooms.Find(x => x.Name == "EntryRoom").Prefab, null);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            Reset();
            Generate();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            OnElementCollected(Element.Earth);
        }
    }

    void UnitTests()
    {
        for (int i = 0; i < 100; i++)
        {
            Debug.Log("Test nbr: " + i);

            Generate();
            Reset();
        }
        Debug.Log("OK!!!");
    }

    void Reset()
    {
        _endRoomGen = false;

        var arr = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        var arr2 = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);
        var arr3 = FindObjectsByType<Room>(FindObjectsSortMode.None);
        foreach (var obj in arr)
        {
            Destroy(obj.gameObject);
        }
        foreach (var obj in arr2)
        {
            Destroy(obj.gameObject);
        }
        foreach (var obj in arr3)
        {
            Destroy(obj.gameObject);
        }
        _rooms.Clear();
        _sideRooms.Clear();
        
    }

    Vector2Int GetOffset(Cardinals card)
    {
        switch (card)
        {
            case Cardinals.NORTH:
                return Vector2Int.up;

            case Cardinals.SOUTH:
                return Vector2Int.down;

            case Cardinals.EAST:
                return Vector2Int.left;

            case Cardinals.WEST:
                return Vector2Int.right;

            default:
                Debug.LogWarning("BasicGenMaster: GetOffset return null!");
                return Vector2Int.zero;
        }
    }

    public void OnElementCollected(Element el)
    {
        _dungeonRenderHolderInstance?.GetComponent<TilesetSwapper>().SetVariation("Earth");
        _currElement = el;
        Generate();
    }

    void Generate()
    {
        GenenerateGraphMainPath(_startPos + GetOffset(Cardinals.NORTH), Cardinals.NORTH, true, 1, _startRoom);

        List<Vector2Int> _fullRoomsList = _rooms.Keys.ToList();
        _fullRoomsList.RemoveAt(0);
        int sidePathNbr = Random.Range(_minSidePathNbr, _maxSidePathNbr);
        for (int i = 0; i < sidePathNbr; i++) 
        {
            int rnd = Random.Range(0, _fullRoomsList.Count);
            GenGraphSidePath(_fullRoomsList[rnd], true, 0, Random.Range(_minSidePathRooms, _maxSidePathRooms), Cardinals.NORTH);
            _fullRoomsList.RemoveAt(rnd);
        }

        if(_renderRooms)
            GenerateRooms(_rooms.ElementAt(1).Value, _startRoom);

        /*
        for (int i = 0; i < _rooms.Count; i++)
        {
            string str = "";
            for (int j = 0; j < _rooms.ElementAt(i).Value.Connections.Count; j++)
            {
                Cardinals c = (Cardinals)Enum.Parse(typeof(Cardinals), _rooms.ElementAt(i).Value.Connections[j].ToString());
                str += c + ", ";
            }
            Debug.Log(str);

        }

        Debug.Log("======================================================================");

        for (int i = 0; i < _sideRooms.Count; i++)
        {
            string str = "";
            for (int j = 0; j < _sideRooms.ElementAt(i).Value.Connections.Count; j++)
            {
                Cardinals c = (Cardinals)Enum.Parse(typeof(Cardinals), _sideRooms.ElementAt(i).Value.Connections[j].ToString());
                str += c + ", ";
            }
            Debug.Log(str);

        }
        */
    }

    List<Cardinals> GetNextRandCard(Cardinals currCardWay, Vector2Int pos)
    {
        List<Cardinals> nextRandCard = new();
        switch (currCardWay)
        {
            case Cardinals.NORTH:
                nextRandCard.Add(Cardinals.EAST);
                nextRandCard.Add(Cardinals.WEST);
                break;

            case Cardinals.SOUTH:
                nextRandCard.Add(Cardinals.EAST);
                nextRandCard.Add(Cardinals.WEST);
                break;

            case Cardinals.EAST:
                nextRandCard.Add(Cardinals.NORTH);
                nextRandCard.Add(Cardinals.SOUTH);
                break;

            case Cardinals.WEST:
                nextRandCard.Add(Cardinals.NORTH);
                nextRandCard.Add(Cardinals.SOUTH);
                break;

        }

        for (int next = nextRandCard.Count - 1; next >= 0; next--)
        {
            Cardinals c = nextRandCard[next];
            if (_rooms.ContainsKey(pos + GetOffset(c)))
            {
                nextRandCard.Remove(c);
            }
        }
        return nextRandCard;
    }

    List<RoomData> GetCorrespondingCardRooms(BasicRoom basicRoom) //sort every room to keep these with doors corresponding with connections
    {
        var _dataRooms = _data.Rooms;
        for (ushort i = 0; i < (ushort)Cardinals.COUNT; i++) //foreach existing directions
        {
            Cardinals currC = (Cardinals)i;
            if (basicRoom.Connections.Contains(currC)) continue; //If false, current room to check does not contain this door orientation

            foreach (var _dRoom in _dataRooms) //foreach room
            {
                List<Door> _doors = _dRoom.Doors;
                Debug.Log(_doors.Count);
                foreach (Door _door in _doors) //foreach doors in this room
                {
                    Debug.Log(_door.Orientation);
                    Debug.Log(currC);
                    if (_door.Orientation == UtilsConverter.CardToOrient(currC)) //if a door orient same as searched cardinal, remove it
                    {
                        _dataRooms.Remove(_dRoom);
                        break;
                    }
                }
            }
        }
        return _dataRooms;
    }

    List<Vector2Int> GetCorrespondingCardPool(BasicRoom basicRoom) //sort every room to keep these with doors corresponding with connections
    {
        List<Vector2Int> _fullRoomsList = _rooms.Keys.ToList();
        for (ushort i = 0; i < (ushort)Cardinals.COUNT; i++) //foreach existing directions
        {
            Cardinals currC = (Cardinals)i;
            if (basicRoom.Connections.Contains(currC)) continue; //If current room contains direction no need to compute

            foreach (Vector2Int elt in _fullRoomsList) //foreach room
            {
                Room _room = room(elt).TilemapRoom;
                List<Door> _doors = _room.GetDoors();
                foreach (Door _door in _doors) //foreach doors in this room
                {
                    if (_door.Orientation == Utils.ORIENTATION.NORTH) //if a door orient same as searched cardinal, remove it
                    {
                        _fullRoomsList.Remove(elt);
                        break;
                    }
                }
            }
        }
        return _fullRoomsList;
    }

    public static Cardinals GetInvertedCardinal(Cardinals baseCardinal, bool invertHorizontal, bool invertVertical)
    {
        //Cardinals inverted = ~baseCardinal;

        //bool GetBit(byte value, int bit) => (value & (1 << bit)) != 0;

        //if (GetBit((byte)inverted, 0))
        //{
        //    if (GetBit((byte)inverted, 1))
        //        return Cardinals.NORTH;
        //    else return Cardinals.EAST;
        //}
        //else
        //{
        //    if (GetBit((byte)inverted, 1))
        //        return Cardinals.WEST;
        //    else return Cardinals.SOUTH;
        //}

        switch (baseCardinal)
        {
            case Cardinals.NORTH: return invertVertical ? Cardinals.SOUTH : Cardinals.NORTH;
            case Cardinals.SOUTH: return invertVertical ? Cardinals.NORTH : Cardinals.SOUTH;
            case Cardinals.EAST: return invertHorizontal ? Cardinals.WEST : Cardinals.EAST;
            case Cardinals.WEST: return invertHorizontal ? Cardinals.EAST : Cardinals.WEST;
            default: Debug.Log("Get inverted cardinal return error!"); return Cardinals.NORTH;
        }
    }

    void GenenerateGraphMainPath(Vector2Int pos, Cardinals currCardWay, bool isMainPath, int currIndex, BasicRoom prevRoom)
    {
        
        BasicRoom _currRoom = new BasicRoom(pos, currCardWay, prevRoom, _renderRooms);
        if (prevRoom != null)
        {
            prevRoom.NextRooms.Add(_currRoom);
            _currRoom.Connections.Add(GetInvertedCardinal(currCardWay, false, true));
        }
        _rooms.Add(pos, _currRoom);
        if(_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(pos.x, pos.y, 0);
            tmp.transform.parent = _debugRenderHolder.transform;
        }
        Cardinals nextWay;
        if(currIndex == 0)
        {
            nextWay = Cardinals.NORTH;
        }
        else
        {
            bool keepWay = Random.Range(0, 100) <= 50;

            List<Cardinals> nextRandCard = new();
            if (!keepWay)
                nextRandCard = GetNextRandCard(currCardWay, pos);

            nextWay = keepWay ? currCardWay : nextRandCard[Random.Range(0, nextRandCard.Count)]; //@TODO: Fix main path full circle colliding with own path
        }

        currIndex++;
        if (currIndex == _mapWalkthroughMaxSize)
        {
            _endRoomGen = true;
            _currRoom.state = BasicRoom.State.BOSS;
            return;
        }

        CreateCorridor(pos, nextWay, Color.green);
        if (prevRoom != null)
            _currRoom.Connections.Add(GetInvertedCardinal(nextWay, true, false));
        else
            _currRoom.Connections.Add(GetInvertedCardinal(currCardWay, false, false));



        GenenerateGraphMainPath(pos + GetOffset(nextWay), nextWay, isMainPath, currIndex, _currRoom);
    }

    

    void GenGraphSidePath(Vector2Int parentRoomPos, bool parentIsMainPath, int currProgress, int currMaxProgress, Cardinals parentWay)
    {
        
        BasicRoom _parentRoom;
        if (parentIsMainPath)
            _parentRoom = _rooms[parentRoomPos];
        else
            _parentRoom = _sideRooms[parentRoomPos];

        List<Cardinals> possibilities = new();
        //for (int c = 0; c < (int)Cardinals.COUNT; c++)
        //    if (!_rooms.ContainsKey(parentRoomPos + GetOffset(c)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset(c)))
        //        possibilities.Add((Cardinals)c);
        //TODO
        if (!_rooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.NORTH)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.NORTH)))
            possibilities.Add(Cardinals.NORTH);
        if (!_rooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.SOUTH)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.SOUTH)))
            possibilities.Add(Cardinals.SOUTH);
        if (!_rooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.EAST)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.EAST)))
            possibilities.Add(Cardinals.EAST);
        if (!_rooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.WEST)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset(Cardinals.WEST)))
            possibilities.Add(Cardinals.WEST);
        if (possibilities.Count == 0)
        {
            Debug.LogWarning("Path enclosed itself!");
            return;
        }
        Cardinals way = possibilities[Random.Range(0, possibilities.Count)];
        Vector2Int newPos = parentRoomPos + GetOffset(way);
        BasicRoom _currRoom = new BasicRoom(newPos, way, _parentRoom, _renderRooms);
        _currRoom.Connections.Add(GetInvertedCardinal(way, false, true));
        if(parentIsMainPath)
        {
            BasicRoom parent = room(parentRoomPos);
            parent.Connections.Add(GetInvertedCardinal(way, true, false));

            foreach (var door in parent.NextRooms)
            {
                if(door.GraphPos.x == parent.GraphPos.x)
                {
                    if (door.GraphPos.y > parent.GraphPos.y) //North
                        parent.ClosedDoors.Add(Cardinals.NORTH);
                    else //South
                        parent.ClosedDoors.Add(Cardinals.SOUTH);
                }
                else
                {
                    if (door.GraphPos.x > parent.GraphPos.x) //East
                        parent.ClosedDoors.Add(Cardinals.EAST);
                    else // West
                        parent.ClosedDoors.Add(Cardinals.WEST);
                }
            }

        }
        else
            sideRoom(parentRoomPos).Connections.Add(GetInvertedCardinal(way, true, false));

        if (_parentRoom != null) _parentRoom.NextRooms.Add(_currRoom);
        _sideRooms.Add(newPos, _currRoom);
        if(_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(newPos.x, newPos.y, 0);
            tmp.transform.parent = _debugRenderHolder.transform;
        }


        CreateCorridor(parentRoomPos, way, Color.red);

        if (currProgress <= currMaxProgress) //Stop current side path
            GenGraphSidePath(newPos, false, currProgress + 1, currMaxProgress, GetInvertedCardinal(way, true, false));
        else
        {
            _currRoom.state = BasicRoom.State.KEY;
            return;
        }

    }

    void GenerateRooms(BasicRoom _room, BasicRoom _parent)
    {
        if(_parent == null)
            _room.SpawnRoom(_data.Rooms.Find(x => x.Name == "EntryRoom").Prefab, _parent);
        else
        {
            /*
            if(!_room.Connections.Contains(Cardinals.EAST) && !_room.Connections.Contains(Cardinals.WEST) && false) //Can spawn 1x2
            {
                if (Random.Range(0, 100) <= 35)
                {
                    var arr = _data.Rooms.FindAll(x => x.Prefab.GetComponent<Room>().Size.y == 2 && x.Prefab.GetComponent<Room>().Element == Element.Earth);
                    _room.SpawnRoom(arr[Random.Range(0, arr.Count)].Prefab, _parent);
                }
                else
                {
                    var arr = _data.Rooms.FindAll(x => x.Prefab.GetComponent<Room>().Size.y == 1 && x.Prefab.GetComponent<Room>().Element == Element.Earth);
                    _room.SpawnRoom(arr[Random.Range(0, arr.Count)].Prefab, _parent);
                }
            }
            else
            {
                var arr = _data.Rooms.FindAll(x => x.Prefab.GetComponent<Room>().Size.y == 1 && x.Prefab.GetComponent<Room>().Element == Element.Earth);
                _room.SpawnRoom(arr[Random.Range(0, arr.Count)].Prefab, _parent);
            }
            */
            if(_room.state == BasicRoom.State.KEY)
                _room.SpawnRoom(_data.Rooms.Find(x => x.Name == "Key Room").Prefab, _parent);
            else
            {
                var arr = _data.Rooms.FindAll(x => x.Prefab.GetComponent<Room>().Size.y == 1 && x.Prefab.GetComponent<Room>().Element == Element.Earth);
                _room.SpawnRoom(arr[Random.Range(0, arr.Count)].Prefab, _parent);
            }
        }
        _room.TilemapRoom.transform.parent = _dungeonRenderHolderInstance.transform;
        foreach(BasicRoom child in _room.NextRooms) GenerateRooms(child, _room);
    }

    void CreateCorridor(Vector2Int pos, Cardinals nextWay, Color color) //For now, just create line renderer
    {
        if (!_renderDebug) return;
        GameObject lrGo = new GameObject("Connection");
        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(pos.x, pos.y, 0));
        Vector2Int nextVec = pos + GetOffset(nextWay);
        Vector3 nextPos = new Vector3(nextVec.x, nextVec.y, 0);
        lr.SetPosition(1, nextPos);
        lr.widthMultiplier = 0.1f;
        lrGo.transform.parent = _debugRenderHolder.transform;
    }
}
