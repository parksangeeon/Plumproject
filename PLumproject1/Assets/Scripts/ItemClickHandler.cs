using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{

    public Inventory _Inventory;

    
    public void OnItemClicked()
    {
        ItemDragHandler dragHandler = GetComponentInChildren<ItemDragHandler>();
        if (dragHandler == null) return;

        IInventoryItem item = dragHandler.Item;
        if (item == null) return;

        Debug.Log(item.Name);
    }
    
}
