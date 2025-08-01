using UnityEngine;
using UnityEngine.UI;

public class InteractionImage : MonoBehaviour
{
    public GameObject imagePanel;      // �г� ��ü (Ȱ��/��Ȱ����)
    public Image imageDisplay;         // �̹��� ������Ʈ
    public Sprite[] imageOptions;      // ���� ��������Ʈ�� ���� ���� ����

    private void Start()
    {
        imagePanel.SetActive(false);   // ó������ ���α�
    }

    public void ShowImage(int index)
    {
        if (index >= 0 && index < imageOptions.Length)
        {
            imageDisplay.sprite = imageOptions[index];
            imagePanel.SetActive(true);
        }
    }

    public void HideImage()
    {
        imagePanel.SetActive(false);
    }
}