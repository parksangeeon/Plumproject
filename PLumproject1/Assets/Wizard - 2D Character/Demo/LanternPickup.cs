using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternPickup : MonoBehaviour
{
  
    public GameObject lanternLight;
    private Light2D lightComponent;
    public MonologueManager monologueManager;
    public Inventory inventory; // <- �ν����� ����

    private bool isPlayerInZone = false;
    private bool lanternAcquired = false;

    void Start()
    {
        lightComponent = lanternLight.GetComponent<Light2D>();
    }

    void Update()
    {
        if (isPlayerInZone && !lanternAcquired && Input.GetKeyDown(KeyCode.Z))
        {
  
            lanternLight.SetActive(true);
            lanternAcquired = true;

            // �κ��丮�� �߰�
            IInventoryItem item = GetComponent<IInventoryItem>();
            if (item != null)
            { 
                inventory.AddItem(item);
                item.OnPickup();
                lightComponent.intensity = 1.0f;
                lightComponent.pointLightOuterRadius = 5f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !lanternAcquired)
        {
            
            isPlayerInZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            isPlayerInZone = false;
        }
    }
}