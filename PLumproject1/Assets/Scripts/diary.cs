using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class diary : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;
    private bool isPlayerNear = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
  
        }
    }


void Update()
    {
        if ( isPlayerNear &&Input.GetKeyDown(KeyCode.Z))
         {
            if (monologueManager != null && talkData != null)
            {
                monologueManager.StartTalk(talkData, 0);
            }
        }
    }
}
