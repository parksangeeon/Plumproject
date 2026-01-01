using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int slotIndex;
    public string sceneName;
    public Vector3 playerPosition;
    public List<string> inventoryItems; // 아이템 이름 리스트
    public DateTime saveTime;
    public string saveTimeString; // UI 표시용
    public bool isEmpty = true;
    
    // 이벤트 플래그 데이터 (대화 완료, 이벤트 플래그, 퍼즐 완료 등)
    public SerializableDictionary gameFlags = new SerializableDictionary();
    
    // 썸네일은 Base64로 저장 (선택사항)
    public string thumbnailBase64;

    public SaveData()
    {
        inventoryItems = new List<string>();
        gameFlags = new SerializableDictionary();
        isEmpty = true;
    }

    public SaveData(int slot, string scene, Vector3 pos, List<string> items, SerializableDictionary flags = null)
    {
        slotIndex = slot;
        sceneName = scene;
        playerPosition = pos;
        inventoryItems = items ?? new List<string>();
        gameFlags = flags ?? new SerializableDictionary();
        saveTime = DateTime.Now;
        saveTimeString = saveTime.ToString("yyyy-MM-dd HH:mm:ss");
        isEmpty = false;
    }

    // SaveGameLoader.cs 호환성을 위한 프로퍼티
    public float px
    {
        get { return playerPosition.x; }
        set { playerPosition.x = value; }
    }

    public float py
    {
        get { return playerPosition.y; }
        set { playerPosition.y = value; }
    }
}


