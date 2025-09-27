using UnityEngine;

public class PuzzleMonsterBinder : MonoBehaviour
{
    public RhythmPuzzleManager puzzle;      // 퍼즐 매니저
    public EnemiesController enemy;         // 몬스터 컨트롤러

    private void Reset()
    {
        if (enemy == null) enemy = GetComponent<EnemiesController>();
    }

    private void OnEnable()
    {
        // RhythmPuzzleManager에 onOpened/onClosed 이벤트가 있다면 구독
        TryBindEvents(true);
    }

    private void OnDisable()
    {
        TryBindEvents(false);
    }

    private void TryBindEvents(bool bind)
    {
        if (puzzle == null) return;

        // onOpened / onClosed 필드가 있는 버전이면 구독/해제
        // (없으면 그냥 무시됨 — 인스펙터에서 수동 연결을 쓰면 됨)
        try
        {
            if (bind)
            {
                puzzle.onOpened.AddListener(OnPuzzleOpened);
                puzzle.onClosed.AddListener(OnPuzzleClosed);
            }
            else
            {
                puzzle.onOpened.RemoveListener(OnPuzzleOpened);
                puzzle.onClosed.RemoveListener(OnPuzzleClosed);
            }
        }
        catch { /* 이벤트가 없는 옛 버전이면 조용히 패스 */ }
    }

    // 퍼즐 열릴 때: 전진 시작
    public void OnPuzzleOpened()
    {
        if (enemy != null) enemy.StartChaseLeft();
    }

    // 퍼즐 닫힐 때(ESC 포함): 리셋
    public void OnPuzzleClosed()
    {
        if (enemy != null) enemy.ResetToDefault();
    }

    // ▼ 이벤트가 없는 경우를 위해 퍼블릭 메서드도 제공 (인스펙터에서 직접 연결해 사용 가능)
    public void StartChase() => OnPuzzleOpened();
    public void ResetMonster() => OnPuzzleClosed();
}
