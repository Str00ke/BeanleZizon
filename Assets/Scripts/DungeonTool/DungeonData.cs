using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
internal sealed class DungeonData : ScriptableObject
{
    [field: Tooltip("Number of rooms")]
    [field: SerializeField] public int DungeonLength { get; private set; } = 10;

    [field: Space]
    [field: Space]
    [field: Header("GENERATED (DO NOT EDIT)")]
    [field: Space]
    [field: SerializeField] public RoomData StartRoom { get; private set; }
    [field: SerializeField] public RoomData EndRoom { get; private set; }
    [field: SerializeField] public RoomData SecretRoom { get; private set; }
    [field: Space]
    [field: SerializeField] public List<RoomData> Rooms { get; private set; }
}