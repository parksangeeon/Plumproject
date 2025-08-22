using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string targetSceneName;        // �̵��� �� �̸�
    public string destinationPointName;   // ������ StartPoint �̸�
    private bool canEnter = false;
    private ClearSky.Player thePlayer;
    private static bool justEntered = false;
    public MonologueManager monologueManager;

    void Start()
    {
        thePlayer = FindAnyObjectByType<ClearSky.Player>();
        Invoke("ClearJustEntered", 0.2f);
    }

    void ClearJustEntered()
    {
        justEntered = false;
    }

    void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.DownArrow) && !justEntered)
        {

            if (targetSceneName == "Prologue")
            {
                monologueManager.SetLines(new List<string> {
                    "���� ����... ����?",
                    "�ٸ� ���� ã�ƺ��߰ھ�."
                });
                return;
            }
            thePlayer.currentMapName = destinationPointName;
            justEntered = true;
            SceneManager.LoadScene(targetSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }
}