using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RhythmPuzzleManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform playArea;     // 노트를 놓을 UI 영역(퍼즐 패널의 내부 RectTransform)
    public GameObject notePrefab;      // UI Image+RhythmNote 붙은 프리팹

    [Header("Puzzle Settings")]
    public int noteCount = 4;          // 노트 개수
    public float beatInterval = 5f;  // 각 노트 간 간격(초)
    public float hitWindow = 5f;    // 허용 타이밍 오차(초)
    public float startDelay = 1.0f;    // 퍼즐 시작 전 준비시간(초)

    [Header("Events")]
    public UnityEvent puzzleClearedEvent; // 성공 시 호출(문 열기 등 연결)

    private readonly List<RhythmNote> notes = new();
    private bool inProgress = false;
    private float startTimeUnscaled = 0f;
    private int nextIndex = 0;

    // 퍼즐 패널은 기본 비활성화로 두고, 열 때 활성화
    public void StartPuzzle()
    {
        if (inProgress) return;

        gameObject.SetActive(true);
        ClearSky.Player.isControlBlocked = true;

        inProgress = true;
        nextIndex = 0;
        startTimeUnscaled = Time.unscaledTime + startDelay;

        ClearNotes();
        SpawnNotes();
        Debug.Log("[RhythmPuzzle] Start");
    }

    public void ClosePuzzle()
    {
        ClearNotes();
        inProgress = false;
        gameObject.SetActive(false);
        ClearSky.Player.isControlBlocked = false;
        Debug.Log("[RhythmPuzzle] Closed");
    }

    private void SpawnNotes()
    {
        // playArea 안의 랜덤 위치에 noteCount개 배치
        for (int i = 0; i < noteCount; i++)
        {
            var go = Instantiate(notePrefab, playArea);
            var note = go.GetComponent<RhythmNote>();
            if (note == null)
            {
                Debug.LogError("[RhythmPuzzle] notePrefab에 RhythmNote 컴포넌트가 필요합니다.");
                Destroy(go);
                continue;
            }

            // 랜덤 위치(여백 조금)
            var rt = (RectTransform)go.transform;
            var rect = playArea.rect;
            float margin = 40f;
            float x = Random.Range(rect.xMin + margin, rect.xMax - margin);
            float y = Random.Range(rect.yMin + margin, rect.yMax - margin);
            rt.anchoredPosition = new Vector2(x, y);

            float scheduled = startTimeUnscaled + i * beatInterval;
            note.Setup(this, i, scheduled);
            notes.Add(note);
        }
    }

    private void ClearNotes()
    {
        foreach (var n in notes)
        {
            if (n) Destroy(n.gameObject);
        }
        notes.Clear();
    }

    // RhythmNote에서 클릭 시 호출
    public void TryHit(RhythmNote note)
    {
        if (!inProgress) return;

        float now = Time.unscaledTime;

        // 순서 체크
        if (note.Index != nextIndex)
        {
            Debug.Log($"[RhythmPuzzle] Wrong order. expected={nextIndex}, clicked={note.Index}");
            Fail();
            return;
        }

        // 타이밍 체크
        float diff = Mathf.Abs(now - note.ScheduledTime);
        if (diff <= hitWindow)
        {
            note.MarkHit();
            nextIndex++;

            if (nextIndex >= noteCount)
            {
                Debug.Log("[RhythmPuzzle] Puzzle Clear!");
                puzzleClearedEvent?.Invoke();
                ClosePuzzle();
            }
        }
        else
        {
            Debug.Log($"[RhythmPuzzle] Miss timing. diff={diff:0.000}s (±{hitWindow}s allowed)");
            Fail();
        }
    }

    private void Update()
    {
        if (!inProgress) return;

        // 타이밍 오버로 실패 처리(다음 노트의 허용 시간 지나면 실패)
        if (nextIndex < notes.Count)
        {
            float now = Time.unscaledTime;
            var nextNote = notes[nextIndex];
            if (now > nextNote.ScheduledTime + hitWindow)
            {
                Debug.Log("[RhythmPuzzle] Miss (timeout)");
                Fail();
            }
        }
    }

    private void Fail()
    {
        Debug.Log("[RhythmPuzzle] Puzzle Failed");
        ClosePuzzle();
        // 필요하면 재시도 UI를 띄우거나, 패널을 그대로 두고 Reset 기능 추가 가능
    }
}
