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
        yield return null; // 2������ ��� (�÷��̾� ���� ��ٸ���)

        var player = FindFirstObjectByType<ClearSky.Player>();
        var cam = FindFirstObjectByType<CinemachineCamera>();

        if (player != null && cam != null)
        {
            cam.Follow = player.transform;  // ���� ����
            Debug.Log(" find");
        }
        else
        {
            Debug.LogError(" can't find");
        }
    }
}