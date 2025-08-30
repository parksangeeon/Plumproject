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
        // ���� ����
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

        monologueManager.gameObject.SetActive(true); // �̰� �־�� Awake/Start �����
        monologueManager.Canvas.SetActive(true);
        // ���� ��� �غ�
        List<string> postPrologueLines = new List<string>
{
             "����... �����?",
             "������ �� ���� �ѵ�...",
             "�� ���� ���ǿ� �ִ� ����?",
             "(���� �¿� ����Ű�� �����Դϴ�)"
        };


        monologueManager.gameObject.SetActive(true);
        monologueManager.SetLines(postPrologueLines);
    }
    IEnumerator WaitForSpacePress()
    {
        // ���ȴٰ� �� ������ ��ٸ� (���� �Է� ����)
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return null; // ���� �����ӱ��� ��ٷ��� �ߺ� ����
    }
    IEnumerator TypeLine(string line)
    {
        textUI.text = ""; // ���� �� �����
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