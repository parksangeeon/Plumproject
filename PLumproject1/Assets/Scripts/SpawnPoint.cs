using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SceneEntrance entranceId;

    void Start()
    {
        var player = FindAnyObjectByType<ClearSky.Player>();
        if (player != null && player.pendingEntrance == entranceId)
            StartCoroutine(ApplySpawn(player));
    }

    IEnumerator ApplySpawn(ClearSky.Player player)
    {
        yield return null; // 씬 초기화 완료 후 적용
        Vector2 pos = transform.position;
        player.transform.position = pos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = pos;          // 물리 엔진 내부 좌표도 동기화
            rb.linearVelocity = Vector2.zero;
        }
        player.pendingEntrance = SceneEntrance.None;
    }
}
