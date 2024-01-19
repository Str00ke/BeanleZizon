using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveCollapseMaster : MonoBehaviour
{

    [SerializeField] int _mapSize; //Square for now
    [SerializeField] GameObject _baseRoomPrefab; 

    int[] totalEntropyMap;

    int GetEntropyAsVec2D(int x, int y) => totalEntropyMap[_mapSize * x + y];


    void Start()
    {
        totalEntropyMap = new int[_mapSize * _mapSize];
        for (int i = 0; i < totalEntropyMap.Length; i++) totalEntropyMap[i] = 35;

        Init();
    }

    void Init()
    {
        for (int x = 0; x < _mapSize; ++x)
        {
            for (int y = 0; y < _mapSize; ++y)
            {
                GameObject room = Instantiate(_baseRoomPrefab, new Vector3(x, y, 0), Quaternion.identity);
                WCBaseRoom baseRoom = room.AddComponent<WCBaseRoom>();
                baseRoom.InitEmpty(new Vector2Int(x, y));
            }
        }
    }


    void Update()
    {
        
    }
}
