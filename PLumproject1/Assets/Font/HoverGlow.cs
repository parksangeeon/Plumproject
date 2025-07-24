using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material defaultMat;
    public Material glowMat;
    private TMP_Text tmpText;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmpText.fontMaterial = glowMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmpText.fontMaterial = defaultMat;
    }
}