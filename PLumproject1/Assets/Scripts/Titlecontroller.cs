using UnityEngine;
using UnityEngine.SceneManagement;
public class Titlecontroller : MonoBehaviour
{
    public void OnClickStart()
    {
        // GameFlags, MonologueManager, Inventory가 DontDestroyOnLoad로 이전 플레이 상태를
        // 그대로 들고 있을 수 있으므로 새 게임 시작 전에 전부 초기화
        GameFlags.Clear();

        var monologueManager = FindAnyObjectByType<MonologueManager>();
        if (monologueManager != null) monologueManager.ResetSeenEvents();

        var player = FindAnyObjectByType<ClearSky.Player>();
        if (player != null)
        {
            if (player.inventory != null) player.inventory.ClearForLoad();

            // Player는 DontDestroyOnLoad 싱글톤이라 Start()가 다시 호출되지 않으므로
            // 나가기 전 위치에 그대로 남아있음 - 새 게임 시작 시 직접 초기 위치로 되돌림
            player.GoingPointName = string.Empty;
            player.transform.position = player.spawnPosition;
        }

        SceneManager.LoadScene("Prologue");
    }
    public void OnClickExit()
    {
        Application.Quit();

    }
    public void OnClickContinue()
    {
        Debug.Log("이어하기 구현안됨");

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
