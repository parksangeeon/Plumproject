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
        Debug.Log("������ �̱���");
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
