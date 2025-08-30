using UnityEngine;
using System.Collections;

public class StartingPoint : MonoBehaviour
{
    public string startPoint;
    private ClearSky.Player thePlayer;

    IEnumerator Start()
    {
        // 1 프레임 기다려서 씬 내 오브젝트가 모두 초기화되도록 함
        yield return null;

        thePlayer = FindAnyObjectByType<ClearSky.Player>();
        if (thePlayer == null)
        {
            Debug.LogWarning("Player를 찾을 수 없습니다!");
            yield break;
        }

        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = transform.position;
            Debug.Log($"StartingPoint 적용 완료: {startPoint}");
        }
        else
        {
            Debug.Log($"현재 맵 이름과 일치하지 않음: {thePlayer.currentMapName} != {startPoint}");
        }
    }
}