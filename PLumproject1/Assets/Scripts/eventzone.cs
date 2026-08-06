using UnityEngine;

public class eventzone : AutoTalkZoneBase
{
    public MonologueManager monologueManager;
    public TalkData talkData;

    protected override MonologueManager TargetMonologueManager => monologueManager;
    protected override TalkData TargetTalkData => talkData;
    protected override int TargetProgress => 0;

    void Awake()
    {
        oneShot = false; // 원본 코드는 진입 가드가 없어 매번 재발동되는 동작이었음
    }
}
