using System.Collections;
using System.Collections.Generic;
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

        // 게임 플래그를 먼저 복원 (오브젝트 Start() 전에 복원되도록)
        if (data.gameFlags != null)
        {
            Dictionary<string, string> flags = data.gameFlags.ToDictionary();
            GameFlags.Instance.RestoreFlags(flags);
        }

        // MonologueManager의 "이미 본 대사" 기록은 GameFlags와 별개로 메모리에 남아있어서,
        // 더 이전 상태로 로드해도 이번 세션에서 한 번 본 progress는 다시 안 재생됨 - 같이 초기화
        var monologueManager = FindAnyObjectByType<MonologueManager>();
        if (monologueManager != null) monologueManager.ResetSeenEvents();

        // StartingPoint가 1프레임 딜레이로 위치를 만지므로, 2프레임 뒤에 강제 적용
        yield return null;
        yield return null;

        var player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            // 스폰 포인트 이동 로직과 충돌 방지
            player.pendingEntrance = SceneEntrance.None;
            player.transform.position = new Vector3(data.px, data.py, 0f);
            
            // 인벤토리 복원 (플래그 확인 후 필터링)
            var bridge = FindAnyObjectByType<SaveLoadBridge>();
            if (bridge != null && data.inventoryItems != null)
            {
                // 플래그에 따라 인벤토리 아이템 필터링
                List<string> filteredItems = new List<string>(data.inventoryItems);
                bool shouldHaveLantern = GameFlags.GetBool("lantern_picked_up", false);

                if (!shouldHaveLantern)
                {
                    // 플래그가 false면 랜턴 제거 (모든 랜턴 제거)
                    filteredItems.RemoveAll(id => id != null && id == "Lantern");
                }

                bridge.ImportItems(filteredItems);
            }
            
            // 인벤토리 복원 후 한 번 더 확인 (안전장치)
            yield return null;
            var inventory = player.inventory;
            if (inventory != null)
            {
                bool shouldHaveLantern = GameFlags.GetBool("lantern_picked_up", false);
                if (!shouldHaveLantern)
                {
                    // 플래그가 false인데 인벤토리에 랜턴이 있으면 모두 제거
                    // GetAllItems로 복사본 만들고 순회하면서 제거
                    var allItemsCopy = new List<IInventoryItem>(inventory.GetAllItems());
                    List<IInventoryItem> lanternsToRemove = new List<IInventoryItem>();
                    
                    // 먼저 제거할 랜턴들을 리스트에 수집 (이름이 "Lantern"인 모든 아이템)
                    foreach (var item in allItemsCopy)
                    {
                        if (item != null && item.Name == "Lantern")
                        {
                            lanternsToRemove.Add(item);
                        }
                    }
                    
                    // 수집한 모든 랜턴 제거
                    foreach (var lantern in lanternsToRemove)
                    {
                        MonoBehaviour mb = lantern as MonoBehaviour;
                        if (mb != null && mb.gameObject != null)
                        {
                            inventory.RemoveItem(lantern);
                            Destroy(mb.gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Load: Player를 못찾았습니다.");
        }

        // 플래그 복원 후 씬의 모든 오브젝트를 다시 초기화하도록 메시지 전송
        yield return null; // 한 프레임 더 대기 (모든 Start()가 호출된 후)
        
        // 월드 오브젝트 상태 복원 (인벤토리 동기화 후)
        yield return null; // 한 프레임 더 대기
        
        // LanternPickup 오브젝트 직접 찾아서 복원 (비활성화된 것도 포함)
        LanternPickup[] lanternPickups = FindObjectsOfType<LanternPickup>(true);
        foreach (LanternPickup pickup in lanternPickups)
        {
            if (pickup != null)
            {
                // 직접 메서드 호출 (public으로 변경했으므로)
                pickup.OnLoadGame();
            }
        }
        
        // 다른 오브젝트들도 복원
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>(true);
        foreach (MonoBehaviour mb in allMonoBehaviours)
        {
            if (mb != null && mb.gameObject != null && !(mb is LanternPickup))
            {
                mb.SendMessage("OnLoadGame", SendMessageOptions.DontRequireReceiver);
            }
        }

        pending = null;
    }
}
