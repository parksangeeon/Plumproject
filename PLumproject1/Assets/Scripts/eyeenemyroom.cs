using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyFindZone : MonoBehaviour
{
    public MonologueManager monologueManager;
    public TalkData talkData;
    public int progress = 0;

    public CinemachineCamera playerCam; // 플레이어 따라가는 카메라
    public CinemachineCamera enemyCam;  // 적 고정 카메라

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(EnemyRevealSequence());
        }
    }

    private IEnumerator EnemyRevealSequence()
    {
        // 1. 플레이어 조작 막기
        ClearSky.Player.isControlBlocked = true;

        // 2. 카메라를 적에게 전환
        enemyCam.Priority = 20;
        playerCam.Priority = 10;

        yield return new WaitForSeconds(0.5f); // 카메라 이동 텀

        // 3. 적 대사 출력
        monologueManager.StartTalk(talkData, progress);

        // 4. 대사 끝날 때까지 기다리기
        bool finished = false;
        monologueManager.DialogueFinished += () => finished = true;
        yield return new WaitUntil(() => finished);

        // 5. 카메라 다시 플레이어에게 전환
        playerCam.Priority = 20;
        enemyCam.Priority = 10;
        monologueManager.StartTalk(talkData, progress+1);

        // 6. 플레이어 조작 해제
        ClearSky.Player.isControlBlocked = false;
    }
}