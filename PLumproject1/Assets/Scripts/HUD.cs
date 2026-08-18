using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Inventory Inventory;
    
    void Start()
    {
        if (Inventory != null)
        {
            Inventory.ItemAdded += OnItemAdded;
            Inventory.ItemRemoved += OnItemRemoved;
            Debug.Log($"[HUD] Inventory 구독 완료 (InstanceID={Inventory.GetInstanceID()})");
        }
        else
        {
            Debug.LogWarning("[HUD] Start: Inventory가 할당되지 않아 이벤트를 구독하지 못했습니다.");
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
        Debug.Log($"[HUD] OnItemAdded 호출됨: {e.Item?.Name}");
        Transform inventoryPanelTransform = transform.Find("InventoryPanel");
        if (inventoryPanelTransform == null)
        {
            Debug.LogError("[HUD] 'InventoryPanel'을 찾지 못했습니다.");
            return;
        }
        RectTransform inventoryPanelRT = inventoryPanelTransform as RectTransform;
        var canvas = GetComponentInParent<Canvas>();

        bool filled = false;
        foreach (Transform slot in inventoryPanelTransform)
        {
            ItemDragHandler drag = slot.GetComponentInChildren<ItemDragHandler>();
            if (drag == null) continue;

            if (drag.Item == null)
            {
                Image image = drag.GetComponent<Image>();
                if (image != null)
                {
                    image.enabled = true;
                    image.sprite  = e.Item.Image;
                }
                drag.Item           = e.Item;
                drag.inventory      = Inventory;
                drag.inventoryPanel = inventoryPanelRT;
                if (canvas != null) drag.canvas = canvas;

                filled = true;
                break;
            }
        }

        if (!filled)
            Debug.LogWarning($"[HUD] OnItemAdded: '{e.Item?.Name}'을 넣을 빈 슬롯 없음.");
    }

    private void OnItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanelTransform = transform.Find("InventoryPanel");
        if (inventoryPanelTransform == null) return;

        foreach (Transform slot in inventoryPanelTransform)
        {
            ItemDragHandler drag = slot.GetComponentInChildren<ItemDragHandler>();
            if (drag != null && object.ReferenceEquals(drag.Item, e.Item))
            {
                Image image = drag.GetComponent<Image>();
                if (image != null)
                {
                    image.enabled = false;
                    image.sprite  = null;
                }
                drag.Item = null;
                break;
            }
        }
    }
}
