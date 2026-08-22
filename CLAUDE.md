# ARENA BREAK — 프로젝트 규약

> 이 파일은 Claude Code가 이 프로젝트에서 작업할 때 자동으로 읽는 컨텍스트다.
> **수업 중 이 파일을 함께 다듬는 것이 1장의 핵심 활동이다.** 규약이 바뀌면 여기부터 고친다.

---

## 프로젝트 개요

3D 1인칭 웨이브 방어 슈터. 아레나 하나에서 몰려오는 적을 처치하고 5웨이브 생존이 목표.

**코어 루프**: 웨이브 스폰 → 적 처치 → 웨이브 클리어 → 강화 선택 → 다음 웨이브

---

## 환경

- **Unity 6.3 LTS (6000.3.x)** — 이 버전 기준으로만 코드를 작성할 것
- **URP** (Universal Render Pipeline)
- **Input System (신규)** — 레거시 `Input.GetKey` 계열 사용 금지
- 대상 플랫폼: Windows Standalone
- **Unity AI Assistant 패키지는 이 프로젝트에 설치하지 않는다.** 설치를 제안하지 말 것
  (MCP for Unity와 `System.Collections.Immutable` 버전이 충돌한다)

---

## 코딩 규약

### 네이밍
- 네임스페이스: `ArenaBreak.Player` / `ArenaBreak.Enemy` / `ArenaBreak.Core` / `ArenaBreak.UI`
- private 필드: `_camelCase` + `[SerializeField]`
- **public 필드 금지** — 필요하면 프로퍼티를 쓸 것
- 클래스·메서드: `PascalCase`, 지역 변수·매개변수: `camelCase`

### 성능
- `Update()` 안에서 `GetComponent` / `Find` / `FindObjectOfType` 계열 호출 **금지**
  → `Awake()`에서 캐싱할 것
- `Update()` 안에서 문자열 연결(`+`) 금지
- 프레임마다 생성/파괴가 반복되는 오브젝트는 풀링 대상으로 표시할 것

### 설계
- **매직 넘버 금지** → `[SerializeField]` 또는 `ScriptableObject`로 노출
- UI는 폴링하지 말고 **C# event 구독**으로 갱신할 것
- 이벤트를 구독했으면 `OnDisable` / `OnDestroy`에서 **반드시 해제**할 것
- 하나의 클래스는 하나의 책임만 가진다. 애매하면 나에게 먼저 물어볼 것

### 주석
- 주석은 **"왜"만** 작성한다. "무엇"은 코드로 표현한다
- 자동 생성된 장황한 XML 주석 블록을 넣지 말 것

---

## 디렉터리 구조

```
Assets/
  Scripts/
    Player/     PlayerController, WeaponSystem
    Enemy/      EnemyAI, 적 변형
    Core/       GameManager, Health, IDamageable, WaveSpawner, WaveData
    UI/         HUDController
  Prefabs/      적, 이펙트, 스폰 포인트
  Scenes/       Arena.unity
  Data/         ScriptableObject 에셋 (웨이브 데이터 등)
  Materials/
  Audio/
```

새 스크립트는 반드시 위 폴더 중 하나에 배치한다. `Assets/` 루트에 두지 말 것.

---

## 작업 규칙 (중요)

1. **한 번에 파일 1~2개만 수정한다.** 여러 시스템을 한꺼번에 만들지 말 것
2. **씬 오브젝트를 임의로 생성·삭제하지 않는다.** 필요하면 먼저 물어보고, 허락 없이는 절차만 안내할 것
3. 코드를 작성한 뒤에는 **반드시 컴파일 에러를 확인**한다 (MCP의 콘솔 읽기 도구 사용)
4. 기존 코드가 있으면 **먼저 읽고 스타일을 맞춘다**
5. **확실하지 않은 API는 추측하지 말고 나에게 먼저 물어본다**
6. 리팩터링 시에는 **공개 API와 event 시그니처를 유지**하고, 변경 전후 차이를 요약해 설명할 것
7. 큰 기능은 구현 전에 **2~3가지 설계안을 장단점과 함께 제시**한다. 선택은 내가 한다

---

## MCP 사용 범위

| 작업 | 방식 |
|---|---|
| C# 스크립트 작성·수정 | **파일 직접 편집** (변경 내역이 보이도록) |
| 콘솔 에러 읽기, 컴파일 확인 | **MCP 도구 사용** |
| 씬 오브젝트 배치, 컴포넌트 연결 | **MCP 도구 사용** (단, 사전에 물어볼 것) |
| 프리팹·머티리얼 생성 | MCP 도구 사용 |
| 플레이 테스트 판정 | **사람이 한다. AI가 "잘 동작합니다"라고 단정하지 말 것** |

MCP 연결이 끊기면 파일 기반으로 폴백하고, 씬 작업은 절차만 안내할 것.

---

## 하지 말아야 할 것

- 요청하지 않은 기능을 "이것도 있으면 좋을 것 같아서" 추가하기
- 테스트하지 않은 코드를 "동작합니다"라고 단정하기
- 한 번에 5개 이상의 파일 생성하기
- `Assets/` 밖의 파일(ProjectSettings, Packages/manifest.json) 임의 수정하기
- Unity AI Assistant 관련 패키지 설치 제안하기
