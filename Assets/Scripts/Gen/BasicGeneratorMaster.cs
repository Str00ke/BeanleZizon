using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class BasicGeneratorMaster : MonoBehaviour
{
    enum Cardinals : ushort
    {
        NORTH = 0,
        SOUTH = 1,
        WEST = 2,
        EAST = 3
    };

    [SerializeField]
    Sprite _tmpRoomImg;

    [SerializeField] private Vector2Int _gridSize;

    [SerializeField] private int _mapWalkthroughMaxSize = 10;
    private int _currMapWalkthroughSize = 0;

    private Dictionary<Vector2Int, BasicRoom> _rooms = new();
 
    private Vector2Int _startPos = Vector2Int.zero;

    bool _endRoomGen = false;
    [SerializeField] private int _connectionChanceBeforeEnd;
    [SerializeField] private int _connectionChanceAfterEnd;

    public BasicRoom room(Vector2Int value)
    {
        return _rooms[value];
    }

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            _currMapWalkthroughSize = 0;
            _endRoomGen = false;

            var arr = FindObjectsByType<BasicRoom>(FindObjectsSortMode.None);
            var arr2 = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None);
            foreach (var obj in arr)
            {
                Destroy(obj.gameObject);
            }
            foreach (var obj in arr2)
            {
                Destroy(obj.gameObject);
            }
            _rooms.Clear();
            Generate();
        }
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
        GenAtRoom(_startPos);
    }

    void GenAtRoom(Vector2Int pos)
    {
        Debug.Log(pos);
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

        bool[] possibilites = new bool[4];/*{ Cardinals.NORTH, Cardinals.SOUTH, Cardinals.EAST, Cardinals.WEST };*/

        bool done = Random.Range(0, 100) <= 10;
        if (done) return;

        //Check if OOB
        possibilites[(ushort)Cardinals.NORTH] = !((pos + GetOffset(Cardinals.NORTH)).x < 0);
        possibilites[(ushort)Cardinals.SOUTH] = !((pos + GetOffset(Cardinals.SOUTH)).x > _gridSize.x);
        possibilites[(ushort)Cardinals.EAST] = !((pos + GetOffset(Cardinals.EAST)).y < 0);
        possibilites[(ushort)Cardinals.WEST] = !((pos + GetOffset(Cardinals.WEST)).y > _gridSize.y);

        //For cardinals:
        for (ushort i = 0; i < 4; i++)
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
                    if (create)
                    {
                        GameObject lrGo = new GameObject("Connection");
                        LineRenderer lr = lrGo.AddComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, new Vector3(pos.x, pos.y, 0));
                        Vector2Int tmpVec = pos + GetOffset((Cardinals)i);
                        Vector3 tmpP = new Vector3(tmpVec.x, tmpVec.y, 0);
                        lr.SetPosition(1, tmpP);
                        lr.widthMultiplier = 0.1f;
                        GenAtRoom(pos + GetOffset((Cardinals)i));
                    }
                }
            }
        }
    }

}
