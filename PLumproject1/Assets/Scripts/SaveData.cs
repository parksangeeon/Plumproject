using System;
using System.Collections.Generic;


[Serializable]
public class SaveData
{
public string sceneName;
public string timestamp; // 저장 시간 문자열
public float playSeconds; // 선택사항(원하면 누적시간 기록)


// 플레이어 위치/상태
public float px, py, pz; // world position
public string goingPointName; // 사용 안 할 땐 null/""


// 인벤토리(문자 ID)
public List<string> itemIds = new();
}


[Serializable]
public class SaveSummary
{
public string sceneName;
public string timestamp;
public int itemCount;
}