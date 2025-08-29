using UnityEngine;
using UnityEngine.UI;

public class LockInteraction : MonoBehaviour
{
    public GameObject lockPanel; // UI�� �ִ� LockPanel ����
    private bool isNear = false;

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.Z))
        {
            lockPanel.SetActive(true);
            ClearSky.Player.isControlBlocked = true;
        }

        if (lockPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab))
        {
            lockPanel.SetActive(false);
            ClearSky.Player.isControlBlocked = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isNear = false;
    }
}