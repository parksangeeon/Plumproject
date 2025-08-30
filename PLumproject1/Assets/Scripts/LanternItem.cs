using UnityEngine;

public class LanternItem : MonoBehaviour, IInventoryItem
{
    public string Name => "Lantern";

    [SerializeField] private Sprite image;
    public Sprite Image => image;
    
    public void OnUse()
    {

    }
    public void OnPickup()
    {
        
        gameObject.SetActive(false); 
    }

    public void OnDrop()
    {
        Debug.Log("������ ����� �� �����ϴ�.");
    }
}