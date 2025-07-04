using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowSetter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedFollowAssign());
    }

    IEnumerator DelayedFollowAssign()
    {
        yield return null;
        yield return null; // 2프레임 대기 (플레이어 생성 기다리기)

        var player = FindFirstObjectByType<ClearSky.Player>();
        var cam = FindFirstObjectByType<CinemachineCamera>();

        if (player != null && cam != null)
        {
            cam.Follow = player.transform;  // 여기 수정
            Debug.Log(" find");
        }
        else
        {
            Debug.LogError(" can't find");
        }
    }
}