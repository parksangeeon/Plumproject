using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Inventory Inventory;
    
    void Start()
    {
        if (Inventory != null)
        {
            Inventory.ItemAdded += OnItemAdded;       // ← 메서드명 새로 통일
            Inventory.ItemRemoved += OnItemRemoved;
        }
    }
     

    void OnDestroy()
    {
        if (Inventory != null)
        {
            Inventory.ItemAdded -= OnItemAdded;
            Inventory.ItemRemoved -= OnItemRemoved;
        }
    }

    private void OnItemAdded(object sender, InventoryEventArgs e)
    {
        // 인벤토리 패널 찾기
        Transform inventoryPanelTransform = transform.Find("InventoryPanel");
        if (inventoryPanelTransform == null)
        {
            Debug.LogError("[HUD] 'InventoryPanel'을 찾지 못했습니다.");
            return;
        }
        RectTransform inventoryPanelRT = inventoryPanelTransform as RectTransform;

        foreach (Transform slot in inventoryPanelTransform)
        {
            // Border -> (자식) -> ItemImage 구조라고 가정
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler drag = imageTransform.GetComponent<ItemDragHandler>();

            if (!image.enabled)
            {
                // 슬롯 채우기
                image.enabled = true;
                image.sprite  = e.Item.Image;

                // 드래그 핸들러에 참조 주입
                drag.Item           = e.Item;
                drag.inventory      = Inventory;
                drag.inventoryPanel = inventoryPanelRT;

                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null) drag.canvas = canvas;

                break;
            }
        }
    }

    private void OnItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanelTransform = transform.Find("InventoryPanel");
        if (inventoryPanelTransform == null) return;

        foreach (Transform slot in inventoryPanelTransform)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler drag = imageTransform.GetComponent<ItemDragHandler>();

            // 같은 인스턴스인지 참조 비교
            if (drag != null && object.ReferenceEquals(drag.Item, e.Item))
            {
                image.enabled = false;
                image.sprite  = null;
                drag.Item     = null;
                break;
            }
        }
    }
}
