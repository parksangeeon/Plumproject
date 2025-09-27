using UnityEngine;
using UnityEngine.Events;

public class RhythmPuzzleManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform playArea;      // 노트 영역
    public GameObject notePrefab;       // RhythmNote 프리팹

    [Header("Puzzle Settings")]
    public int noteCount = 4;
    public float approachTime = 1.2f;
    public float hitWindow = 0.20f;
    public float startDelay = 0.5f;

    [Header("Control")]
    public bool allowEscape = true;

    [Header("Events (optional)")]
    public UnityEvent puzzleClearedEvent;   // 성공 시만
    public UnityEvent onOpened;             // 퍼즐 열릴 때
    public UnityEvent onClosed;             // 퍼즐 닫힐 때(성공/실패/ESC)

    [Header("Monster (direct binding)")]
    public EnemiesController enemy;         // ← 여기에 몬스터 드래그

    // 내부 상태
    private bool inProgress = false;
    private int nextIndex = 0;
    private RhythmNote currentNote = null;
    private float scheduledTime = 0f;

    // 외부 참조용
    public bool InProgress => inProgress;
    public float Now => Time.unscaledTime;
    public float HitWindow => hitWindow;
    public float ApproachTime => approachTime;
    public int NextIndex => nextIndex;
    public float ScheduledTime => scheduledTime;

    public void StartPuzzle()
    {
        if (inProgress) return;

        if (playArea == null || notePrefab == null)
        {
            Debug.LogError("[RhythmPuzzle] playArea/notePrefab 미할당");
            return;
        }

        gameObject.SetActive(true);
        ClearSky.Player.isControlBlocked = true;

        inProgress = true;
        nextIndex = 0;
        DestroyCurrentNote();

        // 첫 노트 준비
        SpawnNext(startDelay);

        // 이벤트/몬스터 호출(둘 다 넣음)
        onOpened?.Invoke();

        if (enemy != null)
        {
            Debug.Log($"[Puzzle] enemy bound = '{enemy.name}' → StartChaseLeft()");
            enemy.StartChaseLeft();
        }
        else
        {
            Debug.LogWarning("[Puzzle] enemy reference is NULL — RhythmPuzzleManager.enemy에 몬스터를 드래그하세요.");
        }

        Debug.Log("[RhythmPuzzle] Start");
    }

    public void ClosePuzzle()
    {
        DestroyCurrentNote();
        inProgress = false;

        gameObject.SetActive(false);
        ClearSky.Player.isControlBlocked = false;

        // 몬스터 리셋(직결) + 이벤트
        if (enemy != null)
        {
            Debug.Log($"[Puzzle] enemy bound = '{enemy.name}' → ResetToDefault()");
            enemy.ResetToDefault();
        }
        onClosed?.Invoke();

        Debug.Log("[RhythmPuzzle] Closed");
    }

    public void ForceClose() => ClosePuzzle();

    private void DestroyCurrentNote()
    {
        if (currentNote != null)
        {
            Destroy(currentNote.gameObject);
            currentNote = null;
        }
    }

    private void SpawnNext(float extraDelay)
    {
        DestroyCurrentNote();

        if (nextIndex >= noteCount)
        {
            Debug.Log("[RhythmPuzzle] Puzzle Clear!");
            puzzleClearedEvent?.Invoke();
            ClosePuzzle();
            return;
        }

        GameObject go = Instantiate(notePrefab, playArea);
        go.SetActive(true);

        RhythmNote note = go.GetComponent<RhythmNote>();
        if (note == null)
        {
            Debug.LogError("[RhythmPuzzle] notePrefab에 RhythmNote 컴포넌트 필요");
            Destroy(go);
            Fail();
            return;
        }

        // 랜덤 위치(여백)
        Rect rect = playArea.rect;
        float margin = 60f;
        float x = Random.Range(rect.xMin + margin, rect.xMax - margin);
        float y = Random.Range(rect.yMin + margin, rect.yMax - margin);
        ((RectTransform)go.transform).anchoredPosition = new Vector2(x, y);

        // 정각(링 닿는 시점)
        float start = Time.unscaledTime + Mathf.Max(0f, extraDelay);
        scheduledTime = start + approachTime;

        note.Setup(this, nextIndex, scheduledTime);
        currentNote = note;
    }

    public void TryHit(RhythmNote note)
    {
        if (!inProgress || note != currentNote) return;

        float diff = Mathf.Abs(Now - scheduledTime);
        if (diff <= hitWindow)
        {
            currentNote.MarkHit();
            nextIndex++;
            SpawnNext(0f);
        }
        else
        {
            Debug.Log($"[RhythmPuzzle] Miss timing. diff={diff:0.000}s (±{hitWindow}s)");
            Fail();
        }
    }

    private void Update()
    {
        if (!inProgress) return;

        if (allowEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[RhythmPuzzle] ESC pressed → close");
            ClosePuzzle();
            return;
        }

        if (currentNote != null && Now > scheduledTime + hitWindow)
        {
            Debug.Log("[RhythmPuzzle] Miss (timeout)");
            Fail();
        }
    }

    private void Fail()
    {
        Debug.Log("[RhythmPuzzle] Puzzle Failed");
        ClosePuzzle();
    }
}
