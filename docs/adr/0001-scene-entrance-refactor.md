# ADR 0001 — 씬 전환 시 플레이어 스폰 위치 리팩터링

- **날짜**: 2026-08-05
- **상태**: 적용됨

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

## 변경 내용

### 신규: `SceneEntrance.cs`

```csharp
public enum SceneEntrance
{
    None     = -1,
    FromHall =  0,
    From1_1  =  1,
    From1_2  =  2,
    // 새 씬 추가 시 여기에 항목 추가
}
```

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

- 각 씬에 입구 수만큼 빈 GameObject에 부착
- Inspector에서 `Entrance Id` 드롭다운으로 선택
- 자기 ID가 `Player.pendingEntrance`와 일치하면 플레이어를 해당 위치로 이동 후 초기화

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
// MoveScene() 내부
thePlayer.GoingPointName = destinationPointName;

// After
public SceneEntrance destinationEntrance;
// MoveScene() 내부
thePlayer.pendingEntrance = destinationEntrance;
```

---

### 삭제

- `StartingPoint.cs` 삭제
- 각 씬에서 StartingPoint GameObject 삭제 (Inspector 작업)

---

## Inspector 작업 (씬별)

1. 플레이어가 나타날 위치에 빈 GameObject 배치
2. `SpawnPoint` 컴포넌트 부착 → `Entrance Id` 드롭다운에서 해당 입구 선택
3. 각 `DoorTrigger`의 `Destination Entrance` 드롭다운에서 목적지 입구 선택

---

## 검증

- `dotnet build` 에러 0개 확인
- 2-hall → 1-1 이동 시 지정한 스폰 위치에 등장하는지 확인
- 1-1 → 2-hall 역방향 이동도 확인
