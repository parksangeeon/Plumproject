using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private static PauseController instance;
    public GameObject pauseButton;

    // 세이브/로드
    public SaveLoadPanel saveLoadPanel;
    public Button saveButton;
    public Button loadButton;

    void Start()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (saveButton) saveButton.onClick.AddListener(OpenSave);
        if (loadButton) loadButton.onClick.AddListener(OpenLoad);
    }

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TITLE")
        {
            if (pauseMenuUI) pauseMenuUI.SetActive(false);
            if (pauseButton) pauseButton.SetActive(false);
            this.enabled = false;
            return;
        }
        else
        {
            if (pauseButton) pauseButton.SetActive(true);
            this.enabled = true;
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
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Pause()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TITLE");
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
    }
    public void OpenOptions() { Debug.Log("옵션 열기"); }

    // 버튼이 SaveGame()을 가리키고 있다면 이 메서드 하나만 남기세요.
    public void SaveGame() => OpenSave();

    public void OpenSave()
    {
        Pause();
        if (saveLoadPanel) saveLoadPanel.Open(SaveLoadMode.Save);
    }
    public void OpenLoad()
    {
        Pause();
        if (saveLoadPanel) saveLoadPanel.Open(SaveLoadMode.Load);
    }
}
