using System;
using System.Linq;
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

    public List<string> GetItemIds()
    {
        var list = new List<string>();
        foreach (var it in mItems)
        {
            if (it != null) list.Add(it.Name); // Name을 저장 ID로 사용
        }
        return list;
    }

    /// <summary>
    /// 현재 인벤토리의 모든 아이템 반환 (읽기 전용)
    /// </summary>
    public IReadOnlyList<IInventoryItem> GetAllItems()
    {
        return mItems.AsReadOnly();
    }

    /// <summary>
    /// 이름으로 아이템 찾기
    /// </summary>
    public IInventoryItem FindItemByName(string itemName)
    {
        return mItems.Find(item => item != null && item.Name == itemName);
    }


    /// <summary>
    /// 로드 전용 클리어: 월드에 드롭하지 않고 내부/UI만 정리
    /// </summary>
    public void ClearForLoad()
    {
        // UI 비우기 알림
        var copy = new List<IInventoryItem>(mItems);
        foreach (var it in copy)
        {
            ItemRemoved?.Invoke(this, new InventoryEventArgs(it));
        }
        mItems.Clear();
    }




}


