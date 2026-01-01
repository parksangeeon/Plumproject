using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClearSky;

public enum SaveLoadMode { Save, Load }

public class SaveLoadPanel : MonoBehaviour
{
    [Header("Slots(4)")]
    public SaveSlotUI[] slots = new SaveSlotUI[SaveSystem.SlotCount];

    [Header("Confirm Save Dialog")]
    public GameObject overwriteDialog;
    public TMPro.TextMeshProUGUI dialogMessageText; // 다이얼로그 메시지 텍스트 (선택사항)
    public Button yesButton;
    public Button noButton;

    SaveLoadMode mode;
    int pendingSlot = -1;

    void Awake()
    {
        gameObject.SetActive(false);
        if (overwriteDialog) overwriteDialog.SetActive(false);

        if (yesButton) yesButton.onClick.AddListener(OnConfirmYes);
        if (noButton) noButton.onClick.AddListener(OnConfirmNo);
    }

    public void Open(SaveLoadMode m)
    {
        mode = m;
        Refresh();
        
        // 부모 GameObject들도 함께 활성화 (루트까지)
        Transform current = transform.parent;
        while (current != null)
        {
            if (!current.gameObject.activeSelf)
            {
                current.gameObject.SetActive(true);
            }
            current = current.parent;
        }
        
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }
            
            var sum = SaveSystem.GetSummary(i);
            slots[i].Bind(i, sum, OnSlotClicked);
        }
    }

    void OnSlotClicked(int index)
    {
        if (mode == SaveLoadMode.Save)
        {
            // 저장 모드: 무조건 확인 다이얼로그 표시
            pendingSlot = index;
            
            // 다이얼로그 메시지 설정
            if (dialogMessageText != null)
            {
                bool exists = SaveSystem.Exists(index);
                dialogMessageText.text = exists 
                    ? $"슬롯 {index + 1}에 이미 저장된 데이터가 있습니다.\n덮어쓰시겠습니까?" 
                    : $"슬롯 {index + 1}에 저장하시겠습니까?";
            }
            
            if (overwriteDialog) overwriteDialog.SetActive(true);
        }
        else // Load
        {
            if (SaveSystem.Exists(index))
                StartCoroutine(DoLoad(index));
        }
    }

    void OnConfirmYes()
    {
        if (pendingSlot >= 0) DoSave(pendingSlot);
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    void OnConfirmNo()
    {
        if (overwriteDialog) overwriteDialog.SetActive(false);
        pendingSlot = -1;
    }

    void DoSave(int slot)
    {
        var player = FindObjectOfType<Player>();
        var bridge = FindObjectOfType<SaveLoadBridge>();
        SaveSystem.Save(slot, player, bridge != null ? bridge.ExportItems : null);
        Refresh();
    }

    IEnumerator DoLoad(int slot)
    {
        yield return SaveSystem.Load(slot);
        Close();
    }
}
