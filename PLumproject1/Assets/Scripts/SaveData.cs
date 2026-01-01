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
    
    // 썸네일은 Base64로 저장 (선택사항)
    public string thumbnailBase64;

    public SaveData()
    {
        inventoryItems = new List<string>();
        isEmpty = true;
    }

    public SaveData(int slot, string scene, Vector3 pos, List<string> items)
    {
        slotIndex = slot;
        sceneName = scene;
        playerPosition = pos;
        inventoryItems = items ?? new List<string>();
        saveTime = DateTime.Now;
        saveTimeString = saveTime.ToString("yyyy-MM-dd HH:mm:ss");
        isEmpty = false;
    }
}


