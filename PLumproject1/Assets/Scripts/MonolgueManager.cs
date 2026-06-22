using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MonologueManager : MonoBehaviour
{
    private HashSet<int> seenEvents = new HashSet<int>();
    private string[] curTalkList;
    private int curTalkIndex = 0;
    public System.Action DialogueFinished;
    private bool isTyping = false;   
    private bool lineCompleted = false;
    public bool IsTalking => curTalkList != null && curTalkIndex < curTalkList.Length;


    [SerializeField] private GameObject monologueCanvas;
    [SerializeField] private TextMeshProUGUI textUI;

    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    void Awake()
    {
        Initialize();
    }
    void Update()
    {

        if (monologueCanvas.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping) 
            {
                StopCoroutine(typingCoroutine);
                textUI.text = curTalkList[curTalkIndex - 1]; 
                isTyping = false;
                lineCompleted = true;
            }
            else if (lineCompleted) 
            {
                lineCompleted = false;
                NextTalk();
            }
        }
    }
    public void Initialize()
    {
        textUI.text = "";
        curTalkIndex = 0;
        curTalkList = null;
        monologueCanvas.SetActive(false); 
    }

    public void StartTalk(TalkData talkData, int progress)
    {
        if (talkData == null)
        {
            Debug.LogError("MonologueManager.StartTalk: talkData가 null입니다!");
            return;
        }

        if (talkData.contents == null)
        {
            Debug.LogError("MonologueManager.StartTalk: talkData.contents가 null입니다!");
            return;
        }

        ClearSky.Player.isControlBlocked = true;
        TalkContent selected = null;

        foreach (var t in talkData.contents)
        {
            if (t.progress == progress)
            {
                
                if (!t.isRepeatable && seenEvents.Contains(progress))
                    return;

                selected = t;

                if (!t.isRepeatable)
                    seenEvents.Add(progress);

                break;
            }
        }

        if (selected != null && selected.scripts.Length > 0)
        {
            curTalkList = selected.scripts;
            monologueCanvas.SetActive(true);
            curTalkIndex = 0;
            NextTalk();
        }
        else
        {
            Debug.Log("대화가 없습니다.");
        }
    }

    public bool NextTalk()
    {
        if (curTalkList != null && curTalkIndex < curTalkList.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine(curTalkList[curTalkIndex]));
            curTalkIndex++;
            return true;
        }
        else
        {
            Debug.Log("대화 종료 - isControlBlocked 해제됨");
            Initialize();
            ClearSky.Player.isControlBlocked = false;
            DialogueFinished?.Invoke();
            return false;
        }
    }

    IEnumerator TypeLine(string line)
    {
        textUI.text = "";
        isTyping = true;
        lineCompleted = false;

        foreach (char c in line)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineCompleted = true;
    }
}
