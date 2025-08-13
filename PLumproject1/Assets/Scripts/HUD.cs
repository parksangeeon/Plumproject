using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    
    public Inventory Inventory;
    

    void Start()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;

    }

    public void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
{
    Debug.Log("d");

    // InventoryPanel 찾기 (최상단에!)
    Transform inventoryPanelTransform = transform.Find("InventoryPanel");
    RectTransform inventoryPanelRT = inventoryPanelTransform as RectTransform;

    foreach (Transform slot in inventoryPanelTransform)
    {
        // Border -> (자식) -> ItemImage 구조라고 가정
        Transform imageTransform = slot.GetChild(0).GetChild(0);
        Image image = imageTransform.GetComponent<Image>();
        ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

        // 빈 슬롯이면 여기서 세팅
        if (!image.enabled)
        {
            image.enabled = true;
            image.sprite = e.Item.Image;

            // 여기서 참조 주입 (선언 후 사용!)
            itemDragHandler.Item = e.Item;
            itemDragHandler.inventory = Inventory;
            itemDragHandler.inventoryPanel = inventoryPanelRT;

            // (선택) 드래그 중 레이캐스트 문제 방지용 Canvas 참조까지 넘기고 싶다면:
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) itemDragHandler.canvas = canvas;

            break;
        }
    }
}


    public void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Debug.Log("HUD에 있는 코드"); //영상에는 넣으라고 해서 코드는 적었으나, 딱히 우리 게임에는 필요없어서 주석처리
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            //UI에서 아이템을 찾았을 때
            if (itemDragHandler.Item.Equals(e.Item))
            {
                image.enabled = false;
                image.sprite = null;
                itemDragHandler.Item = null;
                break;
            }
        }
    }
}
