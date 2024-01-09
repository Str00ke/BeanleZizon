using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal sealed class RoomData
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public GameObject Prefab { get; set; }

    [field: SerializeField] public List<KeyCollectible> Keys { get; set; }
    [field: SerializeField] public List<HeartCollectible> Hearts { get; set; }
    [field: SerializeField] public List<Door> Doors { get; set; }
}