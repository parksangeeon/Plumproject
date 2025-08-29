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
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ���� ����
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
            pauseMenuUI.SetActive(false); // Ÿ��Ʋ �������� ���� UI ���α�
            pauseButton.SetActive(false); 
            this.enabled = false;         // �ƿ� ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }
        else
        {
            this.enabled = true;          // �ΰ��� �������� Ȱ��ȭ
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
        Debug.Log("�ɼ� ����");
    }
    public void SaveGame()
    {
        Debug.Log("��������");
    }
}
