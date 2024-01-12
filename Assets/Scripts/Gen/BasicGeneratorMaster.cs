using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    [SerializeField][Range(1, 5)] int _minSidePathRooms;
    [SerializeField][Range(1, 5)] int _maxSidePathRooms;

    [SerializeField][Range(1, 5)] int _minSidePathNbr;
    [SerializeField][Range(1, 5)] int _maxSidePathNbr;

    //TMP
    [SerializeField] private GameObject _entryRoom;
    [SerializeField] private List<GameObject> _doorUp = new();
    [SerializeField] private List<GameObject> _doorDown = new();
    [SerializeField] private List<GameObject> _doorLeft = new();
    [SerializeField] private List<GameObject> _doorRight = new();

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
        Random.InitState(System.DateTime.Now.Millisecond);
        Generate();
        //UnitTests();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            Reset();
            Generate();
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

    void Generate()
    {
        GenenerateGraphMainPath(_startPos, Cardinals.NORTH, true, 0, null);
        
        List<Vector2Int> _fullRoomsList = _rooms.Keys.ToList();
        int sidePathNbr = Random.Range(_minSidePathNbr, _maxSidePathNbr);
        for (int i = 0; i < sidePathNbr; i++) 
        {
            int rnd = Random.Range(0, _fullRoomsList.Count);
            GenGraphSidePath(_fullRoomsList[rnd], true, 0, Random.Range(_minSidePathRooms, _maxSidePathRooms));
            _fullRoomsList.RemoveAt(rnd);
        }

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

    void GenenerateGraphMainPath(Vector2Int pos, Cardinals currCardWay, bool isMainPath, int currIndex, BasicRoom prevRoom)
    {
        
        BasicRoom _currRoom = new BasicRoom(pos, currCardWay, prevRoom, _renderRooms);
        if (prevRoom != null)
        {
            prevRoom.NextRooms.Add(_currRoom);
            _currRoom.Connections.Add(currCardWay);
        }
        _rooms.Add(pos, _currRoom);
        if(_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(pos.x, pos.y, 0);
        }


        currIndex++;
        if (currIndex == _mapWalkthroughMaxSize)
        {
            _endRoomGen = true;
            return;
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

        CreateCorridor(pos, nextWay, Color.green);
        _currRoom.Connections.Add(~nextWay);

        GenenerateGraphMainPath(pos + GetOffset(nextWay), nextWay, isMainPath, currIndex, _currRoom);
    }

    void GenerateRooms()
    {
        //if (currIndex == 0)
        //{
        //    _currRoom = new BasicRoom(pos, _entryRoom, currCardWay, prevRoom, _renderRooms);
        //}
        //else
        //{
        //    List<GameObject> pool = GetCorrespondingCardPool(currCardWay);
        //    _currRoom = new BasicRoom(pos, pool[Random.Range(0, pool.Count - 1)], currCardWay, prevRoom, _renderRooms);
        //}
    }

    void GenGraphSidePath(Vector2Int parentRoomPos, bool parentIsMainPath, int currProgress, int currMaxProgress)
    {
        if (currProgress > currMaxProgress) //Stop current side path
            return;

        BasicRoom _parentRoom;
        if (parentIsMainPath)
            _parentRoom = _rooms[parentRoomPos];
        else
            _parentRoom = _sideRooms[parentRoomPos];

        List<Cardinals> possibilities = new();
        for (int c = 0; c < (int)Cardinals.COUNT; c++)
            if (!_rooms.ContainsKey(parentRoomPos + GetOffset((Cardinals)c)) && !_sideRooms.ContainsKey(parentRoomPos + GetOffset((Cardinals)c)))
                possibilities.Add((Cardinals)c);
        if(possibilities.Count == 0)
        {
            Debug.LogWarning("Path enclosed itself!");
            return;
        }
        Cardinals way = possibilities[Random.Range(0, possibilities.Count)];
        Vector2Int newPos = parentRoomPos + GetOffset(way);
        BasicRoom _currRoom = new BasicRoom(newPos, way, _parentRoom, _renderRooms);
        _currRoom.Connections.Add(way);
        if(parentIsMainPath)
            room(parentRoomPos).Connections.Add(way);
        else
            sideRoom(parentRoomPos).Connections.Add(~way);

        if (_parentRoom != null) _parentRoom.NextRooms.Add(_currRoom);
        _sideRooms.Add(newPos, _currRoom);
        if(_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(newPos.x, newPos.y, 0);
        }
        

        CreateCorridor(parentRoomPos, way, Color.red);

        GenGraphSidePath(newPos, false, currProgress + 1, currMaxProgress);
        
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
    }
}
