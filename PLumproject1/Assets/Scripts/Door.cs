using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public SceneEntrance destinationEntrance;
    private bool canEnter = false;
    private ClearSky.Player thePlayer;
    private static bool justEntered = false;
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress;

    [Header("이동 차단 조건")]
    public EnemyFindZone requiredZone;      // 완료되어야 통과 가능한 존 (없으면 조건 없음)
    public TalkData blockingTalkData;       // 조건 미충족 시 출력할 대사
    public int blockingProgress = 0;

    [Header("대사 건너뛰기 조건")]
    public GameFlag skipTalkIf;

    void Start()
    {
        thePlayer = ClearSky.Player.Instance;
        Invoke("ClearJustEntered", 0.2f);
    }

    void ClearJustEntered()
    {
        justEntered = false;
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.DownArrow) && !justEntered)
        {
            // 지정된 존이 아직 완료되지 않았으면 이동 차단 (대사만 출력, justEntered 건드리지 않음)
            if (requiredZone != null && !requiredZone.IsCompleted)
            {
                if (blockingTalkData != null)
                    monologueManager.StartTalk(blockingTalkData, blockingProgress);
                return;
            }

            // SO 런타임 값 OR GameFlags(세이브 로드 복원) 둘 다 체크
            bool skipDialogue = skipTalkIf != null &&
                (skipTalkIf.Value || GameFlags.GetBool(skipTalkIf.name, false));

            bool dialogueStarted = false;
            if (!skipDialogue && talkData != null)
            {
                monologueManager.DialogueFinished += MoveScene;
                dialogueStarted = monologueManager.StartTalk(talkData, progress);
            }

            if (!dialogueStarted)
            {
                if (!skipDialogue && talkData != null) monologueManager.DialogueFinished -= MoveScene;
                MoveScene();
            }

            justEntered = true;
        }
    }
    private void MoveScene()
    {
        monologueManager.DialogueFinished -= MoveScene; // 중복 구독 방지
        ClearSky.Player.isControlBlocked = false; // 진행 중인 시네마틱이 있어도 이동 시 강제 해제

        thePlayer.pendingEntrance = destinationEntrance;
        StartCoroutine(ChangeSceneWithDelay());
    }
    IEnumerator ChangeSceneWithDelay()
    {
        justEntered = true;
        yield return new WaitForSeconds(0.2f);  // 혹은 페이드 아웃 코루틴
        string targetScene = SceneEntranceHelper.GetTargetScene(destinationEntrance);
        if (targetScene != null)
            SceneManager.LoadScene(targetScene);
        else
            UnityEngine.Debug.LogError($"[DoorTrigger] {destinationEntrance}에 대한 씬 매핑이 없습니다. SceneEntrance.cs의 connections를 확인하세요.");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }
}