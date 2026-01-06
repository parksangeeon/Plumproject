using UnityEngine;

public class LanternZoneTrigger : MonoBehaviour
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;
    public TalkData talkData;   
    public int progress = 1;    

    private bool hasEntered = false;
    private bool lanternPickedUp = false;

    void Start()
    {
        monologueManager.DialogueFinished += HandleDialogueFinished;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEntered || !other.CompareTag("Player")) return;

        hasEntered = true;
        instructionUI.SetActive(true);
        monologueManager.StartTalk(talkData, progress);
    }

    private void HandleDialogueFinished()
    {
        instructionUI.SetActive(false);
        ClearSky.Player.isControlBlocked = false;
    }

    void Update()
    {
     
    }
}