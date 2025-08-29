using UnityEngine;

public class LanternItem : MonoBehaviour, IInventoryItem
{
    public string Name => "Lantern";

    [SerializeField] private Sprite image;
    public Sprite Image => image;

    public void OnPickup()
    {
        // 시각 효과 등을 여기에 추가할 수 있음
        gameObject.SetActive(false); // 월드에서 제거
    }

    public void OnDrop()
    {
        Debug.Log("랜턴은 드롭할 수 없습니다.");
    }
}