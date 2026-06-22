using UnityEngine;
using System.Collections;
using ClearSky;  // 네임스페이스 맞게 가져오기

public class StartingPoint : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        StartCoroutine(SetPlayerPositionDelayed());
    }

    IEnumerator SetPlayerPositionDelayed()
    {
        // 1 프레임 기다리기
        yield return null;

        ClearSky.Player player = FindAnyObjectByType<ClearSky.Player>();
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            yield break;
        }

        string pointName = player.GoingPointName;

        foreach (Transform point in spawnPoints)
        {
            if (point.name == pointName)
            {
                player.transform.position = point.position;
                Debug.Log($"플레이어를 {pointName} 위치로 이동시켰습니다.");
                yield break;
            }
        }

        Debug.LogWarning($"이름이 {pointName}인 스폰 포인트를 찾지 못했습니다.");
    }
}