using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternPickup : MonoBehaviour
{
  
    public GameObject lanternLight;
    private Light2D lightComponent;
    public MonologueManager monologueManager;
    public Inventory inventory; // <- 인스펙터 연결
    public GameObject mapLight;
    private Light2D mapLight2D;
    private bool isPlayerInZone = false;
    private bool lanternAcquired = false;

    void Start()
    {
        lightComponent = lanternLight.GetComponent<Light2D>();
        mapLight2D = mapLight.GetComponent<Light2D>();
    }

    void Update()
    {
        if (isPlayerInZone && !lanternAcquired && Input.GetKeyDown(KeyCode.Z))
        {
  
            lanternLight.SetActive(true);
            lanternAcquired = true;

            // 인벤토리에 추가
            IInventoryItem item = GetComponent<IInventoryItem>();
            if (item != null)
            { 
                inventory.AddItem(item);
                item.OnPickup();
                mapLight2D.intensity = 0.02f;
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