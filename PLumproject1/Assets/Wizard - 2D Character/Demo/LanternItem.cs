using UnityEngine;

public class LanternItem : MonoBehaviour, IInventoryItem
{
    public string Name => "Lantern";

    [SerializeField] private Sprite image;
    public Sprite Image => image;

    public void OnPickup()
    {
        // �ð� ȿ�� ���� ���⿡ �߰��� �� ����
        gameObject.SetActive(false); // ���忡�� ����
    }

    public void OnDrop()
    {
        Debug.Log("������ ����� �� �����ϴ�.");
    }
}