using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrologueManager : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public CanvasGroup fadePanel;
    public Image backgroundImage;
    public Sprite backgroundSprite;
    public GameObject nextIcon;
    public MonologueManager monologueManager;
    
    

    [TextArea(3, 10)]
    public List<string> lines;

    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool lineCompleted = false;

    void Start()
    {
        monologueManager.gameObject.SetActive(false);
        monologueManager.Canvas.SetActive(false);
        StartCoroutine(RunPrologue());
        var player = FindAnyObjectByType<ClearSky.Player>();
        player.transform.position = new Vector3(0, 0, 0);
        ClearSky.Player.isControlBlocked = true;
    }



    IEnumerator RunPrologue()
    {
        // 조작 막기
        ClearSky.Player.isControlBlocked = true;

        yield return StartCoroutine(FadeIn());

        backgroundImage.sprite = backgroundSprite;

        for (int i = 0; i < lines.Count; i++)
        {
            yield return StartCoroutine(TypeLine(lines[i]));
            StartCoroutine(BlinkNextIcon());
            yield return StartCoroutine(WaitForSpacePress());
            StopCoroutine("BlinkNextIcon");
            nextIcon.SetActive(false);
            textUI.text = "";
        }

        yield return StartCoroutine(FadeOut());
        fadePanel.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);

        monologueManager.gameObject.SetActive(true); // 이게 있어야 Awake/Start 실행돼
        monologueManager.Canvas.SetActive(true);
        // 독백 대사 준비
        List<string> postPrologueLines = new List<string>
{
             "여긴... 어디지?",
             "병원인 것 같긴 한데...",
             "왜 내가 병실에 있는 거지?",
             "(상하 좌우 방향키로 움직입니다)"
        };


        monologueManager.gameObject.SetActive(true);
        monologueManager.SetLines(postPrologueLines);
    }
    IEnumerator WaitForSpacePress()
    {
        // 눌렸다가 뗄 때까지 기다림 (연속 입력 방지)
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return null; // 다음 프레임까지 기다려서 중복 방지
    }
    IEnumerator TypeLine(string line)
    {
        textUI.text = ""; // 이전 줄 지우기
        isTyping = true;

        foreach (char c in line)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineCompleted = true;
    }

    IEnumerator FadeIn()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            fadePanel.alpha = t;
            yield return null;
        }
        fadePanel.alpha = 0f;
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            fadePanel.alpha = t;
            yield return null;
        }
        fadePanel.alpha = 1f;
    }
    IEnumerator BlinkNextIcon()
    {
        nextIcon.SetActive(true);
        Image iconImage = nextIcon.GetComponent<Image>();

        while (lineCompleted)
        {
            iconImage.canvasRenderer.SetAlpha(0f);
            iconImage.CrossFadeAlpha(1f, 0.5f, false);
            yield return new WaitForSeconds(1f);

            iconImage.canvasRenderer.SetAlpha(1f);
            iconImage.CrossFadeAlpha(0f, 0.5f, false);
            yield return new WaitForSeconds(1f);
        }

        nextIcon.SetActive(false);
    }
}