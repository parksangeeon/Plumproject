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

    public void SaveGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        // 플레이어 정보 가져오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // 인벤토리 정보 가져오기
        Inventory inventory = player.GetComponent<ClearSky.Player>()?.inventory;
        List<string> itemNames = new List<string>();
        
        if (inventory != null)
        {
            itemNames = inventory.GetItemIds();
        }

        // 현재 씬 이름
        string currentScene = SceneManager.GetActiveScene().name;

        // 플레이어 위치
        Vector3 playerPos = player.transform.position;

        // 게임 플래그 수집
        Dictionary<string, string> flags = GameFlags.Instance.GetAllFlags();
        SerializableDictionary gameFlags = SerializableDictionary.FromDictionary(flags);

        // 저장 데이터 생성
        SaveData saveData = new SaveData(slotIndex, currentScene, playerPos, itemNames, gameFlags);

        // JSON으로 변환
        string json = JsonUtility.ToJson(saveData, true);
        
        // 파일로 저장
        string filePath = GetSaveFilePath(slotIndex);
        File.WriteAllText(filePath, json);

        Debug.Log($"게임이 슬롯 {slotIndex + 1}에 저장되었습니다: {filePath}");
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

    public void ApplySaveData(SaveData saveData)
    {
        if (saveData == null || saveData.isEmpty)
        {
            Debug.LogError("유효하지 않은 저장 데이터입니다.");
            return;
        }

        // 일시정지 해제
        Time.timeScale = 1f;

        // 씬 로드
        SceneManager.LoadScene(saveData.sceneName);

        // 씬 로드 후 플레이어 위치 복원 (씬 로드가 완료된 후 실행되어야 함)
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == saveData.sceneName)
            {
                // 플레이어 위치 복원 (약간의 지연을 두어 씬이 완전히 로드된 후 실행)
                StartCoroutine(RestorePlayerData(saveData));

                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
    }

    private System.Collections.IEnumerator RestorePlayerData(SaveData saveData)
    {
        // 씬 로드가 완전히 완료될 때까지 대기
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        // 플레이어 위치 복원
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerPosition;
        }

        // 인벤토리 복원
        Inventory inventory = player?.GetComponent<ClearSky.Player>()?.inventory;
        if (inventory != null)
        {
            // 인벤토리 아이템 복원 로직 (추후 구현)
            // inventory.RestoreItems(saveData.inventoryItems);
        }
    }

    public SaveData GetSaveData(int slotIndex)
    {
        return LoadGame(slotIndex);
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

    // SaveLoadPanel.cs 호환성을 위한 정적 메서드들
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

// SaveLoadPanel.cs 호환성을 위한 클래스
public class SaveSummary
{
    public string sceneName;
    public string saveTime;
    public bool isEmpty;
}

