# ADR 0001 — 씬 전환 시 플레이어 스폰 위치 리팩터링

| 버전 | 날짜 | 상태 |
|------|------|------|
| v1 | 2026-08-05 | 적용됨 |
| v2 | 2026-08-06 | Inspector 방식 수정, targetSceneName 제거 |
| v3 | 2026-08-06 | Hall1_Hall2 연결 추가, 중복 키 버그 수정 |
| v4 | 2026-08-06 | 스폰 위치 미적용 버그 수정 |

---

## Context

`StartingPoint.cs`가 문자열 이름으로 스폰 포인트를 찾는 구조였다.
`DoorTrigger.destinationPointName`과 씬 내 GameObject 이름이 한 글자라도 다르면
`"이름이 X인 스폰 포인트를 찾지 못했습니다"` 경고와 함께 플레이어가 엉뚱한 위치에 스폰되는 버그가 반복됐다.

`Player`가 이미 `DontDestroyOnLoad` 싱글톤이므로,
Player에 "어떤 입구로 들어왔는지" ID(`SceneEntrance` enum)만 들고 다니게 하면
도착 씬의 `SpawnPoint`가 자기 ID와 맞춰서 플레이어를 직접 이동시킬 수 있다.

**문자열 없음 · 타입 안전 · Inspector 드롭다운 선택**

---

## 코드 변경 내용 (v1 · 2026-08-05)

### 신규: `SceneEntrance.cs`

```csharp
public enum SceneEntrance
{
    None          = -1,
    Hall1_Room1_1 =  0,  // 1층 홀 ↔ 1-1
    Hall1_Room1_2 =  1,  // 1층 홀 ↔ 1-2
    Hall1_Hall2   =  2,  // 1층 홀 ↔ 2층 홀
    // 새 연결 추가 시 enum 항목 + connections 딕셔너리에 한 줄 추가
}
```

> **v2 네이밍 변천**:
> - `From...` 시도 → `DoorTrigger.destinationEntrance = From1Hall`처럼 읽혀 어색
> - `To...` 시도 → DoorTrigger/SpawnPoint가 같은 값을 공유해야 하는데 To/From을 분리하면 매칭 불가
> - **최종: 연결 이름** — 방향 개념 제거. 두 문이 같은 enum 값 하나를 공유하며, 읽는 사람도 어느 씬 사이 연결인지 바로 알 수 있음

씬/입구가 늘어나면 enum 항목만 추가하면 된다.

---

### 신규: `SpawnPoint.cs` — `StartingPoint.cs` 대체

```csharp
public class SpawnPoint : MonoBehaviour
{
    public SceneEntrance entranceId;

    void Start()
    {
        var player = FindAnyObjectByType<ClearSky.Player>();
        if (player != null && player.pendingEntrance == entranceId)
        {
            player.transform.position = transform.position;
            player.pendingEntrance = SceneEntrance.None;
        }
    }
}
```

자기 ID가 `Player.pendingEntrance`와 일치하면 플레이어를 해당 위치로 이동 후 초기화.

---

### 수정: `Player.cs`

```csharp
// Before
public string GoingPointName;

// After
public SceneEntrance pendingEntrance = SceneEntrance.None;
```

---

### 수정: `Door.cs`

```csharp
// Before
public string destinationPointName;
thePlayer.GoingPointName = destinationPointName;

// After
public SceneEntrance destinationEntrance;
thePlayer.pendingEntrance = destinationEntrance;
```

---

### 삭제

- `StartingPoint.cs` 삭제
- 각 씬에서 StartingPoint GameObject 삭제 (Inspector 작업)

---

## Inspector 작업 방식 변경 (v2 · 2026-08-06)

### 원래 계획 (v1)

> 각 씬에 플레이어가 나타날 위치에 **빈 GameObject를 따로 만들고**
> `SpawnPoint` 컴포넌트를 부착한 뒤 `Entrance Id`를 선택하는 방식.

### 수정된 방식 (v2)

**별도 GameObject 불필요 — 도착 씬의 문(Door) GameObject에 직접 SpawnPoint를 부착.**

도착 씬의 문이 곧 입구이므로, 그 문 오브젝트가 스폰 포인트 역할을 겸한다.
문 위치에 플레이어가 스폰되더라도 `DoorTrigger`의 `justEntered` 플래그가
0.2초간 재진입을 차단하므로 즉시 재발동 문제는 없다.

**수정 이유**: 씬마다 빈 오브젝트를 별도로 관리하는 것보다
기존 문 오브젝트를 재활용하는 것이 씬 구조가 단순해지고 실수가 줄어든다.

### 최종 Inspector 작업 순서

1. 도착 씬의 **문 GameObject** 선택
2. `SpawnPoint` 컴포넌트 추가 → `Entrance Id` 드롭다운에서 해당 입구 선택
3. 출발 씬의 `DoorTrigger` → `Destination Entrance` 드롭다운에서 목적지 입구 선택

---

---

## 스폰 위치 미적용 버그 수정 (v4 · 2026-08-06)

**증상**: 문을 통해 씬을 이동하면 플레이어가 문 위치가 아닌 에디터에서 임의로 설정해둔 위치(주로 (0,0))로 이동함.

**원인 1 — 중복 Player.Start() 실행**

씬에 Player 프리팹이 배치된 경우, 중복 Player의 `Awake()`에서 `Destroy(gameObject)`를 호출해도
Unity는 같은 프레임 내에 `Start()`를 실행한다.
이 `Start()`에서 `FindWithTag("Player")`로 DontDestroyOnLoad 플레이어를 찾아
`player.transform.position = spawnPosition`으로 위치를 덮어쓴다.

**수정**: `Player.Start()` 첫 줄에 `if (instance != this) return;` 추가.

**원인 2 — Dynamic Rigidbody2D 물리 좌표 불일치**

Dynamic Rigidbody2D는 `transform.position`을 직접 쓰면 다음 FixedUpdate에서
물리 엔진 내부 좌표로 되돌아갈 수 있다.

**수정**: `SpawnPoint.ApplySpawn()`에서 `transform.position`과 함께 `rb.position`도 설정.

---

## 검증

- `dotnet build` 에러 0개 확인
- 2-hall → 1-1 이동 시 1-1의 문 위치에 플레이어가 스폰되는지 확인
- 1-1 → 2-hall 역방향 이동도 확인
