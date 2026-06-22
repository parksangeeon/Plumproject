using UnityEngine;

public class doorzone : AutoTalkZoneBase
{
    public TalkData talkData;
    public MonologueManager monologueManager;

    protected override MonologueManager TargetMonologueManager => monologueManager;
    protected override TalkData TargetTalkData => talkData;
    protected override int TargetProgress => 4;
}
