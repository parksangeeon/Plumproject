using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 있는 모든 ISaveable 오브젝트를 찾아서 상태를 수집하고 복원하는 매니저
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameStateManager");
                instance = go.AddComponent<GameStateManager>();
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
    }

    /// <summary>
    /// 현재 씬의 모든 ISaveable 오브젝트에서 상태를 수집
    /// </summary>
    public SerializableDictionary CaptureGameState()
    {
        SerializableDictionary state = new SerializableDictionary();
        
        // 씬에 있는 모든 ISaveable 컴포넌트 찾기
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>() as ISaveable[];
        if (saveables == null || saveables.Length == 0)
        {
            // MonoBehaviour를 직접 확인
            MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in allMonoBehaviours)
            {
                if (mb is ISaveable saveable)
                {
                    string id = saveable.GetSaveID();
                    Dictionary<string, string> objState = saveable.CaptureState();
                    
                    // 각 상태를 "ID.key" 형식으로 저장
                    foreach (var kvp in objState)
                    {
                        state.Add($"{id}.{kvp.Key}", kvp.Value);
                    }
                }
            }
        }
        else
        {
            foreach (var saveable in saveables)
            {
                if (saveable != null)
                {
                    string id = saveable.GetSaveID();
                    Dictionary<string, string> objState = saveable.CaptureState();
                    
                    // 각 상태를 "ID.key" 형식으로 저장
                    foreach (var kvp in objState)
                    {
                        state.Add($"{id}.{kvp.Key}", kvp.Value);
                    }
                }
            }
        }

        return state;
    }

    /// <summary>
    /// 저장된 상태를 모든 ISaveable 오브젝트에 복원
    /// </summary>
    public void RestoreGameState(SerializableDictionary savedState)
    {
        if (savedState == null || savedState.items == null || savedState.items.Count == 0)
        {
            return;
        }

        Dictionary<string, string> stateDict = savedState.ToDictionary();

        // 씬에 있는 모든 ISaveable 컴포넌트 찾기
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        Dictionary<string, Dictionary<string, string>> groupedState = new Dictionary<string, Dictionary<string, string>>();

        // 상태를 ID별로 그룹화
        foreach (var kvp in stateDict)
        {
            string[] parts = kvp.Key.Split(new char[] { '.' }, 2);
            if (parts.Length == 2)
            {
                string id = parts[0];
                string key = parts[1];

                if (!groupedState.ContainsKey(id))
                {
                    groupedState[id] = new Dictionary<string, string>();
                }
                groupedState[id][key] = kvp.Value;
            }
        }

        // 각 ISaveable에 상태 복원
        foreach (var mb in allMonoBehaviours)
        {
            if (mb is ISaveable saveable)
            {
                string id = saveable.GetSaveID();
                if (groupedState.ContainsKey(id))
                {
                    saveable.RestoreState(groupedState[id]);
                }
            }
        }
    }
}



