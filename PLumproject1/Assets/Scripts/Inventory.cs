using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 4;

    private List<IInventoryItem> mItems = new List<IInventoryItem>();

    public event EventHandler<InventoryEventArgs> ItemAdded;

    public event EventHandler<InventoryEventArgs> ItemRemoved;
    
    // Inventory.cs
    public event EventHandler<InventoryEventArgs> ItemUsed; // 필요하면 UI용으로도 쓸 수 있음

    public void UseItem(IInventoryItem item)
    {
        if (!mItems.Contains(item)) return;

        mItems.Remove(item);

        // 사용 로직 (아이템별 동작)
        item.OnUse();

        // UI에서 아이템 비우기 (지금은 ItemRemoved만 써도 HUD가 지워줌)
        ItemRemoved?.Invoke(this, new InventoryEventArgs(item));
        ItemUsed?.Invoke(this, new InventoryEventArgs(item)); // 선택 사항
    }



    public void AddItem(IInventoryItem item)
    {
        Debug.Log("그럼 인건 됨???");

        if (mItems.Count < SLOTS)
        {
            Collider2D collider = (item as MonoBehaviour).GetComponent<Collider2D>();
            if (collider.enabled)
            {
                collider.enabled = false;

                mItems.Add(item);

                item.OnPickup();

                if (ItemAdded != null)
                {
                    ItemAdded(this, new InventoryEventArgs(item));
                }
            }
        }
    }
    


    // Inventory.cs
    public void RemoveItem(IInventoryItem item)
    {
        Debug.Log("[Inventory] RemoveItem");

        if (!mItems.Contains(item))
        {
            Debug.LogWarning("[Inventory] mItems에 아이템이 없음 (참조 불일치?)");
            return;
        }

        mItems.Remove(item);
        item.OnDrop();

        var mb = item as MonoBehaviour;
        var col2d = mb != null ? mb.GetComponent<Collider2D>() : null;
        if (col2d) col2d.enabled = true;

        ItemRemoved?.Invoke(this, new InventoryEventArgs(item));
    }



}


