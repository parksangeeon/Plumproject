using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternZoneTrigger : MonoBehaviour
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;

    private bool hasEntered = false;
    private bool dialogueFinished = false;
    private bool lanternPickedUp = false;

    void Start()
    {
       
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEntered || !other.CompareTag("Player")) return;

        hasEntered = true;
        instructionUI.SetActive(true);

        // 플레이어 조작 잠금
        ClearSky.Player.isControlBlocked = true;

        // 대사 출력
        List<string> lines = new List<string>
        {
            "불빛이다...",
            "이게있으면 더 밝게 볼 수 있겠어...",
            "(Z키로 상호작용합니다)"
        };
        monologueManager.SetLines(lines);

        StartCoroutine(WaitForMonologueThenEnablePickup());
    }

    IEnumerator WaitForMonologueThenEnablePickup()
    {
        // 모노로그 끝날 때까지 기다림
        while (monologueManager.gameObject.activeSelf)
            yield return null;

        dialogueFinished = true;
        instructionUI.SetActive(false);
        
        ClearSky.Player.isControlBlocked = false; // 조작 해제
    }

    void Update()
    {
        if (dialogueFinished && !lanternPickedUp && Input.GetKeyDown(KeyCode.Z))
        {
            lanternPickedUp = true;
            
        }
    }
}