using UnityEngine;

// 플레이어가 트리거에 들어오면 자동으로 대화를 시작하는 존들의 공통 로직.
// eventzone / doorzone / introroom이 각자 복붙해 구현하던 "진입 -> null 체크 -> StartTalk" 패턴을 여기로 모음.
public abstract class AutoTalkZoneBase : MonoBehaviour
{
    public bool oneShot = true;

    private bool hasTriggered = false;

    protected abstract MonologueManager TargetMonologueManager { get; }
    protected abstract TalkData TargetTalkData { get; }
    protected abstract int TargetProgress { get; }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        if (TargetMonologueManager == null || TargetTalkData == null)
        {
            Debug.LogError($"{name}: MonologueManager 또는 TalkData가 할당되지 않았습니다!");
            return;
        }

        hasTriggered = oneShot;
        OnBeforeTalk();
        TargetMonologueManager.StartTalk(TargetTalkData, TargetProgress);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!oneShot && other.CompareTag("Player"))
            hasTriggered = false;
    }

    protected virtual void OnBeforeTalk() { }
}
