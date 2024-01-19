using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCBaseRoom : MonoBehaviour
{
    Vector2Int _position;

    public void InitEmpty(Vector2Int pos)
    {
        _position = pos;
        new WCEmpty(35, pos, this);
    }
}
