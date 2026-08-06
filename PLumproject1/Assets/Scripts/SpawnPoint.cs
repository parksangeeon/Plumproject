using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SceneEntrance entranceId;

    void Start()
    {
        var player = ClearSky.Player.Instance;
        if (player != null && player.pendingEntrance == entranceId)
            StartCoroutine(ApplySpawn(player));
    }

    IEnumerator ApplySpawn(ClearSky.Player player)
    {
        yield return null; // frame 1: 씬 초기화 완료 후 플레이어 위치 설정
        Vector2 pos = transform.position;
        player.transform.position = pos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = pos;
            rb.linearVelocity = Vector2.zero;
        }
        player.pendingEntrance = SceneEntrance.None;

        yield return null; // frame 2: CinemachineCamera를 스폰 위치로 즉시 스냅
        var brain = Camera.main?.GetComponent<CinemachineBrain>();
        var vcam = brain?.ActiveVirtualCamera as CinemachineCamera;
        if (vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.ForceCameraPosition(pos, brain.transform.rotation);
        }
    }
}
