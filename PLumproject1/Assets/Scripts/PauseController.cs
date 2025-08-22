using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private static PauseController instance;
    public GameObject pauseButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TITLE")
        {
            pauseMenuUI.SetActive(false); // 타이틀 씬에서는 퍼즈 UI 꺼두기
            pauseButton.SetActive(false); 
            this.enabled = false;         // 아예 스크립트 비활성화
            return;
        }
        else
        {
            this.enabled = true;          // 인게임 씬에서는 활성화
        }
    }
    void Update()
    {
        if (!this.enabled) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
                
            }
            else
                Pause();
        }
    }
    
    public void OnPauseButtonClicked()
    {
        if (!isPaused)
        {
            Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TITLE");
        pauseMenuUI.SetActive(false);
    }
    public void OpenOptions()
    {
        Debug.Log("옵션 열기");
    }
    public void SaveGame()
    {
        Debug.Log("게임저장");
    }
}
