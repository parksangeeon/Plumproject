using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TalkData", menuName = "Talk/Talk Data")]
public class TalkData : ScriptableObject
{
    public TYPE type;                  // 어떤 NPC인지 (또는 PlayerMonologue)
    public List<TalkContent> contents; // 진행도별 대사
}

[Serializable]
public class TalkContent
{
    public int progress;
    public bool isRepeatable;       // 반복 가능 여부
    public bool trigger;
    [TextArea(2, 6)]
    public string[] scripts;
}