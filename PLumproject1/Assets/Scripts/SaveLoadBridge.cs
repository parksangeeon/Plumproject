using System.Collections.Generic;
using UnityEngine;
using ClearSky; // Player


public class SaveLoadBridge : MonoBehaviour
{
[Header("Refs")]
public Player player; // 인스펙터에서 플레이어 할당
public ItemDatabase itemDatabase; // 아이템 ID ↔ 프리팹 매핑(ScriptableObject)
public Transform spawnParent; // 인스턴스 생성 위치(빈 오브젝트 추천)


// === 저장용: 인벤토리 → 문자열 ID 목록 ===
public List<string> ExportItems()
{
if (player == null || player.inventory == null) return new List<string>();
return player.inventory.GetItemIds(); // Inventory.cs에 아래 패치 추가됨
}


// === 로드용: 문자열 ID 목록 → 인벤토리 재구성 ===
public void ImportItems(List<string> ids)
{
if (player == null || player.inventory == null) return;
var inv = player.inventory;


inv.ClearForLoad(); // UI 정리 + 내부 리스트 비우기(월드 드롭 없음)


if (ids == null || ids.Count == 0) return;
foreach (var id in ids)
{
var item = itemDatabase != null ? itemDatabase.InstantiateById(id, spawnParent) : null;
if (item == null)
{
Debug.LogWarning($"[SaveLoadBridge] Item prefab not found for id='{id}'");
continue;
}


// 월드에 잠깐 보이지 않도록 즉시 비활성화 후 AddItem 호출
var mb = item as MonoBehaviour;
if (mb != null) mb.gameObject.SetActive(false);


inv.AddItem(item); // AddItem이 collider 비활성 + OnPickup()까지 호출
}
}
}