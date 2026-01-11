using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string targetSceneName;        
    public string destinationPointName;   
    private bool canEnter = false;
    private ClearSky.Player thePlayer;
    private static bool justEntered = false;
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress;

    void Start()
    {
        thePlayer = FindAnyObjectByType<ClearSky.Player>();
        Invoke("ClearJustEntered", 0.2f);
    }

    void ClearJustEntered()
    {
        justEntered = false;
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.DownArrow) && !justEntered)
        {
           
            
            if (talkData != null)
            {
                monologueManager.DialogueFinished += MoveScene;
                monologueManager.StartTalk(talkData, progress);
            }
            else
            {
                MoveScene();
            }
            justEntered = true;
        }
    }
    private void MoveScene()
    {
        monologueManager.DialogueFinished -= MoveScene; // 중복 구독 방지

        thePlayer.GoingPointName = destinationPointName;
        StartCoroutine(ChangeSceneWithDelay());
    }
    IEnumerator ChangeSceneWithDelay()
    {
        justEntered = true;
        yield return new WaitForSeconds(0.2f);  // 혹은 페이드 아웃 코루틴
        SceneManager.LoadScene(targetSceneName);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }
}