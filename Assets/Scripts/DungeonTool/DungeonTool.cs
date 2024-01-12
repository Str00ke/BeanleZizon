using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class DungeonTool : MonoBehaviour
{
    private const string ROOM_DIR = "Assets/Prefabs/Rooms";
    private const string DUNGEON_DATA_PATH = "Assets/ScriptableObjects/DungeonData.asset";

    private static readonly DungeonData dungeonData;
    private static DungeonData DungeonData => dungeonData != null ? dungeonData : AssetDatabase.LoadAssetAtPath<DungeonData>(DUNGEON_DATA_PATH);

    private static List<RoomData> Rooms => DungeonData.Rooms;

    [MenuItem(nameof(DungeonTool) + "/" + nameof(BakeDungeonData))]
    public static void BakeDungeonData()
    {
        Rooms.Clear();

        foreach (var guid in AssetDatabase.FindAssets("t:prefab", new string[] { ROOM_DIR }))
        {
            var room = AssetDatabase.LoadAssetAtPath<Room>(AssetDatabase.GUIDToAssetPath(guid));

            var keys = room.GetComponentsInChildren<KeyCollectible>();
            var hearts = room.GetComponentsInChildren<HeartCollectible>();
            var doors = room.GetComponentsInChildren<Door>();

            foreach (var door in doors)
            {
                door.Initialize(room);
            }

            var roomData = new RoomData
            {
                Name = room.name,
                Prefab = room.gameObject,
                Keys = keys.ToList(),
                Hearts = hearts.ToList(),
                Doors = doors.ToList(),
            };

            Rooms.Add(roomData);
        }
    }

    [MenuItem(nameof(DungeonTool) + "/" + nameof(ClearDungeonData))]
    public static void ClearDungeonData()
    {
        Rooms.Clear();
    }
}