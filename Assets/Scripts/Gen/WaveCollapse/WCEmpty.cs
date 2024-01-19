using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class WCEmpty
{
    int _currEntropy = 0;

    TextMeshPro _tmp;

    WCBaseRoom _room;

    public WCEmpty(int entropy, Vector2Int pos, WCBaseRoom room)
    {
        _currEntropy = entropy;
        _room = room;
        _room.gameObject.name = pos.x.ToString() + "_" + pos.y.ToString();
        GameObject goCan = new GameObject(_room.name + "_Canvas");
        goCan.transform.parent = _room.transform;
        RectTransform canRT = goCan.AddComponent<RectTransform>();
        Canvas canvas = goCan.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canRT.position = new Vector2(pos.x, pos.y);
        canRT.sizeDelta = new Vector2(1, 1);
        _tmp = goCan.AddComponent<TextMeshPro>();
        _tmp.text = _currEntropy.ToString();
        _tmp.fontSize = 5;
        _tmp.alignment = TextAlignmentOptions.CenterGeoAligned;
    }

    public void SetEntropy(int newEntropy)
    {
        _currEntropy = newEntropy;
        _tmp.text = _currEntropy.ToString();
    }
}
