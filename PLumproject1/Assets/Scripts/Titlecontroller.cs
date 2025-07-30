using UnityEngine;
using UnityEngine.SceneManagement;
public class Titlecontroller : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("Intro");
    }
    public void OnClickExit()
    {
        Application.Quit();

    }
    public void OnClickContinue()
    {
        Debug.Log("저장기능 미구현");
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
