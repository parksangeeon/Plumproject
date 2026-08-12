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

## GameFlag SO 도입 — 시네마틱 완료 후 문 대사 건너뛰기

### 신규: `GameFlag.cs`

ScriptableObject 기반 bool 플래그. Inspector에서 에셋을 드래그로 연결하므로 문자열 없음.

```csharp
[CreateAssetMenu(menuName = "Game/GameFlag")]
public class GameFlag : ScriptableObject
{
    private bool _value;
    public bool Value => _value;

    public void Set(bool value)
    {
        _value = value;
        GameFlags.Set(name, value); // 세이브 시스템(GameFlags)에도 동기화
    }

    public void RestoreFromSave()
    {
        _value = GameFlags.GetBool(name, false); // 로드 후 복원용
    }
}
```

SO 에셋의 `name`(파일명)이 GameFlags 키로 사용되므로 세이브/로드와 자동 연동됨.

### 수정: `eyeenemyroom.cs` (EnemyFindZone)

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| 완료 상태 저장 | `private string FlagKey` (자동 생성 문자열) | `public GameFlag completionFlag` (에셋 참조) |
| Start() 복원 | `GameFlags.GetBool(FlagKey)` | `completionFlag.RestoreFromSave()` → `completionFlag.Value` |
| 완료 시 저장 | `GameFlags.Set(FlagKey, true)` | `completionFlag?.Set(true)` |

### 수정: `Door.cs` (DoorTrigger)

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| 대사 건너뛰기 조건 | `public string skipTalkIfFlagSet` (문자열 직접 입력) | `public GameFlag skipTalkIf` (에셋 참조) |

DoorTrigger가 다른 씬의 EnemyFindZone을 직접 참조할 수 없으므로, 같은 GameFlag 에셋을 공유하는 방식으로 크로스씬 통신. 세이브 로드 시 SO 값이 초기화될 수 있으므로 `skipTalkIf.Value || GameFlags.GetBool(skipTalkIf.name)` 둘 다 체크.

### Inspector 작업

1. Project 창에서 `Create → Game → GameFlag` → 이름 설정 (예: `Zone_1-1_EnemyFound`)
2. 1-1 씬 EnemyFindZone → `Completion Flag`에 에셋 드래그
3. 1-hall 씬 DoorTrigger → `Skip Talk If`에 **같은** 에셋 드래그

---

## 씬 전환 시 카메라 즉시 스냅

### 수정: `SpawnPoint.cs`

**문제**: `CameraFollowSetter`(CameraManager.cs)가 2프레임 후에 Follow를 설정하는 사이, Cinemachine이 씬 초기 위치에서 플레이어 위치로 이동하는 게 화면에 보였다.

**수정**: `ApplySpawn` 코루틴에 두 번째 `yield return null`을 추가해, 플레이어 위치 설정 다음 프레임에 활성 `CinemachineCamera`를 스폰 위치로 즉시 스냅.

```csharp
yield return null; // frame 2: CinemachineCamera를 스폰 위치로 즉시 스냅
var brain = Camera.main?.GetComponent<CinemachineBrain>();
var vcam = brain?.ActiveVirtualCamera as CinemachineCamera;
if (vcam != null)
{
    vcam.Follow = player.transform;
    vcam.ForceCameraPosition(pos, brain.transform.rotation);
}
```

`Follow`도 직접 설정해 `CameraFollowSetter`의 실행 순서에 무관하게 동작한다. `ICinemachineCamera`에는 `Follow`가 없으므로 `CinemachineCamera`로 캐스팅 필요.

---

## FindAnyObjectByType → Player.Instance 교체 (스폰 위치 버그)

### 원인

씬에 Player 프리팹이 배치된 경우 `FindAnyObjectByType<ClearSky.Player>()`가
DontDestroyOnLoad 플레이어 대신 복제본을 반환할 수 있다.

- **DoorTrigger**: 복제본에 `pendingEntrance = Hall1_Room1_2`를 설정 → DontDestroyOnLoad 플레이어는 None 유지 → 1-hall 도착 시 SpawnPoint 조건 불일치 → 초기 위치에 스폰
- **SpawnPoint**: 복제본의 pendingEntrance(None)를 읽어 스폰 조건 실패

### 수정: `Player.cs`

```csharp
public static Player Instance => instance;
```

싱글톤 인스턴스를 외부에서 참조할 수 있도록 공개.

### 수정: `Door.cs` / `SpawnPoint.cs`

| 파일 | 변경 전 | 변경 후 |
|---|---|---|
| Door.cs | `FindAnyObjectByType<ClearSky.Player>()` | `ClearSky.Player.Instance` |
| SpawnPoint.cs | `FindAnyObjectByType<ClearSky.Player>()` | `ClearSky.Player.Instance` |

---

## 일기 상호작용 구현 (diary.cs 개선)

### 수정: `diary.cs`

기존 코드에 `isControlBlocked` 처리와 `DialogueFinished` 구독이 없어 읽는 중 플레이어가 움직이고 조작이 복구되지 않는 문제 수정.

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| progress | 하드코딩 `0` | `public int progress` (Inspector 설정) |
| 조작 잠금 | 없음 | StartTalk 성공 시 `isControlBlocked = true` |
| 조작 복구 | 없음 | `DialogueFinished` 구독 → `OnReadFinished`에서 해제 |
| 구독 누수 | 없음 | 람다 대신 메서드 참조로 자동 해제 |

```csharp
void Update()
{
    if (isPlayerNear && Input.GetKeyDown(KeyCode.Z))
    {
        bool started = monologueManager.StartTalk(talkData, progress);
        if (started)
        {
            ClearSky.Player.isControlBlocked = true;
            monologueManager.DialogueFinished += OnReadFinished;
        }
    }
}

void OnReadFinished()
{
    ClearSky.Player.isControlBlocked = false;
    monologueManager.DialogueFinished -= OnReadFinished;
}
```

### Inspector 작업

- `Assets/dialogue/` 에 TalkData 에셋(`1-2DiaryD`) 생성
- progress 0에 일기 내용을 scripts[]로 구성 (`isRepeatable: true`)
- 1-2 씬 일기 GameObject에 Collider2D(IsTrigger) + `diary` 컴포넌트 추가 후 할당

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
