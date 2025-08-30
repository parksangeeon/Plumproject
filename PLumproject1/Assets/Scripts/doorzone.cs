using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorzone : MonoBehaviour
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

       
        ClearSky.Player.isControlBlocked = true;

        
        List<string> lines = new List<string>
        {
            "문이다...",
            "일단 밖으로 나가봐야겠어...",
            "(아래 방향키로 움직입니다)"
        };
        monologueManager.SetLines(lines);

        StartCoroutine(WaitForMonologueThenEnablePickup());
    }

    IEnumerator WaitForMonologueThenEnablePickup()
    {
        
        while (monologueManager.gameObject.activeSelf)
            yield return null;

        dialogueFinished = true;
        instructionUI.SetActive(false);

        ClearSky.Player.isControlBlocked = false; 
    }

    void Update()
    {
        if (dialogueFinished && !lanternPickedUp && Input.GetKeyDown(KeyCode.Z))
        {
            lanternPickedUp = true;

        }
    }
}