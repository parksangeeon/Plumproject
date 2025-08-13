using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Axe : MonoBehaviour, IInventoryItem
{
    public string Name => "Axe";
    public Sprite _Image;
    public Sprite Image => _Image;

    public void OnPickup()
    {
        gameObject.SetActive(false);
    }

    public void OnDrop()
    {
        // 필요시 월드 드롭 로직
    }

    public void OnUse()   // ← 추가
    {
        Debug.Log("Axe used!");
        // 소모품처럼 효과만 내고 끝내도 되고,
        // 필요하면 여기서 파티클/사운드 등 처리 가능
    }
}

