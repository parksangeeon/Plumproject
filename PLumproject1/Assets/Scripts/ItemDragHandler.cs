using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public IInventoryItem Item;          // HUD에서 주입
    public Inventory inventory;          // HUD에서 주입
    public RectTransform inventoryPanel; // (필수 아님) 필요 시 사용
    public Canvas canvas;                // HUD에서 주입 권장

    private CanvasGroup cg;
    private Transform originalParent;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        if (cg != null) cg.blocksRaycasts = false; // UI 레이캐스트 방해 금지
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Canvas Scaler 대응: eventData.position 사용
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cg != null) cg.blocksRaycasts = true;

        bool overUseTarget = IsOverUseTargetByTag(eventData);
        if (overUseTarget && Item != null && inventory != null)
        {
            inventory.UseItem(Item); // ← Tag="Item" 물체 위에서만 소모
        }
        // 아니면 아무 일 없음 (인벤토리에 그대로 남기고 아이콘만 복귀)

        // 아이콘 원위치 복귀
        transform.SetParent(originalParent, false);
        ((RectTransform)transform).anchoredPosition = Vector2.zero;
    }

    private bool IsOverUseTargetByTag(PointerEventData eventData)
    {
        // 월드 카메라 확보
        Camera cam = eventData.pressEventCamera ?? canvas?.worldCamera ?? Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[ItemDragHandler] No world camera found.");
            return false;
        }

        // 화면좌표 → 레이 → 2D 교차 검사 (모든 레이어)
        Ray ray = cam.ScreenPointToRay(eventData.position);
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);

        if (hits != null && hits.Length > 0)
        {
            // Tag == "Item" 이 하나라도 있으면 사용 허용
            foreach (var h in hits)
            {
                if (h.collider != null && h.collider.CompareTag("Item"))
                {
                    Debug.Log($"[ItemDragHandler] Tag hit: '{h.collider.name}' (tag=Item)");
                    return true;
                }
            }

            // 디버그: 뭐를 맞췄는지 로그
            foreach (var h in hits)
            {
                if (h.collider != null)
                    Debug.Log($"[DEBUG] hit-any '{h.collider.name}' layer={LayerMask.LayerToName(h.collider.gameObject.layer)} tag={h.collider.gameObject.tag}");
            }
        }
        else
        {
            Debug.Log("[ItemDragHandler] Ray hit nothing. Check Collider2D / camera.");
        }

        return false;
    }
}
