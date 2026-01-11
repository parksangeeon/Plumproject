using UnityEngine;

public class doorzone : MonoBehaviour
{
    public TalkData talkData;
    public MonologueManager monologueManager;

    private bool hasEntered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEntered || !other.CompareTag("Player")) return;

        hasEntered = true;

        
        monologueManager.StartTalk(talkData, 4); 
    }
}