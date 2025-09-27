using UnityEngine;

public class LanternItem : MonoBehaviour, IInventoryItem
{
    public string Name => "Lantern";

    [SerializeField] private Sprite image;
    public Sprite Image => image;

    public void OnPickup()
    {
        // 인벤토리에 들어갈 때 월드에서 숨김
        gameObject.SetActive(false);
    }

    public void OnDrop()
    {
        // 월드에 드롭될 때 행동(원하면 콜라이더/물리 활성화 등)
        Debug.Log("[LanternItem] Dropped.");
    }

    public void OnUse() // ← 딱 하나만 존재해야 함
    {
        // 퍼즐/던전 등에서 사용됐을 때 행동
        Debug.Log("[LanternItem] Used.");
        // TODO: 실제 효과(라이트 켜기 등) 구현
        // ex) GetComponentInChildren<Light2D>()?.gameObject.SetActive(true);
    }
}
