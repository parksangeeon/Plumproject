using UnityEngine;
using System.Collections;

public class StartingPoint : MonoBehaviour
{
    public string startPoint;
    private ClearSky.Player thePlayer;

    IEnumerator Start()
    {
        // 1 ������ ��ٷ��� �� �� ������Ʈ�� ��� �ʱ�ȭ�ǵ��� ��
        yield return null;

        thePlayer = FindAnyObjectByType<ClearSky.Player>();
        if (thePlayer == null)
        {
            Debug.LogWarning("Player�� ã�� �� �����ϴ�!");
            yield break;
        }

        if (startPoint == thePlayer.currentMapName)
        {
            thePlayer.transform.position = transform.position;
            Debug.Log($"StartingPoint ���� �Ϸ�: {startPoint}");
        }
        else
        {
            Debug.Log($"���� �� �̸��� ��ġ���� ����: {thePlayer.currentMapName} != {startPoint}");
        }
    }
}