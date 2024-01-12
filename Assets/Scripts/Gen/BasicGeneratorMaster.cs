using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

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

    //TMP
    [SerializeField] private List<GameObject> _earth1x1Rooms = new();

    public BasicRoom room(Vector2Int value)
    {
        return _rooms[value];
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
        /*switch (card)
        {
            case Cardinals.NORTH:
                return new Vector2Int(_currPos.x, _currPos.y + 1);

            case Cardinals.SOUTH:
                return new Vector2Int(_currPos.x, _currPos.y - 1);

            case Cardinals.EAST:
                return new Vector2Int(_currPos.x - 1, _currPos.y);

            case Cardinals.WEST:
                return new Vector2Int(_currPos.x + 1, _currPos.y);

            default:
                Debug.LogWarning("BasicGenMaster: GetOffset return null!");
                return Vector2Int.zero;
        }*/

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
        GenAtRoom(_startPos, (Cardinals)Random.Range(0, (int)Cardinals.COUNT), true, 0, null);
        //GenDeviated(_startPos, true);
    }

    List<Cardinals> GetNextRandCard(Cardinals currCardWay, Vector2Int pos)
    {
        List<Cardinals> nextRandCard = new();
        switch (currCardWay)
        {
            //case Cardinals.NORTH | Cardinals.SOUTH:
            //    nextRandCard.Add(Cardinals.EAST);
            //    nextRandCard.Add(Cardinals.WEST);
            //    break;

            //case Cardinals.EAST | Cardinals.WEST:
            //    nextRandCard.Add(Cardinals.NORTH);
            //    nextRandCard.Add(Cardinals.SOUTH);
            //    break;

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

    void GenAtRoom(Vector2Int pos, Cardinals currCardWay, bool isMainPath, int currIndex, BasicRoom prevRoom)
    {
        BasicRoom _currRoom = new BasicRoom(pos, _earth1x1Rooms[Random.Range(0, _earth1x1Rooms.Count - 1)], currCardWay);
        if(prevRoom != null) prevRoom.NextRooms.Add(_currRoom);
        _rooms.Add(pos, _currRoom);
        if(_renderDebug)
        {
            GameObject tmp = new GameObject("testRoom");
            tmp.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
            sr.sprite = _tmpRoomImg;
            tmp.transform.position = new Vector3(pos.x, pos.y, 0);
        }
        

        bool[] possibilites = new bool[(int)Cardinals.COUNT];/*{ Cardinals.NORTH, Cardinals.SOUTH, Cardinals.EAST, Cardinals.WEST };*/

        //Check if OOB
        possibilites[(ushort)Cardinals.NORTH] = !((pos + GetOffset(Cardinals.NORTH)).x < 0);
        possibilites[(ushort)Cardinals.SOUTH] = !((pos + GetOffset(Cardinals.SOUTH)).x > _gridSize.x);
        possibilites[(ushort)Cardinals.EAST] = !((pos + GetOffset(Cardinals.EAST)).y < 0);
        possibilites[(ushort)Cardinals.WEST] = !((pos + GetOffset(Cardinals.WEST)).y > _gridSize.y);


        bool keepWay = Random.Range(0, 100) <= 50;

        List<Cardinals> nextRandCard = new();
        if (!keepWay)
            nextRandCard = GetNextRandCard(currCardWay, pos);

        //Cardinals nextWay = keepWay ? currCardWay : nextRandCard[Random.Range(0, nextRandCard.Count - 1)];
        Cardinals nextWay;
        if (keepWay)
            nextWay = currCardWay;
        else
        {
            nextWay = nextRandCard[Random.Range(0, nextRandCard.Count - 1)];
        }


        if (isMainPath)
        {
            if (currIndex == _mapWalkthroughMaxSize)
            {
                _endRoomGen = true;
                return;
            }
            CreateCorridor(pos, nextWay, Color.green);
            GenAtRoom(pos + GetOffset(nextWay), nextWay, isMainPath, currIndex + 1, _currRoom);
        }
    }

    void GenAtRoomTMP(Vector2Int pos, Cardinals currCardWay, bool isMainPath)
    {
        /*
        //Debug.Log(pos);
        _rooms.Add(pos, new BasicRoom());
        GameObject tmp = new GameObject("testRoom");
        tmp.AddComponent<BasicRoom>();
        SpriteRenderer sr = tmp.AddComponent<SpriteRenderer>();
        sr.sprite = _tmpRoomImg;
        tmp.transform.position = new Vector3(pos.x, pos.y, 0);


        _currMapWalkthroughSize++;
        if(_currMapWalkthroughSize >= _mapWalkthroughMaxSize)
        {
            _endRoomGen = true;
            return;
        }

        bool[] possibilites = new bool[(int)Cardinals.COUNT];//{ Cardinals.NORTH, Cardinals.SOUTH, Cardinals.EAST, Cardinals.WEST };

        //Check if OOB
        possibilites[(ushort)Cardinals.NORTH] = !((pos + GetOffset(Cardinals.NORTH)).x < 0);
        possibilites[(ushort)Cardinals.SOUTH] = !((pos + GetOffset(Cardinals.SOUTH)).x > _gridSize.x);
        possibilites[(ushort)Cardinals.EAST] = !((pos + GetOffset(Cardinals.EAST)).y < 0);
        possibilites[(ushort)Cardinals.WEST] = !((pos + GetOffset(Cardinals.WEST)).y > _gridSize.y);

        //For cardinals:
        for (ushort i = 0; i < (int)Cardinals.COUNT; i++)
        {
            if (possibilites[i])
            {
                //Check if room exist
                if (_rooms.ContainsKey(pos + GetOffset((Cardinals)i))) //Connect
                {
                    //l> if yes, rand connect
                    bool connect = Random.Range(0, 100) <= (_endRoomGen ? _connectionChanceAfterEnd : _connectionChanceBeforeEnd);
                    if (connect)
                    {
                        GameObject lrGo = new GameObject("Connection");
                        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, new Vector3(pos.x, pos.y, 0));
                        Vector2Int tmpVec = pos + GetOffset((Cardinals)i);
                        Vector3 tmpP = new Vector3(tmpVec.x, tmpVec.y, 0);
                        lr.SetPosition(1, tmpP);
                        lr.widthMultiplier = 0.1f;
                    }
                }
                else if (!_endRoomGen) //Create
                {
                    //l> else, rand create
                    bool create = Random.Range(0, 100) <= 50;
                    bool keepWay = Random.Range(0, 100) <= 50;
                    Cardinals nextWay = keepWay ? currCardWay : (Cardinals)Random.Range(0, (int)Cardinals.COUNT); //Can get same way though, might fix
                    Debug.Log(nextWay);
                    if (isMainPath)
                    {
                        GameObject lrGo = new GameObject("Connection");
                        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, new Vector3(pos.x, pos.y, 0));
                        Vector2Int nextVec = pos + GetOffset(nextWay);
                        Vector3 nextPos = new Vector3(nextVec.x, nextVec.y, 0);
                        lr.SetPosition(1, nextPos);
                        lr.widthMultiplier = 0.1f;
                        GenAtRoom(pos + GetOffset(nextWay), nextWay, isMainPath);
                    }
                    else if (create)
                    {
                        GameObject lrGo = new GameObject("Connection");
                        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, new Vector3(pos.x, pos.y, 0));
                        Vector2Int tmpVec = pos + GetOffset((Cardinals)i);
                        Vector3 tmpP = new Vector3(tmpVec.x, tmpVec.y, 0);
                        lr.SetPosition(1, tmpP);
                        lr.widthMultiplier = 0.1f;
                        GenAtRoom(pos + GetOffset((Cardinals)i), nextWay, isMainPath);
                    }
                }
            }
        }
        */
    }


    void GenDeviated(Vector2Int parentRoomPos, bool parentIsMainPath)
    {
        if(parentIsMainPath)
            if (Random.Range(0, 100) <= 50) return;


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
        BasicRoom _currRoom = new BasicRoom(newPos, _earth1x1Rooms[Random.Range(0, _earth1x1Rooms.Count - 1)], way);
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


        if (Random.Range(0, 100) <= 90) //Continue current side path
        {
            GenDeviated(newPos, false);
        }
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
