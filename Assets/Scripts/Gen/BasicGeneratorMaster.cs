using System.Collections;
using System.Collections.Generic;
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
    private Vector2Int _gridSize;

    private Dictionary<Vector2Int, BasicRoom> _rooms = new();
 
    private Vector2Int _startPos = Vector2Int.zero;

    public BasicRoom room(Vector2Int value)
    {
        return _rooms[value];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        _rooms[_startPos] = new BasicRoom();

        GenAtRoom(_startPos);
    }

    void GenAtRoom(Vector2Int pos)
    {
        _rooms[pos] = new BasicRoom();
        GameObject tmp = new GameObject("testRoom");
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
                if (room(pos + GetOffset((Cardinals)i)))
                {
                    //l> if yes, rand connect
                    bool connect = Random.Range(0, 100) > 50;
                    //TODO
                }
                else
                {
                    //l> else, rand create
                    bool create = Random.Range(0, 100) > 50;
                    if (create) GenAtRoom(pos + GetOffset((Cardinals)i));
                }
            }
        }
    }

}
