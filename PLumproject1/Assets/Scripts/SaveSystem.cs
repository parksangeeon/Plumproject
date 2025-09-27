using System;
using System.Collections;               // ★ 추가
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ClearSky;

public static class SaveSystem
{
    public const int SlotCount = 4;

    static string Root => Path.Combine(Application.persistentDataPath, "saves");
    static string SlotPath(int slot) => Path.Combine(Root, $"slot{slot}.json");

    public static bool Exists(int slot) => File.Exists(SlotPath(slot));

    public static SaveSummary GetSummary(int slot)
    {
        if (!Exists(slot)) return null;
        try
        {
            var json = File.ReadAllText(SlotPath(slot));
            var data = JsonUtility.FromJson<SaveData>(json);
            return new SaveSummary
            {
                sceneName = data.sceneName,
                timestamp = data.timestamp,
                itemCount = data.itemIds?.Count ?? 0
            };
        }
        catch { return null; }
    }

    public static void Save(int slot, Player player, Func<List<string>> exportItems)
    {
        if (!Directory.Exists(Root)) Directory.CreateDirectory(Root);

        var data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playSeconds = Time.realtimeSinceStartup,
            px = player.transform.position.x,
            py = player.transform.position.y,
            pz = player.transform.position.z,
            goingPointName = player.GoingPointName,
            itemIds = exportItems != null ? exportItems() : new List<string>()
        };

        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SlotPath(slot), json);
        Debug.Log($"[SaveSystem] Saved to slot {slot}: {SlotPath(slot)}");
    }

    public static IEnumerator Load(int slot, Action onFail = null)
    {
        if (!Exists(slot)) { onFail?.Invoke(); yield break; }

        SaveData data;
        try
        {
            var json = File.ReadAllText(SlotPath(slot));
            data = JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Load parse error: {e}");
            onFail?.Invoke();
            yield break;
        }

        // 씬 로드
        Time.timeScale = 1f;
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone) yield return null;

        // 오브젝트 찾기
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        var bridge = UnityEngine.Object.FindObjectOfType<SaveLoadBridge>();
        if (player == null)
        {
            Debug.LogError("[SaveSystem] Player not found after scene load");
            yield break;
        }

        // 위치/상태 복원
        player.GoingPointName = string.IsNullOrEmpty(data.goingPointName) ? null : data.goingPointName;
        player.transform.position = new Vector3(data.px, data.py, data.pz);

        // 인벤토리 복원
        if (bridge != null) bridge.ImportItems(data.itemIds);
        else Debug.LogWarning("[SaveSystem] SaveLoadBridge not found - inventory not restored");

        Debug.Log($"[SaveSystem] Loaded slot {slot} ({data.sceneName})");
    }
}
