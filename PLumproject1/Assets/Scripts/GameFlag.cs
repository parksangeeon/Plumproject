using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameFlag")]
public class GameFlag : ScriptableObject
{
    private bool _value;
    public bool Value => _value;

    public void Set(bool value)
    {
        _value = value;
        GameFlags.Set(name, value);
    }

    // 세이브 파일 로드 후 GameFlags에서 복원
    public void RestoreFromSave()
    {
        _value = GameFlags.GetBool(name, false);
    }
}
