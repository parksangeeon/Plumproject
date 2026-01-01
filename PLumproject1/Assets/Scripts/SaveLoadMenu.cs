using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadMenu : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject menuPanel;
    public SaveSlotUI[] saveSlots; // 4개의 슬롯
    public Button closeButton;
    public Button backButton;
    public TextMeshProUGUI menuTitleText; // "저장" 또는 "불러오기" 텍스트

    [Header("Mode")]
    public bool isSaveMode = true; // true: 저장 모드, false: 로드 모드

    private PauseController pauseController;

    void Start()
    {
        pauseController = FindFirstObjectByType<PauseController>();
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseMenu);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // 슬롯 초기화
        InitializeSlots();
    }

    public void OpenMenu(bool saveMode)
    {
        isSaveMode = saveMode;
        menuPanel.SetActive(true);
        
        // 메뉴 제목 변경
        if (menuTitleText != null)
        {
            menuTitleText.text = saveMode ? "저장" : "불러오기";
        }
        
        RefreshAllSlots();
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    private void OnBackButtonClicked()
    {
        CloseMenu();
        if (pauseController != null)
        {
            pauseController.ReturnToMainMenu();
        }
    }

    private void InitializeSlots()
    {
        if (saveSlots == null || saveSlots.Length != 4)
        {
            Debug.LogError("SaveLoadMenu: saveSlots 배열이 4개여야 합니다!");
            return;
        }

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null)
            {
                saveSlots[i].Initialize(i, this);
            }
        }
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in saveSlots)
        {
            if (slot != null)
            {
                slot.Refresh();
            }
        }
    }

    public void OnSlotSelected(int slotIndex, SaveData saveData)
    {
        if (isSaveMode)
        {
            // 저장 모드
            SaveToSlot(slotIndex);
        }
        else
        {
            // 로드 모드
            if (saveData != null && !saveData.isEmpty)
            {
                LoadFromSlot(slotIndex);
            }
            else
            {
                Debug.Log($"슬롯 {slotIndex + 1}에는 저장된 데이터가 없습니다.");
                // 빈 슬롯 클릭 시 메시지 표시 (선택사항)
            }
        }
    }

    private void SaveToSlot(int slotIndex)
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem.Instance가 null입니다!");
            return;
        }

        SaveSystem.Instance.SaveGame(slotIndex);
        Debug.Log($"슬롯 {slotIndex + 1}에 게임이 저장되었습니다.");
        
        // 슬롯 UI 새로고침
        if (saveSlots[slotIndex] != null)
        {
            saveSlots[slotIndex].Refresh();
        }

        // 저장 후 메뉴 닫기
        StartCoroutine(CloseMenuAfterDelay(0.5f));
    }
    
    private IEnumerator CloseMenuAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        CloseMenu();
        
        // 메인 메뉴로 돌아가기
        if (pauseController != null)
        {
            pauseController.ReturnToMainMenu();
        }
    }

    private void LoadFromSlot(int slotIndex)
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem.Instance가 null입니다!");
            return;
        }

        SaveData saveData = SaveSystem.Instance.LoadGame(slotIndex);
        
        if (saveData != null && !saveData.isEmpty)
        {
            // 일시정지 해제
            if (pauseController != null)
            {
                pauseController.Resume();
            }

            // 게임 로드
            SaveSystem.Instance.ApplySaveData(saveData);
            
            Debug.Log($"슬롯 {slotIndex + 1}에서 게임을 불러왔습니다.");
        }
        else
        {
            Debug.LogError($"슬롯 {slotIndex + 1}에서 게임을 불러올 수 없습니다.");
        }
    }
}

