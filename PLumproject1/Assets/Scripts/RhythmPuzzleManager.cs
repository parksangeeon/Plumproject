using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmPuzzleManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform playArea;   // 노트가 뜨는 영역
    public GameObject notePrefab;    // UI Image + RhythmNote 프리팹

    [Header("Puzzle Settings")]
    public int noteCount = 4;        // 총 노트 개수
    public float approachTime = 1.2f;// 링이 테두리에 닿기까지 걸리는 시간(초)
    public float hitWindow = 0.20f;  // 허용 타이밍(±초)
    public float startDelay = 0.5f;  // 첫 노트 전 준비 시간

    [Header("Events")]
    public UnityEvent puzzleClearedEvent;

    // 내부 상태
    private bool inProgress = false;
    private int nextIndex = 0;               // 현재 노트 번호(0부터)
    private RhythmNote currentNote = null;   // 지금 화면의 노트
    private float scheduledTime = 0f;        // 이번 노트 정각(링이 딱 맞닿는 시점)

    // 외부에서 읽을 수 있게(노트가 쓸 정보)
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
        currentNote = null;

        // 첫 노트 예약 시점: 지금 + startDelay + approachTime(링 접근을 보여주려면 즉시 스폰하고 approachTime만큼 수축)
        SpawnNext(afterDelay: startDelay);
        Debug.Log("[RhythmPuzzle] Start");
    }

    public void ClosePuzzle()
    {
        if (currentNote) Destroy(currentNote.gameObject);
        currentNote = null;
        inProgress = false;

        gameObject.SetActive(false);
        ClearSky.Player.isControlBlocked = false;
        Debug.Log("[RhythmPuzzle] Closed");
    }

    private void SpawnNext(float afterDelay = 0f)
    {
        // 기존 노트 제거
        if (currentNote) Destroy(currentNote.gameObject);
        currentNote = null;

        if (nextIndex >= noteCount)
        {
            // 모든 노트 클리어
            Debug.Log("[RhythmPuzzle] Puzzle Clear!");
            puzzleClearedEvent?.Invoke();
            ClosePuzzle();
            return;
        }

        // 스폰 위치 랜덤
        var go = Instantiate(notePrefab, playArea);
        go.SetActive(true);
        var note = go.GetComponent<RhythmNote>();
        if (note == null)
        {
            Debug.LogError("[RhythmPuzzle] notePrefab에 RhythmNote 컴포넌트 필요");
            Destroy(go);
            Fail(); // 안전상 실패처리
            return;
        }

        var rect = playArea.rect;
        float margin = 60f;
        float x = Random.Range(rect.xMin + margin, rect.xMax - margin);
        float y = Random.Range(rect.yMin + margin, rect.yMax - margin);
        ((RectTransform)go.transform).anchoredPosition = new Vector2(x, y);

        // 정각 시점(링이 딱 맞닿는 순간)
        float start = Time.unscaledTime + Mathf.Max(0f, afterDelay);
        scheduledTime = start + approachTime; // 지금부터 approachTime 동안 수축 → 정각

        note.Setup(this, nextIndex, scheduledTime);
        currentNote = note;
    }

    // RhythmNote가 클릭을 넘김
    public void TryHit(RhythmNote note)
    {
        if (!inProgress || note != currentNote) return;

        float diff = Mathf.Abs(Now - scheduledTime);
        if (diff <= hitWindow)
        {
            currentNote.MarkHit();   // 간단 피드백
            nextIndex++;
            SpawnNext();             // 다음 노트 스폰
        }
        else
        {
            Debug.Log($"[RhythmPuzzle] Miss timing. diff={diff:0.000}s (±{hitWindow}s)");
            Fail();
        }
    }

    private void Update()
    {
        if (!inProgress || currentNote == null) return;

        // 타임아웃(정각 + 허용 윈도우를 지나면 실패)
        if (Now > scheduledTime + hitWindow)
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
