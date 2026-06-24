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

    public void Bind(int index, SaveSummary summary, System.Action<int> onClick)
    {
        slotIndex = index;
        
        if (slotNumberText != null)
        {
            slotNumberText.text = $"슬롯 {index + 1}";
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClick?.Invoke(index));
        }

        if (summary == null || summary.isEmpty)
        {
            // 빈 슬롯
            if (emptySlotText != null) emptySlotText.SetActive(true);
            if (savedDataPanel != null) savedDataPanel.SetActive(false);
            if (saveTimeText != null) saveTimeText.text = "";
            if (sceneNameText != null) sceneNameText.text = "";
        }
        else
        {
            // 저장된 데이터
            if (emptySlotText != null) emptySlotText.SetActive(false);
            if (savedDataPanel != null) savedDataPanel.SetActive(true);
            if (saveTimeText != null) saveTimeText.text = summary.saveTime;
            if (sceneNameText != null) sceneNameText.text = $"씬: {summary.sceneName}";
        }
    }
}

