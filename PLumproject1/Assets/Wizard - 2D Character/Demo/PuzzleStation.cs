using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuzzleStation : MonoBehaviour
{
    [Header("Refs")]
    public RhythmPuzzleManager puzzle;       // 퍼즐 매니저(퍼즐 패널 오브젝트)
    public RectTransform inventoryPanel;     // HUD 인벤토리 패널 (있으면 퍼즐 시작 시 숨김)
    public EnemiesController monster;        // ← 몬스터(EnemiesController가 붙은 오브젝트)

    [Header("Player")]
    public string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        // 혹시 몬스터 참조 안 넣었으면 자동으로 하나 찾아줌(씬에 하나만 있다고 가정)
        if (monster == null) monster = FindObjectOfType<EnemiesController>(true);
        if (puzzle == null) puzzle = FindObjectOfType<RhythmPuzzleManager>(true);
    }

    private void OnEnable()
    {
        // 퍼즐이 닫힐 때(성공/실패/ESC) HUD 복구 + 몬스터 리셋
        if (puzzle != null)
        {
            puzzle.onClosed.AddListener(ReenableHUD);
            puzzle.onClosed.AddListener(ResetMonster);
        }
    }

    private void OnDisable()
    {
        if (puzzle != null)
        {
            puzzle.onClosed.RemoveListener(ReenableHUD);
            puzzle.onClosed.RemoveListener(ResetMonster);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // HUD 숨김
            if (inventoryPanel != null) inventoryPanel.gameObject.SetActive(false);

            // ★ 퍼즐 시작과 동시에 몬스터 출발(직접 호출)
            if (monster != null)
            {
                monster.StartChaseLeft();
                Debug.Log("[Station] StartChaseLeft() called on monster");
            }
            else
            {
                Debug.LogWarning("[Station] monster reference is NULL");
            }

            // 퍼즐 시작
            if (puzzle != null)
            {
                puzzle.StartPuzzle();
            }
            else
            {
                Debug.LogError("[Station] puzzle reference is NULL");
            }
        }
    }

    private void ReenableHUD()
    {
        if (inventoryPanel != null) inventoryPanel.gameObject.SetActive(true);
    }

    private void ResetMonster()
    {
        if (monster != null)
        {
            monster.ResetToDefault();
            Debug.Log("[Station] ResetToDefault() called on monster");
        }
    }
}
