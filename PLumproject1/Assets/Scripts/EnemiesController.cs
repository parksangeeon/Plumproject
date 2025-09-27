using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [Header("Movement")]
    public float approachSpeed = 2f;                          // 왼쪽 이동 속도(유닛/초)
    public Vector3 defaultPosition = new Vector3(40f, 0f, 0f);// ESC/종료 시 돌아갈 좌표

    [Header("Debug")]
    public bool forceChase = false; // 디버그용: 강제로 항상 전진

    private bool chasing = false;

    private void Update()
    {
        if (forceChase || chasing)
        {
            // timeScale=0이어도 움직이게 unscaledDeltaTime 사용
            transform.position += Vector3.left * (approachSpeed * Time.unscaledDeltaTime);
        }
    }

    public void StartChaseLeft()
    {
        chasing = true;
        Debug.Log("[Enemy] StartChaseLeft()");
    }

    public void ResetToDefault()
    {
        chasing = false;
        transform.position = defaultPosition;
        Debug.Log($"[Enemy] ResetToDefault() → pos={transform.position}");
    }
}
