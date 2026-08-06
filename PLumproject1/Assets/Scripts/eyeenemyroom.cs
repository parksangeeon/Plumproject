using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyFindZone : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress = 0;

    public CinemachineCamera playerCam;
    public CinemachineCamera enemyCam;

    [Header("완료 플래그 — 같은 에셋을 DoorTrigger의 Skip Talk If에도 연결하세요")]
    public GameFlag completionFlag;

    private bool hasTriggered = false;
    public bool IsCompleted { get; private set; }

    void Start()
    {
        if (completionFlag != null)
            completionFlag.RestoreFromSave();

        if (completionFlag != null && completionFlag.Value)
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

        enemyCam.Priority = 20;
        playerCam.Priority = 10;
        yield return new WaitForSeconds(0.5f);

        bool finished1 = false;
        System.Action onDone = null;
        onDone = () => { finished1 = true; monologueManager.DialogueFinished -= onDone; };
        monologueManager.DialogueFinished += onDone;
        bool started = monologueManager.StartTalk(talkData, progress);
        if (!started)
            monologueManager.DialogueFinished -= onDone;
        else
            yield return new WaitUntil(() => finished1);

        playerCam.Priority = 20;
        enemyCam.Priority = 10;

        IsCompleted = true;
        completionFlag?.Set(true);

        monologueManager.StartTalk(talkData, progress + 1);
    }
}
