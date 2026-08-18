using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public TalkData approachTalkData;
    public TalkData pickupTalkData;
    public MonologueManager monologueManager;
    public Inventory inventory;
    public GameFlag acquiredFlag;

    private bool isPlayerInZone = false;
    private bool keyAcquired = false;

    void Start()
    {
        RestoreState();
    }

    public void OnLoadGame()
    {
        RestoreState();
    }

    void RestoreState()
    {
        if (acquiredFlag != null) acquiredFlag.RestoreFromSave();
        keyAcquired = acquiredFlag != null && acquiredFlag.Value;
        if (keyAcquired) gameObject.SetActive(false);
    }

    void Update()
    {
        if (ClearSky.Player.isControlBlocked) return;
        if (isPlayerInZone && !keyAcquired && Input.GetKeyDown(KeyCode.Z))
        {
            keyAcquired = true;
            acquiredFlag?.Set(true);

            IInventoryItem item = GetComponent<IInventoryItem>();
            Debug.Log($"[KeyPickup] item={item}, inventory InstanceID={inventory?.GetInstanceID()}");
            if (item != null) inventory.AddItem(item);
            else Debug.LogWarning("[KeyPickup] IInventoryItem 컴포넌트를 찾지 못했습니다.");

            if (pickupTalkData != null)
                monologueManager.StartTalk(pickupTalkData, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !keyAcquired)
        {
            isPlayerInZone = true;
            if (approachTalkData != null)
                monologueManager.StartTalk(approachTalkData, 0);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInZone = false;
    }
}
