using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 알만툴 스타일의 이벤트 플래그 중앙 관리 시스템
/// </summary>
public class GameFlags : MonoBehaviour
{
    private static GameFlags instance;
    private Dictionary<string, string> flags = new Dictionary<string, string>();

    public static GameFlags Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameFlags");
                instance = go.AddComponent<GameFlags>();
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
    /// 플래그 설정 (bool 값)
    /// </summary>
    public static void Set(string flagName, bool value)
    {
        Instance.flags[flagName] = value.ToString();
    }

    /// <summary>
    /// 플래그 설정 (문자열 값)
    /// </summary>
    public static void Set(string flagName, string value)
    {
        Instance.flags[flagName] = value;
    }

    /// <summary>
    /// 플래그 설정 (숫자 값)
    /// </summary>
    public static void Set(string flagName, int value)
    {
        Instance.flags[flagName] = value.ToString();
    }

    /// <summary>
    /// 플래그 설정 (실수 값)
    /// </summary>
    public static void Set(string flagName, float value)
    {
        Instance.flags[flagName] = value.ToString();
    }

    /// <summary>
    /// 플래그 값 가져오기 (bool, 기본값: false)
    /// </summary>
    public static bool GetBool(string flagName, bool defaultValue = false)
    {
        if (Instance.flags.ContainsKey(flagName))
        {
            if (bool.TryParse(Instance.flags[flagName], out bool result))
                return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 플래그 값 가져오기 (문자열, 기본값: "")
    /// </summary>
    public static string GetString(string flagName, string defaultValue = "")
    {
        if (Instance.flags.ContainsKey(flagName))
            return Instance.flags[flagName];
        return defaultValue;
    }

    /// <summary>
    /// 플래그 값 가져오기 (int, 기본값: 0)
    /// </summary>
    public static int GetInt(string flagName, int defaultValue = 0)
    {
        if (Instance.flags.ContainsKey(flagName))
        {
            if (int.TryParse(Instance.flags[flagName], out int result))
                return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 플래그 값 가져오기 (float, 기본값: 0f)
    /// </summary>
    public static float GetFloat(string flagName, float defaultValue = 0f)
    {
        if (Instance.flags.ContainsKey(flagName))
        {
            if (float.TryParse(Instance.flags[flagName], out float result))
                return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 플래그가 존재하는지 확인
    /// </summary>
    public static bool Has(string flagName)
    {
        return Instance.flags.ContainsKey(flagName);
    }

    /// <summary>
    /// 플래그 삭제
    /// </summary>
    public static void Remove(string flagName)
    {
        if (Instance.flags.ContainsKey(flagName))
            Instance.flags.Remove(flagName);
    }

    /// <summary>
    /// 모든 플래그 초기화
    /// </summary>
    public static void Clear()
    {
        Instance.flags.Clear();
    }

    /// <summary>
    /// 현재 플래그들을 Dictionary로 반환 (저장용)
    /// </summary>
    public Dictionary<string, string> GetAllFlags()
    {
        return new Dictionary<string, string>(flags);
    }

    /// <summary>
    /// Dictionary로부터 플래그 복원 (로드용)
    /// </summary>
    public void RestoreFlags(Dictionary<string, string> savedFlags)
    {
        flags.Clear();
        if (savedFlags != null)
        {
            foreach (var kvp in savedFlags)
            {
                flags[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// 현재 플래그 수
    /// </summary>
    public static int Count => Instance.flags.Count;
}


