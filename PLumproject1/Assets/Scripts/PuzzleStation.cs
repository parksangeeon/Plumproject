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

    private bool isPlayerInZone = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (monster == null) monster = FindObjectOfType<EnemiesController>(true);
        if (puzzle == null) puzzle = FindObjectOfType<RhythmPuzzleManager>(true);
    }

    private void OnEnable()
    {
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

    private void Update()
    {
        if (!isPlayerInZone) return;
        if (puzzle != null && puzzle.InProgress) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (inventoryPanel != null) inventoryPanel.gameObject.SetActive(false);

            if (monster != null)
            {
                monster.StartChaseLeft();
                Debug.Log("[Station] StartChaseLeft() called on monster");
            }
            else
            {
                Debug.LogWarning("[Station] monster reference is NULL");
            }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) isPlayerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) isPlayerInZone = false;
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
            //if (HUD != null) HUD.gameObject.SetActive(false);
            //puzzle?.StartPuzzle();

        }
    }
}
