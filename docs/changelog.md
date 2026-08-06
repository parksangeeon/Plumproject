# 코드 변경 내역

## SceneEntrance + SpawnPoint 시스템 (StartingPoint 대체)

### 신규: `SceneEntrance.cs`

씬 간 연결을 enum으로 정의. 방향 없이 연결 이름으로 표현.

```csharp
public enum SceneEntrance
{
    None          = -1,
    Hall1_Room1_1 =  0,  // 1층 홀 ↔ 1-1
    Hall1_Room1_2 =  1,  // 1층 홀 ↔ 1-2
    Hall1_Hall2   =  2,  // 1층 홀 ↔ 2층 홀
}
```

`SceneEntranceHelper.GetTargetScene(entrance)` — 현재 씬 이름 기준으로 이동할 씬 자동 계산.

### 신규: `SpawnPoint.cs`

도착 씬의 문 오브젝트에 부착. `entranceId`가 `Player.pendingEntrance`와 일치하면 플레이어 위치 적용.

- `yield return null` 1프레임 대기 후 위치 적용 (씬 초기화 물리 충돌 방지)
- `rb.position`과 `transform.position` 동시 설정 (Dynamic Rigidbody2D 물리 좌표 동기화)
- 적용 후 `pendingEntrance = None` 초기화

### 수정: `Player.cs`

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| 씬 전환 식별자 | `public string GoingPointName` | `public SceneEntrance pendingEntrance = SceneEntrance.None` |
| Start() 복제본 방지 | 없음 | `if (instance != this) return;` |
| 초기 위치 설정 | `FindWithTag("Player").transform.position = spawnPosition` | `pendingEntrance == None`일 때만 설정, `rb.position`도 함께 동기화 |

복제본 방지 이유: 씬에 Player 프리팹이 배치된 경우 `Awake()`에서 `Destroy` 호출해도 `Start()`는 실행됨. 복제본의 `Start()`가 DontDestroyOnLoad 플레이어 위치를 `spawnPosition`으로 덮어쓰는 버그 차단.

### 수정: `Door.cs` (DoorTrigger)

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| 이동 씬 지정 | `public string targetSceneName` | 삭제 (`SceneEntranceHelper`가 자동 계산) |
| 입구 식별자 | `public string destinationPointName` | `public SceneEntrance destinationEntrance` |
| 이동 차단 조건 | GameFlags 문자열 방식 | `public EnemyFindZone requiredZone` (직접 참조) |

추가 필드:
- `requiredZone`: 완료되어야 통과 가능한 EnemyFindZone (null이면 조건 없음)
- `blockingTalkData` / `blockingProgress`: 차단 시 출력할 대사

### 삭제

- `StartingPoint.cs` 삭제 (SpawnPoint로 대체)

---

## EnemyFindZone 시네마틱 1회성 처리

### 수정: `eyeenemyroom.cs` (EnemyFindZone)

**완료 상태 영속화 (GameFlags)**

씬 재로드 시 `hasTriggered`/`IsCompleted` 초기화 방지.

```csharp
private string FlagKey => $"zone_{gameObject.scene.name}_{gameObject.name}";
// Inspector 입력 없이 씬 이름 + 오브젝트 이름으로 자동 생성

void Start()
{
    if (GameFlags.GetBool(FlagKey))
    {
        hasTriggered = true;
        IsCompleted = true; // 시네마틱 재실행 없이 완료 상태 복원
    }
}
```

시네마틱 완료 시 `GameFlags.Set(FlagKey, true)` 저장.

**DialogueFinished 람다 구독 해제 (누수 수정)**

```csharp
// 변경 전 — 해제 안 됨
monologueManager.DialogueFinished += () => finished1 = true;

// 변경 후 — 사용 후 자동 해제
System.Action onDone = null;
onDone = () => { finished1 = true; monologueManager.DialogueFinished -= onDone; };
monologueManager.DialogueFinished += onDone;
```

누수 시 다른 대사 종료 이벤트에 MoveScene이 이중 호출되는 버그 차단.

**StartTalk 반환값 검사**

```csharp
bool started = monologueManager.StartTalk(talkData, progress);
if (!started)
    monologueManager.DialogueFinished -= onDone; // 대기 없이 진행
else
    yield return new WaitUntil(() => finished1);
```

`StartTalk`가 false 반환 시(이미 본 대사 또는 없는 progress) `WaitUntil` 무한 대기 방지.

---

## TalkData 관리 원칙

- `TalkData` 에셋은 `Assets/dialogue/` 폴더에 씬별로 보관
- `DoorTrigger`와 `EnemyFindZone`이 같은 TalkData 에셋을 공유할 경우 **progress 값이 겹치면 안 됨**
  - 같은 `(TalkData, progress)` 쌍을 두 곳에서 쓰면 MonologueManager의 `seenEvents`가 한쪽을 건너뜀
- Inspector에서 `Assets/dialogue/씬명D` 에셋 클릭 → Inspector에서 편집 (텍스트 에디터 직접 편집 금지)

### 1-1 씬 TalkData 구성 예시 (1-1D.asset)

| progress | 담당 컴포넌트 | 비고 |
|---|---|---|
| 1 | 1-hall DoorTrigger | 문 진입 전 대사 |
| 2 | EnemyFindZone (progress=2) | 시네마틱 1번째 대사 |
| 3 | EnemyFindZone (progress+1=3) | 시네마틱 2번째 대사 |
