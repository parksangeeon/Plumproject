using UnityEngine;

public class eventzone : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           monologueManager.StartTalk(talkData, 0);
           
        }
    }
            // Update is called once per frame
            void Update()
    {
        
    }
}
