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

    private Dictionary<Vector2Int, Room> _rooms = new();
 
    private Vector2Int _startPos = Vector2Int.zero;

    public Room room(Vector2Int value)
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
        bool canGen = true;
        Vector2Int currPos = _startPos;
        bool[] possibilites = new bool[4];/*{ Cardinals.NORTH, Cardinals.SOUTH, Cardinals.EAST, Cardinals.WEST };*/
        while(canGen) 
        {
            _rooms[_startPos] = new Room();

            //Check if OOB
            possibilites[(ushort)Cardinals.NORTH] = !((currPos.x - 1) < 0);
            possibilites[(ushort)Cardinals.SOUTH] = !((currPos.x + 1) > _gridSize.x);
            possibilites[(ushort)Cardinals.EAST]  = !((currPos.y - 1) < 0);
            possibilites[(ushort)Cardinals.WEST]  = !((currPos.y + 1) > _gridSize.y);

            //For cardinals:
            foreach(bool card in possibilites)
            {
                if (card)
                {
                    //Check if room exist
                    if(room)
                }
            }
            //l> if yes, rand connect
            //l> else, rand create
        }
    }
}
