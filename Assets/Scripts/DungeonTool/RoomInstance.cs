using UnityEngine;

internal sealed class RoomInstance : MonoBehaviour
{
    [field: SerializeField] public RoomData RoomData { get; private set; }
}