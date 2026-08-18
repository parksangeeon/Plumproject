using UnityEngine;
using UnityEngine.Events;

public class RhythmPuzzleManager : MonoBehaviour
{
    [Header("References")]
    public GameObject panelRoot;        // 실제로 켜고 끌 루트 패널 (비우면 이 오브젝트 자체)
    public RectTransform playArea;      // 노트가 뜨는 UI 영역
    public GameObject notePrefab;       // RhythmNote가 붙은 UI 프리팹

    [Header("Puzzle Settings")]
    public int noteCount = 4;           // 총 노트 개수
    public float approachTime = 1.2f;   // 흰 링이 테두리에 닿기까지 걸리는 시간
    public float hitWindow = 0.20f;     // 허용 타이밍(±초)
    public float startDelay = 0.5f;     // 첫 노트 전 준비 시간

    [Header("Control")]
    public bool allowEscape = true;     // ESC로 종료 허용

    [Header("Events (optional)")]
    public UnityEvent puzzleClearedEvent;   // 성공 시만 호출
    public UnityEvent onOpened;             // 퍼즐 열릴 때 호출
    public UnityEvent onClosed;             // 퍼즐 닫힐 때(성공/실패/ESC) 호출

    [Header("Note Spawn Positions (열쇠 모양 순서대로)")]
    public RectTransform[] noteSpawnPoints;

    [Header("Monster (optional direct binding)")]
    public EnemiesController enemy;         // 있으면 시작/종료 시 자동 제어(없어도 정상 동작)

    // 내부 상태
    private bool inProgress = false;
    private int nextIndex = 0;                 // 현재 노트 인덱스(0부터)
    private RhythmNote currentNote = null;     // 지금 화면의 노트
    private float scheduledTime = 0f;          // 이번 노트 정각

    // 외부 읽기용
    public bool InProgress => inProgress;
    public float Now => Time.unscaledTime;
    public float HitWindow => hitWindow;
    public float ApproachTime => approachTime;
    public int NextIndex => nextIndex;
    public float ScheduledTime => scheduledTime;

    // 퍼즐 시작
    public void StartPuzzle()
    {
        if (inProgress) return;

        if (playArea == null || notePrefab == null)
        {
            Debug.LogError("[RhythmPuzzle] playArea 또는 notePrefab이 비어 있습니다.");
            return;
        }

        // 패널 활성화
        (panelRoot != null ? panelRoot : gameObject).SetActive(true);

        inProgress = true;
        nextIndex = 0;
        DestroyCurrentNote();

        // 첫 노트 스폰 (startDelay 후 approachTime 동안 링 수축 → 정각)
        SpawnNext(startDelay);

        // 이벤트/몬스터 호출
        onOpened?.Invoke();
        if (enemy != null) enemy.StartChaseLeft();

        Debug.Log("[RhythmPuzzle] Start");
    }

    // 퍼즐 종료(성공/실패/ESC 공통)
    public void ClosePuzzle()
    {
        DestroyCurrentNote();
        inProgress = false;

        // 패널 비활성화
        (panelRoot != null ? panelRoot : gameObject).SetActive(false);

        // 몬스터/이벤트
        if (enemy != null) enemy.ResetToDefault();
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

    // 한 번에 하나만 보이게 스폰
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

        var go = Instantiate(notePrefab, playArea);
        go.SetActive(true);

        var note = go.GetComponent<RhythmNote>();
        if (note == null)
        {
            Debug.LogError("[RhythmPuzzle] notePrefab에 RhythmNote 컴포넌트가 필요합니다.");
            Destroy(go);
            Fail();
            return;
        }

        // 고정 위치(noteSpawnPoints) 또는 랜덤 위치
        if (noteSpawnPoints != null && nextIndex < noteSpawnPoints.Length && noteSpawnPoints[nextIndex] != null)
        {
            ((RectTransform)go.transform).anchoredPosition = noteSpawnPoints[nextIndex].anchoredPosition;
        }
        else
        {
            Rect rect = playArea.rect;
            float margin = 60f;
            float x = Random.Range(rect.xMin + margin, rect.xMax - margin);
            float y = Random.Range(rect.yMin + margin, rect.yMax - margin);
            ((RectTransform)go.transform).anchoredPosition = new Vector2(x, y);
        }

        // 정각(링이 테두리에 딱 닿는 시점)
        float start = Time.unscaledTime + Mathf.Max(0f, extraDelay);
        scheduledTime = start + approachTime;

        note.Setup(this, nextIndex, scheduledTime);
        currentNote = note;
    }

    // 노트 클릭 시 RhythmNote에서 호출
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

        // ESC로 즉시 종료
        if (allowEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[RhythmPuzzle] ESC pressed → close");
            ClosePuzzle();
            return;
        }

        // 타임아웃(정각 + 허용 윈도우 지나면 실패)
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
