using CreativeSpore.SuperTilemapEditor;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single room in your dungeon A room has an (i,j) integer position and a (di, dj)
/// integer size in index coordinates
/// </summary>
public class Room : MonoBehaviour
{
    [field: SerializeField] public bool IsStartRoom { get; private set; }
    [field: SerializeField] public bool IsEndRoom { get; private set; }
    [field: SerializeField] public bool IsSecretRoom { get; private set; }
    [field: Space]
    [field: SerializeField] public int Difficulty { get; private set; }
    [field: SerializeField] public Element Element { get; private set; }
    [field: Space]
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public Vector2Int TotalTiledSize { get; private set; } = Vector2Int.one;
    [field: Space]
    [field: SerializeField] public Vector2Int Position_DoNotEdit { get; private set; } = Vector2Int.zero;

    private TilemapGroup _tilemapGroup;
    private List<Door> doors = null;
    private bool _isInitialized = false;
    public static List<Room> allRooms { get; private set; } = new List<Room>();

    /// <summary>
    /// Get a list of all doors in a room. Do not use at Awake.
    /// </summary>
    public List<Door> GetDoors()
    {
        if (doors == null)
        {
            RefreshDoors();
        }
        return doors;
    }

    /// <summary>
    /// Returns the min and max tiles positions of room in local-space
    /// </summary>
    public void GetLocalTileBounds(out Vector2Int outMin, out Vector2Int outMax)
    {
        outMin = Vector2Int.zero;
        outMax = Vector2Int.zero;
        foreach (STETilemap tilemap in _tilemapGroup.Tilemaps)
        {
            outMin.x = Mathf.Min(tilemap.MinGridX, outMin.x);
            outMin.y = Mathf.Min(tilemap.MinGridY, outMin.y);
            outMax.x = Mathf.Max(tilemap.MaxGridX, outMax.x);
            outMax.y = Mathf.Max(tilemap.MaxGridY, outMax.y);
        }
    }

    /// <summary>
    /// Returns the bounding box of room in local-space
    /// </summary>
    public Bounds GetLocalBounds()
    {
        Initialize();
        Bounds roomBounds = new Bounds(Vector3.zero, Vector3.zero);
        if (_tilemapGroup == null)
            return roomBounds;

        foreach (STETilemap tilemap in _tilemapGroup.Tilemaps)
        {
            Bounds bounds = tilemap.MapBounds;
            roomBounds.Encapsulate(bounds);
        }
        return roomBounds;
    }

    /// <summary>
    /// Returns the bounding box of room in world-space
    /// </summary>
    public Bounds GetWorldBounds()
    {
        Bounds result = GetLocalBounds();
        result.center += transform.position;
        return result;
    }

    /// <summary>
    /// Check if a world-space point is included in room's bounding box
    /// </summary>
    public bool Contains(Vector3 point)
    {
        point.z = 0;
        return (GetWorldBounds().Contains(point));
    }

    /// <summary>
    /// On enter room is called when starting game on starting room or when walking through a door.
    /// May be overloaded for any purpose.
    /// </summary>
    /// <param name="from">
    /// The room player comes from. "null" when first called for the starting room.
    /// </param>
    public virtual void OnEnterRoom(Room from)
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        Bounds cameraBounds = GetWorldBounds();
        cameraFollow.SetBounds(cameraBounds);
    }

    /// <summary>
    /// Get adjacent room for given orientation (North, south, east or west)
    /// </summary>
    public Room GetAdjacentRoom(Utils.ORIENTATION orientation, Vector3 from)
    {
        Vector2Int dir = Utils.OrientationToDir(orientation);
        Vector2Int adjacentPos = Position_DoNotEdit + dir + GetPositionOffset(from);
        Room adjacentRoom = Room.allRooms.Find(x =>
               adjacentPos.x >= x.Position_DoNotEdit.x
            && adjacentPos.y >= x.Position_DoNotEdit.y
            && adjacentPos.x < x.Position_DoNotEdit.x + x.Size.x
            && adjacentPos.y < x.Position_DoNotEdit.y + x.Size.y);
        return adjacentRoom;
    }

    /// <summary>
    /// Get door for given orientation (North, south, east or west) and a given world point (for
    /// room with size greater than one)
    /// </summary>
    public Door GetDoor(Utils.ORIENTATION orientation, Vector3 from)
    {
        Vector2Int doorPosition = Position_DoNotEdit + GetPositionOffset(from);
        List<Door> doors = GetDoors();
        foreach (Door door in doors)
        {
            if (doorPosition == Position_DoNotEdit + GetPositionOffset(door.transform.position)
                && door.Orientation == orientation)
            {
                return door;
            }
        }
        return null;
    }
    #region Internal

    private void Awake()
    {
        allRooms.Add(this);
        Initialize();
    }

    private void Start()
    {
        RefreshDoors();
        if (IsStartRoom)
        {
            //Player.Instance.EnterRoom(this);
        }
    }

    private void RefreshDoors()
    {
        if (doors == null)
        {
            doors = new List<Door>();
        }
        else
        {
            doors.Clear();
        }
        GetComponentsInChildren<Door>(true, doors);
    }

    /// <summary>
    /// Get position offset in index for a point in world coordinates inside rooms with a index size
    /// greater than {1,1}
    /// </summary>
    public Vector2Int GetPositionOffset(Vector3 worldPoint)
    {
        if (Size.x <= 1 && Size.y <= 1)
            return Vector2Int.zero;

        Vector2Int offset = Vector2Int.zero;
        Bounds bounds = GetWorldBounds();
        Vector3 localPoint = worldPoint - bounds.min;
        if (Size.x > 1)
        {
            offset.x = Mathf.Clamp((int)(localPoint.x / (bounds.size.x / Size.x)), 0, Size.x - 1);
        }
        if (Size.y > 1)
        {
            offset.y = Mathf.Clamp((int)(localPoint.y / (bounds.size.y / Size.y)), 0, Size.y - 1);
        }
        return offset;
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;
        _tilemapGroup = GetComponentInChildren<TilemapGroup>();
        foreach (STETilemap tilemap in _tilemapGroup.Tilemaps)
        {
            tilemap.RecalculateMapBounds();
        }
        _isInitialized = true;
    }

    private void OnDestroy()
    {
        allRooms.Remove(this);
    }
    #endregion Internal
}