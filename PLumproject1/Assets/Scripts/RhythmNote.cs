using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RhythmNote : MonoBehaviour, IPointerClickHandler
{
    [Header("Refs")]
    public Image baseCircle;     // 본체(안쪽 원) Image (없으면 자동 GetComponent<Image>())
    public Image approachRing;   // 바깥 흰색 링(자식 Image; Raycast Target 꺼두기)

    [Header("Visual")]
    public float startScale = 2.2f;  // 시작할 때 링의 스케일(기본 원 대비 배율)
    public Color hitTint = new Color(0.6f, 1f, 0.6f, 1f);

    public int Index { get; private set; }
    public float ScheduledTime { get; private set; }

    private RhythmPuzzleManager mgr;
    private RectTransform ringRT;
    private RectTransform baseRT;
    private bool isHit = false;

    public void Setup(RhythmPuzzleManager manager, int index, float scheduledTime)
    {
        mgr = manager;
        Index = index;
        ScheduledTime = scheduledTime;

        if (baseCircle == null) baseCircle = GetComponent<Image>();
        if (baseCircle != null) baseCircle.raycastTarget = true;

        ringRT = approachRing ? approachRing.rectTransform : null;
        baseRT = (RectTransform)transform;

        // 초기 상태: 링이 크게 시작해서 수축해 들어옴
        if (ringRT != null)
        {
            // 링의 피벗/앵커가 중앙(0.5,0.5)이어야 정확
            ringRT.localScale = Vector3.one * startScale;
        }
        isHit = false;

        // 숫자/순서가 필요하면 여기서 Text 붙여서 (index+1) 표시 가능
    }

    private void Update()
    {
        if (isHit || mgr == null || !mgr.InProgress) return;

        // 링 수축 애니메이션: (ScheduledTime - ApproachTime) ~ ScheduledTime 구간에서 startScale → 1.0
        if (ringRT != null)
        {
            float start = ScheduledTime - mgr.ApproachTime;
            float t = Mathf.InverseLerp(start, ScheduledTime, mgr.Now); // 0→1
            float scale = Mathf.Lerp(startScale, 1f, Mathf.Clamp01(t));
            ringRT.localScale = new Vector3(scale, scale, 1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHit) return;
        mgr?.TryHit(this); // 타이밍 판정은 매니저가 수행
    }

    public void MarkHit()
    {
        isHit = true;
        if (baseCircle != null) baseCircle.color = hitTint;
        if (approachRing != null) approachRing.enabled = false; // 링 숨김
    }
}
