using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ClearSky;

public class SaveGameLoader : MonoBehaviour
{
    static SaveGameLoader _instance;
    SaveData pending;

    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Load(SaveData data)
    {
        if (data == null) { Debug.LogWarning("불러오기 데이터가 없습니다."); return; }
        if (_instance == null)
        {
            var go = new GameObject("~SaveGameLoader");
            _instance = go.AddComponent<SaveGameLoader>();
            DontDestroyOnLoad(go);
        }
        _instance.StartCoroutine(_instance.CoLoad(data));
    }

    IEnumerator CoLoad(SaveData data)
    {
        // 퍼즈 해제 후 로드
        Time.timeScale = 1f;
        pending = data;

        var current = SceneManager.GetActiveScene().name;
        if (current != data.sceneName)
        {
            // 씬 이동
            bool done = false;
            SceneManager.sceneLoaded += (s, m) => { done = true; };
            SceneManager.LoadScene(data.sceneName);
            yield return new WaitUntil(() => done);
        }

        // StartingPoint가 1프레임 딜레이로 위치를 만지므로, 2프레임 뒤에 강제 적용
        yield return null;
        yield return null;

        var player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            // 스폰 포인트 이동 로직과 충돌 방지
            player.GoingPointName = string.Empty;
            player.transform.position = new Vector3(data.px, data.py, 0f);
        }
        else
        {
            Debug.LogWarning("Load: Player를 못찾았습니다.");
        }
        pending = null;
    }
}
