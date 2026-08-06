# 버그 수정: 1-1 시네마틱 후 1-hall 스폰 위치 초기화 문제

## 증상

- 1-1 시네마틱(EnemyFindZone)이 처음 발동된 뒤 1-hall로 이동하면 플레이어가 문 앞이 아닌 바닥 초기 위치에 스폰됨
- 시네마틱이 이미 완료된 상태(재방문)에서는 정상적으로 문 앞에 스폰됨

## 원인

`EnemyRevealSequence` 마지막에 두 번째 대사를 시작하고 코루틴이 종료됨:

```csharp
monologueManager.StartTalk(talkData, progress + 1); // 대사 시작 후 코루틴 끝
```

`MonologueManager`는 DontDestroyOnLoad 싱글톤이라 씬이 바뀌어도 대사가 계속 실행됨.
이 상태에서 플레이어가 문을 통해 1-hall로 이동하면:

1. `isControlBlocked = true` (대사 진행 중) 상태로 1-hall 로드
2. `SpawnPoint`가 플레이어를 문 위치에 스폰하고 `rb.linearVelocity = 0` 설정
3. 그러나 `isControlBlocked = true` → `Player.Update()`의 `Run()` 미실행
4. 게임 코드로 위치를 잡아주지 않는 상태에서 Rigidbody2D 중력이 작동
5. 플레이어가 바닥으로 낙하 → 초기값처럼 보이는 위치에 착지

재방문 시에는 시네마틱이 실행되지 않아 `isControlBlocked = false` 상태로 씬 전환 → 정상 스폰.

## 수정

`Door.cs` — `MoveScene()`에서 씬 전환 시 `isControlBlocked` 강제 해제:

```csharp
private void MoveScene()
{
    monologueManager.DialogueFinished -= MoveScene;
    ClearSky.Player.isControlBlocked = false; // 진행 중인 시네마틱이 있어도 이동 시 강제 해제

    thePlayer.pendingEntrance = destinationEntrance;
    StartCoroutine(ChangeSceneWithDelay());
}
```

## 수정 파일

- `PLumproject1/Assets/Scripts/Door.cs`
