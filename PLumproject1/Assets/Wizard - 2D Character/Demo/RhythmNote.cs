using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RhythmNote : MonoBehaviour, IPointerClickHandler
{
    public int Index { get; private set; }
    public float ScheduledTime { get; private set; }

    private RhythmPuzzleManager manager;
    private Image img;
    private bool isHit = false;

    public void Setup(RhythmPuzzleManager mgr, int index, float scheduledTime)
    {
        manager = mgr;
        Index = index;
        ScheduledTime = scheduledTime;

        if (img == null) img = GetComponent<Image>();
        if (img != null)
        {
            img.raycastTarget = true;
            img.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHit) return;
        manager?.TryHit(this);
    }

    public void MarkHit()
    {
        isHit = true;
        if (img != null)
        {
            var c = img.color;
            c.a = 0.35f;   // 히트되면 반투명하게
            img.color = c;
        }
    }
}
