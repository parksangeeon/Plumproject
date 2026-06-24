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
    [SerializeField] private GameObject inventoryHud;

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

    // 대사가 실제로 시작됐으면 true, 매칭되는 progress가 없어 아무 일도 안 일어났으면 false를 반환.
    // 호출자가 "대화 끝나면 다음 동작" 같은 걸 DialogueFinished 이벤트에 의존하고 있다면
    // 이 반환값으로 대화가 시작되지 않았을 때를 구분해서 직접 다음 동작을 처리해야 함 (예: Door.cs)
    public bool StartTalk(TalkData talkData, int progress)
    {
        if (talkData == null)
        {
            Debug.LogError("MonologueManager.StartTalk: talkData가 null입니다!");
            return false;
        }

        if (talkData.contents == null)
        {
            Debug.LogError("MonologueManager.StartTalk: talkData.contents가 null입니다!");
            return false;
        }

        TalkContent selected = null;

        foreach (var t in talkData.contents)
        {
            if (t.progress == progress)
            {

                if (!t.isRepeatable && seenEvents.Contains(progress))
                    return false;

                selected = t;

                if (!t.isRepeatable)
                    seenEvents.Add(progress);

                break;
            }
        }

        if (selected != null && selected.scripts.Length > 0)
        {
            ClearSky.Player.isControlBlocked = true;
            curTalkList = selected.scripts;
            monologueCanvas.SetActive(true);
            if (inventoryHud != null) inventoryHud.SetActive(false);
            curTalkIndex = 0;
            NextTalk();
            return true;
        }
        else
        {
            Debug.Log("대화가 없습니다.");
            return false;
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
            if (inventoryHud != null) inventoryHud.SetActive(true);
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
