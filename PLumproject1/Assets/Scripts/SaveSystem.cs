using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    private const string SAVE_DIRECTORY = "/SaveData/";
    private const int MAX_SLOTS = 4;
    
    private static SaveSystem instance;
    
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SaveSystem");
                instance = go.AddComponent<SaveSystem>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // 저장 디렉토리 생성
        string savePath = Application.persistentDataPath + SAVE_DIRECTORY;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    public SaveData LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return null;
        }

        string filePath = GetSaveFilePath(slotIndex);
        
        if (!File.Exists(filePath))
        {
            // 빈 슬롯은 정상이므로 로그 출력하지 않음
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            if (saveData.isEmpty)
            {
                return null;
            }

            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 데이터를 불러오는 중 오류 발생: {e.Message}");
            return null;
        }
    }

    public bool HasSaveData(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            return saveData != null && !saveData.isEmpty;
        }
        catch
        {
            return false;
        }
    }

    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + SAVE_DIRECTORY + $"save_{slotIndex}.json";
    }

    public void DeleteSave(int slotIndex)
    {
        string filePath = GetSaveFilePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"슬롯 {slotIndex + 1}의 저장 데이터가 삭제되었습니다.");
        }
    }

    public static int SlotCount => MAX_SLOTS;

    public static SaveSummary GetSummary(int slotIndex)
    {
        if (Instance == null) return null;
        SaveData data = Instance.LoadGame(slotIndex);
        if (data == null || data.isEmpty) return null;
        return new SaveSummary
        {
            sceneName = data.sceneName,
            saveTime = data.saveTimeString,
            isEmpty = false
        };
    }

    public static bool Exists(int slotIndex)
    {
        return Instance != null && Instance.HasSaveData(slotIndex);
    }

    public static void Save(int slotIndex, ClearSky.Player player, System.Func<List<string>> exportItems = null)
    {
        if (Instance == null) return;

        // 플레이어 위치
        Vector3 playerPos = player.transform.position;

        // 현재 씬 이름
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // 인벤토리 정보
        List<string> itemNames = new List<string>();
        if (exportItems != null)
        {
            itemNames = exportItems();
        }
        else if (player.inventory != null)
        {
            itemNames = player.inventory.GetItemIds();
        }

        // 게임 플래그 수집
        Dictionary<string, string> flags = GameFlags.Instance.GetAllFlags();
        SerializableDictionary gameFlags = SerializableDictionary.FromDictionary(flags);

        // 저장 데이터 생성
        SaveData saveData = new SaveData(slotIndex, currentScene, playerPos, itemNames, gameFlags);

        // JSON으로 변환
        string json = JsonUtility.ToJson(saveData, true);

        // 파일로 저장
        string filePath = Instance.GetSaveFilePath(slotIndex);
        File.WriteAllText(filePath, json);

        Debug.Log($"게임이 슬롯 {slotIndex + 1}에 저장되었습니다: {filePath}");
    }

    public static System.Collections.IEnumerator Load(int slotIndex)
    {
        if (Instance == null) yield break;

        SaveData saveData = Instance.LoadGame(slotIndex);
        if (saveData == null || saveData.isEmpty)
        {
            Debug.LogError($"슬롯 {slotIndex + 1}에서 게임을 불러올 수 없습니다.");
            yield break;
        }

        SaveGameLoader.Load(saveData);
        yield return null;
    }
}

public class SaveSummary
{
    public string sceneName;
    public string saveTime;
    public bool isEmpty;
}

