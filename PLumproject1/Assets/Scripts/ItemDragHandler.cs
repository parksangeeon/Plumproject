using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public IInventoryItem Item;                 // HUD에서 주입
    public Inventory inventory;                 // HUD에서 주입
    public RectTransform inventoryPanel;        // HUD에서 주입
    public Canvas canvas;                       // 드래그하는 UI가 속한 캔버스

    private CanvasGroup cg;
    private Transform originalParent;

    void Awake()
    {
         cg = GetComponent<CanvasGroup>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (inventoryPanel == null)
        {
            var go = GameObject.Find("InventoryPanel"); // 이름 맞춰서
            if (go != null) inventoryPanel = go.GetComponent<RectTransform>();
        }
            
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        // 드래그 중에는 다른 UI가 레이캐스트를 받을 수 있게 해야 함
        if (cg != null) cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 캔버스 스케일 고려
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cg != null) cg.blocksRaycasts = true;

        //bool outside = IsOutsideInventory(eventData);
        // Debug.Log($"EndDrag outside={outside}");
        /*
        if (outside && Item != null && inventory != null)
        {
            inventory.RemoveItem(Item); // ← 여기서 반드시 인벤토리 제거 호출
        }

        // 아이콘은 원위치
        transform.SetParent(originalParent);
        (transform as RectTransform).anchoredPosition = Vector2.zero;*/
    }

    /*bool IsOutsideInventory(PointerEventData eventData)
{
    if (inventoryPanel == null)
    {
        Debug.LogWarning("[ItemDragHandler] inventoryPanel is NULL. Treating as outside.");
        return true; // 최소한 예외는 막자
    }

    var cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : canvas?.worldCamera;

    if (RectTransformUtility.RectangleContainsScreenPoint(inventoryPanel, eventData.position, cam))
        return false;

    // … (월드 코너로 보수 판정하는 코드 그대로)
}
*/
}
