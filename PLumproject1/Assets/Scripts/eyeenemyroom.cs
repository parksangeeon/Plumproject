using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyFindZone : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress = 0;

    public CinemachineCamera playerCam; // 플레이어 따라가는 카메라
    public CinemachineCamera enemyCam;  // 적 고정 카메라

    private bool hasTriggered = false;
    public bool IsCompleted { get; private set; }

    private string FlagKey => $"zone_{gameObject.scene.name}_{gameObject.name}";

    void Start()
    {
        if (GameFlags.GetBool(FlagKey))
        {
            hasTriggered = true;
            IsCompleted = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(EnemyRevealSequence());
        }
    }

    private IEnumerator EnemyRevealSequence()
    {
        ClearSky.Player.isControlBlocked = true;

        // 카메라를 적에게 전환
        enemyCam.Priority = 20;
        playerCam.Priority = 10;
        yield return new WaitForSeconds(0.5f);

        // 적 대사 (첫 번째) — 끝날 때까지 대기
        bool finished1 = false;
        System.Action onDone = null;
        onDone = () => { finished1 = true; monologueManager.DialogueFinished -= onDone; };
        monologueManager.DialogueFinished += onDone;
        bool started = monologueManager.StartTalk(talkData, progress);
        if (!started)
            monologueManager.DialogueFinished -= onDone; // 이미 봤거나 없는 대사 — 대기 없이 진행
        else
            yield return new WaitUntil(() => finished1);

        // 카메라 다시 플레이어에게
        playerCam.Priority = 20;
        enemyCam.Priority = 10;

        IsCompleted = true;
        GameFlags.Set(FlagKey, true);

        // 돌아온 후 대사 (없거나 실패해도 무방)
        monologueManager.StartTalk(talkData, progress + 1);
        // isControlBlocked는 MonologueManager가 대사 종료 시 자동 해제
    }
}