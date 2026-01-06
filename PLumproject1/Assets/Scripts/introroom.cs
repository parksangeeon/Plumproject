using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class introroom : MonoBehaviour
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;
    public TalkData talkData;    
    public int progress = 1;     

    private bool hasEntered = false;

    void OnTriggerEnter2D(Collider2D other)
    { 
        if (!other.CompareTag("Player")) return;
        if (hasEntered) { return; }
        hasEntered = true;
        instructionUI.SetActive(true);

        //  MonologueManager가 알아서 조작 잠금/해제 처리
        monologueManager.StartTalk(talkData, progress);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            instructionUI.SetActive(false);
            hasEntered = false; // 다시 들어올 수 있도록 리셋하고 싶으면 유지
        }
    }
}