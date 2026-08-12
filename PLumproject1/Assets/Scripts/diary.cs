using UnityEngine;

public class diary : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress = 0;
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Z))
        {
            bool started = monologueManager.StartTalk(talkData, progress);
            if (started)
            {
                ClearSky.Player.isControlBlocked = true;
                monologueManager.DialogueFinished += OnReadFinished;
            }
        }
    }

    void OnReadFinished()
    {
        ClearSky.Player.isControlBlocked = false;
        monologueManager.DialogueFinished -= OnReadFinished;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}
