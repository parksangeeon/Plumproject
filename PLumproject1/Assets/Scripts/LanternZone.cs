using UnityEngine;
using System.Collections;

public class LanternZoneTrigger : MonoBehaviour
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;
    public TalkData talkData;   
    public int progress = 1;    

    [Header("Flag Names")]
    public string flagHasEntered = "lantern_zone_entered";
    public string flagDialogueFinished = "lantern_zone_dialogue_finished";
    public string flagLanternPickedUp = "lantern_picked_up";

    private bool hasEntered = false;
    private bool dialogueFinished = false;
    private bool lanternPickedUp = false;

    void Start()
    {
        RestoreState();
        if (monologueManager != null)
        {
            monologueManager.DialogueFinished += HandleDialogueFinished;
        }
        else
        {
            Debug.LogError("LanternZoneTrigger: MonologueManager가 할당되지 않았습니다!");
        }
    }

    // SaveGameLoader가 플래그 복원 후 호출
    void OnLoadGame()
    {
        RestoreState();
    }

    void RestoreState()
    {
        // 플래그에서 상태 복원
        hasEntered = GameFlags.GetBool(flagHasEntered, false);
        dialogueFinished = GameFlags.GetBool(flagDialogueFinished, false);
        lanternPickedUp = GameFlags.GetBool(flagLanternPickedUp, false);

        // 복원된 상태에 따라 UI 업데이트
        if (instructionUI != null)
        {
            if (dialogueFinished)
            {
                instructionUI.SetActive(false);
            }
            else if (hasEntered)
            {
                // hasEntered가 true면 대화가 진행 중이거나 진행되지 않은 상태
                instructionUI.SetActive(true);
            }
            else
            {
                instructionUI.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEntered || !other.CompareTag("Player")) return;

        hasEntered = true;
        GameFlags.Set(flagHasEntered, true);
        instructionUI.SetActive(true);

        // 플레이어 조작 잠금
        ClearSky.Player.isControlBlocked = true;

        // 대화 시작 (main 방식 사용)
        bool dialogueStarted = false;
        if (monologueManager != null && talkData != null)
        {
            dialogueStarted = monologueManager.StartTalk(talkData, progress);
        }
        else
        {
            Debug.LogError("LanternZoneTrigger: MonologueManager 또는 TalkData가 할당되지 않았습니다!");
        }

        // 대화가 실제로 시작되지 않았다면(매칭되는 대사 없음 등) 조작 잠금을 풀어줘야
        // HandleDialogueFinished가 영원히 안 불려서 플레이어가 멈추는 일이 없음
        if (!dialogueStarted)
        {
            ClearSky.Player.isControlBlocked = false;
        }
    }

    private void HandleDialogueFinished()
    {
        dialogueFinished = true;
        instructionUI.SetActive(false);
        GameFlags.Set(flagDialogueFinished, true);
        ClearSky.Player.isControlBlocked = false;
    }

    void Update()
    {
        if (dialogueFinished && !lanternPickedUp && Input.GetKeyDown(KeyCode.Z))
        {
            lanternPickedUp = true;
            GameFlags.Set(flagLanternPickedUp, true);
        }
    }
}
