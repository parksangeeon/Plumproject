using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("왼쪽으로 이동 속도(유닛/초)")]
    public float approachSpeed = 2f;

    [Tooltip("퍼즐 종료/ESC 시 돌아갈 위치")]
    public Vector3 defaultPosition = new Vector3(40f, 0f, 0f);

    [Header("Debug")]
    public bool debugLogs = true;
    [Tooltip("체크 시 항상 전진(디버그용)")]
    public bool forceChase = false;

    private bool chasing = false;

    void OnEnable()
    {
        if (debugLogs) Debug.Log($"[Enemy] OnEnable on '{name}' pos={transform.position}");
    }

    void Update()
    {
        bool doChase = forceChase || chasing;

        if (doChase)
        {
            // timeScale=0이어도 움직이게 unscaledDeltaTime 사용
            float dx = approachSpeed * Time.unscaledDeltaTime;
            transform.position += Vector3.left * dx;
        }
    }

    [ContextMenu("DEBUG/StartChaseLeft")]
    public void StartChaseLeft()
    {
        chasing = true;
        if (debugLogs) Debug.Log($"[Enemy] StartChaseLeft() on '{name}'");
    }

    [ContextMenu("DEBUG/StopChase")]
    public void StopChase()
    {
        chasing = false;
        if (debugLogs) Debug.Log($"[Enemy] StopChase() on '{name}'");
    }

    [ContextMenu("DEBUG/ResetToDefault")]
    public void ResetToDefault()
    {
        chasing = false;
        transform.position = defaultPosition;
        if (debugLogs) Debug.Log($"[Enemy] ResetToDefault() → pos={transform.position} on '{name}'");
    }
}
