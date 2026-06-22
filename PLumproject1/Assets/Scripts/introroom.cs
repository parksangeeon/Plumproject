using UnityEngine;

public class introroom : AutoTalkZoneBase
{
    public GameObject instructionUI;
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress = 1;

    protected override MonologueManager TargetMonologueManager => monologueManager;
    protected override TalkData TargetTalkData => talkData;
    protected override int TargetProgress => progress;

    void Awake()
    {
        oneShot = false; // 원본 코드는 나갈 때마다 다시 들어올 수 있도록 리셋함
    }

    protected override void OnBeforeTalk()
    {
        if (instructionUI != null) instructionUI.SetActive(true);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.CompareTag("Player") && instructionUI != null)
            instructionUI.SetActive(false);
    }
}
