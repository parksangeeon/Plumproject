using UnityEngine;
using UnityEngine.UI;

public class InteractionImage : MonoBehaviour
{
    public GameObject imagePanel;      // 패널 전체 (활성/비활성용)
    public Image imageDisplay;         // 이미지 컴포넌트
    public Sprite[] imageOptions;      // 여러 스프라이트를 넣을 수도 있음

    private void Start()
    {
        imagePanel.SetActive(false);   // 처음에는 꺼두기
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