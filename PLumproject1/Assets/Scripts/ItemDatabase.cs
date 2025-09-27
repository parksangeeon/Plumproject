using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
[Tooltip("IInventoryItem 컴포넌트를 가진 프리팹들을 등록하세요. Name 문자열이 저장 ID가 됩니다.")]
public List<GameObject> itemPrefabs = new();


public IInventoryItem InstantiateById(string id, Transform parent = null)
{
if (string.IsNullOrEmpty(id)) return null;


foreach (var p in itemPrefabs)
{
if (p == null) continue;
var comp = p.GetComponent<IInventoryItem>();
if (comp != null && comp.Name == id)
{
var go = Instantiate(p, parent);
return go.GetComponent<IInventoryItem>();
}
}


Debug.LogWarning($"[ItemDatabase] No prefab matches id='{id}'");
return null;
}
}