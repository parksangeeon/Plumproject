using ClearSky;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonologueManager : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public float typingSpeed = 0.05f;
    public GameObject Canvas;
    public MonologueManager monologueManager;
    private List<string> lines = new();  // 외부에서 설정
    private bool isTyping = false;
    private bool lineCompleted = false;

    void Awake()
    {
        if (Canvas != null)
            Canvas.SetActive(false);
    }

    public void SetLines(List<string> newLines)
    {
        lines = newLines;
        if (!Canvas.activeSelf) Canvas.SetActive(true);
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        StartCoroutine(ShowLines());
    }

    IEnumerator ShowLines()
    {
        Player.isControlBlocked = true;
        foreach (var line in lines)
        {
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            lineCompleted = false;
            textUI.text = "";
        }

        Player.isControlBlocked = false;
        gameObject.SetActive(false);
        Canvas.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        textUI.text = "";
        isTyping = true;

        foreach (char c in line)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineCompleted = true;
    }
}