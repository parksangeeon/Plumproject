using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button slotButton;
    public TextMeshProUGUI slotNumberText;
    public TextMeshProUGUI saveTimeText;
    public TextMeshProUGUI sceneNameText;
    public Image thumbnailImage; // 썸네일 이미지 (선택사항)
    public GameObject emptySlotText;
    public GameObject savedDataPanel;

    private int slotIndex;
    private SaveData saveData;
    private SaveLoadMenu parentMenu;

    public void Initialize(int index, SaveLoadMenu menu)
    {
        slotIndex = index;
        parentMenu = menu;
        
        if (slotNumberText != null)
        {
            slotNumberText.text = $"슬롯 {index + 1}";
        }
        
        // 버튼 클릭 이벤트
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }

        // 저장 데이터 로드 및 표시
        LoadSlotData();
    }

    public void LoadSlotData()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem.Instance가 null입니다!");
            emptySlotText.SetActive(true);
            savedDataPanel.SetActive(false);
            return;
        }

        saveData = SaveSystem.Instance.GetSaveData(slotIndex);
        
        if (saveData != null && !saveData.isEmpty)
        {
            // 저장된 데이터가 있는 경우
            if (emptySlotText != null)
            {
                emptySlotText.SetActive(false);
            }
            if (savedDataPanel != null)
            {
                savedDataPanel.SetActive(true);
            }
            
            if (saveTimeText != null)
            {
                saveTimeText.text = saveData.saveTimeString;
            }
            if (sceneNameText != null)
            {
                sceneNameText.text = $"씬: {saveData.sceneName}";
            }
        }
        else
        {
            // 빈 슬롯
            if (emptySlotText != null)
            {
                emptySlotText.SetActive(true);
            }
            if (savedDataPanel != null)
            {
                savedDataPanel.SetActive(false);
            }
            if (saveTimeText != null)
            {
                saveTimeText.text = "";
            }
            if (sceneNameText != null)
            {
                sceneNameText.text = "";
            }
        }
    }

    private void OnSlotClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.OnSlotSelected(slotIndex, saveData);
        }
    }

    public void Refresh()
    {
        LoadSlotData();
    }
}

