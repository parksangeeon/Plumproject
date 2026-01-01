using System.Collections.Generic;

/// <summary>
/// 게임 상태를 저장/로드할 수 있는 오브젝트들이 구현하는 인터페이스
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// 이 오브젝트의 고유 ID (예: "LanternZone_1", "Door_Hall_1")
    /// </summary>
    string GetSaveID();

    /// <summary>
    /// 현재 상태를 Dictionary로 반환 (저장 시 호출)
    /// </summary>
    Dictionary<string, string> CaptureState();

    /// <summary>
    /// 저장된 상태를 복원 (로드 시 호출)
    /// </summary>
    void RestoreState(Dictionary<string, string> state);
}


