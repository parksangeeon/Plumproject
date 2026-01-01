using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternZoneTrigger : MonoBehaviour
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;

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

        // 대화 시작
        List<string> lines = new List<string>
        {
            "어둡다...",
            "이런 곳에서 랜턴을 찾아야 하나...",
            "(Z키로 획득하세요)"
        };
        monologueManager.SetLines(lines);

        StartCoroutine(WaitForMonologueThenEnablePickup());
    }

    IEnumerator WaitForMonologueThenEnablePickup()
    {
        // 대화가 끝날 때까지 대기
        while (monologueManager.gameObject.activeSelf)
            yield return null;

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
