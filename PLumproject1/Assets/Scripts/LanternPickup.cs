using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternPickup : MonoBehaviour
{
    public GameObject lanternLight;
    private Light2D lightComponent;
    public TalkData talkData;
    public MonologueManager monologueManager;
    public Inventory inventory;
    public GameObject mapLight;
    private Light2D mapLight2D;
    private bool isPlayerInZone = false;
    private bool lanternAcquired = false;

    [Header("Flag Name")]
    public string flagLanternPickedUp = "lantern_picked_up";

    void Start()
    {
        lightComponent = lanternLight.GetComponent<Light2D>();
        mapLight2D = mapLight.GetComponent<Light2D>();
        RestoreState();
    }

    // SaveGameLoader가 플래그 복원 후 호출 (public으로 변경)
    public void OnLoadGame()
    {
        // 월드 오브젝트는 상태만 복원 (인벤토리 동기화는 SaveGameLoader에서 처리)
        RestoreState();
    }

    void RestoreState()
    {
        // 플래그에서 상태 복원
        lanternAcquired = GameFlags.GetBool(flagLanternPickedUp, false);

        if (lanternAcquired)
        {
            // 이미 획득한 상태면 랜턴 오브젝트 비활성화
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            // 획득하지 않은 상태면 랜턴 오브젝트 활성화
            if (gameObject != null)
            {
                // 부모 오브젝트들을 활성화 (루트까지)
                Transform current = transform.parent;
                while (current != null)
                {
                    if (!current.gameObject.activeSelf)
                    {
                        current.gameObject.SetActive(true);
                    }
                    current = current.parent;
                }
                
                // 자신도 활성화
                gameObject.SetActive(true);
            }
            isPlayerInZone = false;
        }
    }

    void Update()
    {
        if (isPlayerInZone && !lanternAcquired && Input.GetKeyDown(KeyCode.Z))
        {
            monologueManager.StartTalk(talkData, 3);
            lanternLight.SetActive(true);
            lanternAcquired = true;
            GameFlags.Set(flagLanternPickedUp, true);

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
            monologueManager.StartTalk(talkData, 2);
            monologueManager.DialogueFinished += Ondialoguefinished;
        }
        
    }
    void Ondialoguefinished()
    {
        ClearSky.Player.isControlBlocked = false;
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }
}
