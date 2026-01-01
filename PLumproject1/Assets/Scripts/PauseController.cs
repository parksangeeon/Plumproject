using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private static PauseController instance;
    public GameObject pauseButton;
    
    [Header("Save/Load Menu")]
    public SaveLoadPanel saveLoadPanel; // SaveLoadPanel 사용
    public SaveLoadMenu saveLoadMenu; // SaveLoadMenu (선택사항, 둘 중 하나만 사용)
    public GameObject mainMenuPanel; // Resume, Save, Option, Exit 버튼이 있는 메인 패널
    
    [Header("Buttons")]
    public Button saveButton;
    public Button loadButton;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (saveButton) saveButton.onClick.AddListener(OpenSave);
        if (loadButton) loadButton.onClick.AddListener(OpenLoad);
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TITLE")
        {
            pauseMenuUI.SetActive(false); // 타이틀 씬에서는 일시정지 UI 비활성화
            pauseButton.SetActive(false); 
            this.enabled = false;         // 이 컨트롤러 비활성화
            return;
        }
        else
        {
            this.enabled = true;          // 다른 씬에서는 활성화
        }
    }

    void Update()
    {
        if (!this.enabled) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume(); else Pause();
        }
    }

    public void OnPauseButtonClicked() { if (!isPaused) Pause(); }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (saveLoadPanel != null)
        {
            saveLoadPanel.Close();
        }
        if (saveLoadMenu != null)
        {
            saveLoadMenu.CloseMenu();
        }
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        if (saveLoadPanel != null)
        {
            saveLoadPanel.Close();
        }
        if (saveLoadMenu != null)
        {
            saveLoadMenu.CloseMenu();
        }
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TITLE");
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
    }
    public void OpenOptions()
    {
        Debug.Log("옵션 열기");
    }
    public void SaveGame()
    {
        // 메인 메뉴 숨기고 세이브 메뉴 표시
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        
        if (saveLoadPanel != null)
        {
            saveLoadPanel.Open(SaveLoadMode.Save);
        }
        else if (saveLoadMenu != null)
        {
            saveLoadMenu.OpenMenu(true); // 저장 모드
        }
        else
        {
            Debug.LogError("SaveLoadPanel 또는 SaveLoadMenu가 할당되지 않았습니다!");
        }
    }

    public void LoadGame()
    {
        // 메인 메뉴 숨기고 로드 메뉴 표시
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
        
        if (saveLoadPanel != null)
        {
            saveLoadPanel.Open(SaveLoadMode.Load);
        }
        else if (saveLoadMenu != null)
        {
            saveLoadMenu.OpenMenu(false); // 로드 모드
        }
        else
        {
            Debug.LogError("SaveLoadPanel 또는 SaveLoadMenu가 할당되지 않았습니다!");
        }
    }

    public void ReturnToMainMenu()
    {
        // 세이브/로드 메뉴에서 메인 메뉴로 돌아가기
        if (saveLoadPanel != null)
        {
            saveLoadPanel.Close();
        }
        if (saveLoadMenu != null)
        {
            saveLoadMenu.CloseMenu();
        }
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void OpenSave()
    {
        SaveGame();
    }

    public void OpenLoad()
    {
        LoadGame();
    }
}
