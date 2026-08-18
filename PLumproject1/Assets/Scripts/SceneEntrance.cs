using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum SceneEntrance
{
    None          = -1,
    Hall1_Room1_1  =  0,  // 1층 홀 ↔ 1-1
    Hall1_Room1_2  =  1,  // 1층 홀 ↔ 1-2
    Hall1_Hall2    =  2,  // 1층 홀 ↔ 2층 홀
    Prologue_Hall1 =  3,  // 프롤로그 ↔ 1-hall
    // 새 연결 추가 시 enum 항목 + connections 딕셔너리에 한 줄 추가
}

public static class SceneEntranceHelper
{
    private static readonly Dictionary<SceneEntrance, (string, string)> connections = new()
    {
        { SceneEntrance.Hall1_Room1_1,  ("1-hall",   "1-1")    },
        { SceneEntrance.Hall1_Room1_2,  ("1-hall",   "1-2")    },
        { SceneEntrance.Hall1_Hall2,    ("1-hall",   "2-hall") },
        { SceneEntrance.Prologue_Hall1, ("Prologue", "1-hall") },
    };

    public static string GetTargetScene(SceneEntrance entrance)
    {
        if (!connections.TryGetValue(entrance, out var pair)) return null;
        string current = SceneManager.GetActiveScene().name;
        return current == pair.Item1 ? pair.Item2 : pair.Item1;
    }
}
