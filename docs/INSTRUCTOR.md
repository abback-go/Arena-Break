# 교수·조교용 운영 노트

> 이 문서는 학생에게 배포해도 무방하지만, 주 독자는 수업을 운영하는 사람입니다.

---

## 0. 한 학기 15주에서 이 문서가 놓이는 자리

`docs/01~03` 은 **장(章)** 단위이고, 학기에서는 **6~7주차 두 번의 수업**에 들어갑니다.
장 셋을 두 주에 압축하고, 남는 것은 과제로 돌립니다.

| 주 | 내용 |
|---|---|
| 1 | 강의소개 + 환경 구축 + 데모 시연 |
| 2 | Unity 기초 ① 컴포넌트 조립, 인스펙터 직렬화 |
| 3 | Unity 기초 ② 프레임 루프, 입력, 물리 |
| 4 | Unity 기초 ③ 이벤트 구독·해제, 프리팹 |
| 5 | Git & GitHub (GitMap 7주제 전부 + 짝 실습으로 충돌·PR) |
| 6 | **ARENA BREAK ① 쏘고 죽인다** — 1장 전부 + 2-2 체력/데미지 |
| 7 | **ARENA BREAK ② 몰려오고 끝난다** — 2장 나머지 + 3장 + AI 코드 리뷰 |
| 8 | 중간고사 |
| 9 | 팀 기획 확정 + 팀 리포 세팅 |
| 10 | 기획 발표 |
| 11~13 | 팀 개발 |
| 14 | 최종 발표 |
| 15 | 기말고사 |

성적: 중간 30 / 기말 30 / 출석 20 / 평소 20.

**두 주에 넣는 방법**

체력·데미지(2-2)를 1일차 끝으로 당깁니다. 쏴서 표적이 **죽는 것**까지 봐야 첫 수업이
게임으로 끝납니다. 2일차는 적 AI → 웨이브 → 상태 머신·HUD → 빌드로 달립니다.

문서에 있으나 수업 시간에 못 넣는 것은 과제입니다 — 각 장 끝의 과제, 3장 폴리시 절,
`docs/retrospective.md`.

**팀 프로젝트는 ARENA BREAK 확장을 필수로 거세요.** 실개발이 11~13주 세 주뿐이라
백지에서는 못 만듭니다. 씬·NavMesh·풀링·HUD가 이미 서 있으면 기능 두세 개는 붙습니다.

### 이번 학기에 바뀐 것

| 항목 | 내용 |
|---|---|
| 빌드 화면 | 전체 화면 → **창 모드 1280x720**. 스타터의 `ProjectSettings` 에 이미 들어 있습니다 |
| 플레이어 체력 | 100 → **500**. 적 공격이 10이라 50대를 버팁니다. 100이면 5웨이브를 볼 새가 없습니다 |
| `.claude/hooks` | `.ps1` 을 쓸 때 UTF-8 BOM을 자동으로 붙이는 훅. 없으면 PowerShell이 한글을 cp949로 읽어 깨집니다 |
| rescue 스크립트 | 점프 후 최신 도구·문서·빌드 설정을 되살립니다 (3-4 참고) |
| 문서 표기 | 「N주차」 → 「N장」. 학기 주차와 문서 장 번호가 더는 같지 않습니다 |

---

## 1. 스타터 리포 만드는 순서 (최초 1회)

이 스캐폴드는 **Unity 프로젝트 위에 덮어씌우는 껍데기**입니다.
Unity 프로젝트 본체(`ProjectSettings`, `Packages`, URP 에셋)는 Unity Hub가 생성합니다.

**이 장의 구조**

| 구간 | 하는 일 |
|---|---|
| 1-1 ~ 1-5 | 배포할 **내용물을 만든다** — 프로젝트 생성, 스캐폴드, 설정, 씬, MCP |
| 1-6 ~ 1-7 | 그 내용물을 **배포한다** — push, Template 설정 |

1-5까지 끝나야 배포할 것이 완성됩니다. 순서대로 진행하세요.

### Unity 6.3 기준 메뉴

인터넷 튜토리얼 다수가 Unity 5~2022 기준이라 아래 항목은 경로가 다릅니다.
이 문서는 Unity 6.3(6000.3) 공식 매뉴얼 기준이며, 근거는 「1-8. 검증 근거」에 있습니다.

| 하려는 것 | Unity 6.3 |
|---|---|
| 패키지 설치·확인 | `Window > Package Management > Package Manager` |
| NavMesh 굽기 | `NavMesh Surface` 컴포넌트를 붙이고 그 안의 `Bake` |
| 빌드 | `File > Build Profiles` |
| 입력 방식 | `Edit > Project Settings > Player > Other Settings > Active Input Handling` |

### 이 프로젝트에서 쓰는 고정 이름

문서·git 태그·프롬프트 카드가 모두 아래 표기를 전제로 합니다.

| 항목 | 표기 |
|---|---|
| 씬 파일 | `Assets/Scenes/Arena.unity` |
| 교수 리포 (원격) | `abback-go/Arena-Break` |
| 교수 리포 (로컬 폴더) | `arena-break-starter` |
| 학생 리포 | `arena-break` |
| MCP 버전 태그 | `v10.1.2` |

---

### 1-1. Unity Hub에서 프로젝트 생성

```
Unity Hub → New project
  Editor Version : 6000.3.x  (Unity 6.3 LTS)
  Template       : Universal 3D
  Project name   : arena-break-starter
```

URP 템플릿은 Hub/에디터 버전에 따라 **`Universal 3D`** 또는 **`3D (URP)`** 로 표시됩니다. 둘 다 같은 것입니다.
`3D (Built-In Render Pipeline)` 은 이 수업에서 쓰지 않습니다.

목록에 URP 템플릿이 없으면 에디터 설치 시 템플릿이 빠진 것입니다.
Hub → `Installs` → 해당 버전의 톱니바퀴 → `Add modules` 에서 추가합니다.

생성 후 첫 임포트가 끝날 때까지 기다립니다(3~10분).

### 1-2. 스캐폴드 덮어쓰기

zip을 풀면 `arena-break-starter/` 폴더가 한 겹 생깁니다.
Unity 프로젝트 루트에 복사할 것은 **그 폴더 안의 내용물**입니다.

```powershell
# Windows PowerShell — 경로는 실제 위치에 맞게
Copy-Item -Path "C:\Downloads\arena-break-starter\*" `
          -Destination "C:\Unity\arena-break-starter" -Recurse -Force
```

복사가 끝나면 프로젝트 루트가 이 상태입니다.

```
arena-break-starter/
  Assets/  Packages/  ProjectSettings/                 ← Unity가 만든 것
  CLAUDE.md  README.md  docs/  tools/  reference/      ← 스캐폴드
```

### 1-3. 기본 설정 확인

Unity로 프로젝트를 열고 세 가지를 확인합니다.

**(1) 콘솔**

```
Window > General > Console
```

빨간 에러 0개. 노란 경고는 그대로 두어도 됩니다.

**(2) 입력 방식**

```
Edit > Project Settings > Player > Other Settings > Active Input Handling
```

`Other Settings` 하위에 있어 스크롤이 필요합니다.
값은 새 Input System 항목으로 둡니다 — 에디터 버전에 따라
`Input System Package (New)` 또는 `New Input System` 으로 표기됩니다.

값을 바꾼 경우에만 에디터 재시작이 필요하며, Unity가 재시작 여부를 물어봅니다.

**(3) NavMesh 패키지**

```
Window > Package Management > Package Manager > In Project
```

**AI Navigation** 이 목록에 있어야 합니다. 2장 적 AI가 이 패키지를 씁니다.
없으면 `Unity Registry` 에서 설치합니다.

### 1-4. 아레나 씬 만들기

여섯 단계를 순서대로 진행합니다. 순서를 지켜야 하는 이유는 (5)에 적었습니다.

| 단계 | 내용 |
|---|---|
| (1) | 오브젝트 배치 |
| (2) | `Assets/Scenes/Arena.unity` 로 저장 |
| (3) | SampleScene 삭제 |
| (4) | 빌드 씬 목록 등록 |
| (5) | NavMesh 굽기 |
| (6) | 결과 확인 |

#### (1) 오브젝트 배치

`GameObject > 3D Object > ...` 와 `GameObject > Create Empty` 로 만듭니다.

| 오브젝트 | 만드는 법 | 설정 |
|---|---|---|
| `Floor` | 3D Object > Plane | Position (0,0,0), Scale (4, 1, 4) → 40×40m |
| `Wall_N/S/E/W` | 3D Object > Cube | 바닥 가장자리를 둘러싸게 (예: Scale (40, 4, 1)) |
| `SpawnPoints` | Create Empty | 자식으로 Create Empty 6개를 가장자리 안쪽에 |
| `Directional Light` | 기본 생성됨 | 그대로 |

Inspector의 `Static` 드롭다운은 사용하지 않습니다. NavMesh는 (5)에서 컴포넌트로 굽습니다.

#### (2) 씬 저장

```
File > Save As  →  Assets/Scenes/Arena.unity
```

저장 후 Project 창에 `Assets/Scenes/Arena` 로 표시됩니다.

다른 이름으로 저장했다면 Project 창에서 선택하고 **F2** 로 `Arena` 로 바꿉니다.
GUID가 유지되므로 참조는 그대로입니다.

#### (3) SampleScene 삭제

URP 템플릿이 만든 기본 씬입니다. 이 프로젝트가 쓰는 씬은 `Arena` 하나입니다.

```
Project 창에서 SampleScene.unity 와 SampleScene 폴더를 함께 선택 → Delete
```

`SampleScene` 폴더에는 라이팅 데이터가 들어 있습니다.
그 안에 `NavMesh-...` 로 시작하는 에셋이 보이면 (5)를 먼저 하고 (3)으로 돌아오세요 —
그 파일이 베이크 결과이고, 삭제하면 (5)를 다시 해야 합니다.

#### (4) 빌드 씬 목록 등록

빌드에는 이 목록에 있는 씬만 포함됩니다. SampleScene을 지웠으므로 `Arena` 를 등록합니다.

```
Arena 씬을 연 상태에서
File > Build Profiles → 플랫폼 프로필(Windows) 선택
  → Scene List 에서 [Add Open Scenes]
```

목록에 `Arena` 가 들어가면 됩니다.
커스텀 빌드 프로필을 새로 만든 경우에는 `Add Settings > Scene List` 로 항목을 먼저 추가합니다.
추가하지 않은 프로필은 전역(공유) 씬 목록을 씁니다.

#### (5) NavMesh 굽기

베이크된 NavMesh 에셋은 **베이크 시점의 씬 이름으로 된 폴더**에 저장됩니다.
그래서 (2) 씬 저장과 (3) SampleScene 삭제를 먼저 합니다.
이 순서대로면 에셋이 `Assets/Scenes/Arena/` 에 만들어집니다.

Unity 6의 `Static > Navigation Static` 은 비활성 상태입니다.
AI Navigation 2.x에서 Static 플래그 방식이 NavMesh Surface 컴포넌트 방식으로 대체되었고,
`Window > AI > Navigation` 창에는 Agents / Areas 탭만 있습니다.

```
1. Arena 씬이 열려 있는지 확인
2. Hierarchy에서 Floor 선택
3. Inspector > Add Component > Navigation > NavMesh Surface
4. 설정은 기본값 그대로
     Agent Type      : Humanoid
     Use Geometry    : Render Meshes
     Collect Objects : All Game Objects
5. 컴포넌트 하단의 [Bake] 클릭
```

바닥이 파랗게 덮이면 완료입니다.
벽(Cube)은 장애물로 인식되어 파란 영역에서 빠지고, `SpawnPoints` 는 메시가 없어 영향이 없습니다.

파란 표시를 그리는 것은 씬 뷰의 **AI Navigation 오버레이**입니다.
기본값은 켜짐이고 씬 뷰 우측 하단에 도킹됩니다. 표시가 없으면 오버레이 상태를 확인합니다.

```
씬 뷰에서 ` (백틱) 키  또는  씬 뷰 우측 상단의 ⋮ 버튼
  → 오버레이 목록에서 "AI Navigation"
  → AI Navigation 오버레이 > Surfaces > Show NavMesh
```

#### (6) 결과 확인

```
□ Assets/Scenes/Arena/ 폴더에 NavMesh-Floor 에셋이 있다
□ NavMesh Surface 의 NavMesh Data 가 그 에셋을 가리킨다 (None / Missing 이 아님)
□ 빌드 씬 목록에 Arena 가 있다
□ 씬 저장 (Ctrl+S)
```

NavMesh 에셋은 커밋 대상입니다. 이 파일이 없으면 학생 프로젝트에서 적이 이동하지 않습니다.

이 씬과 베이크는 스타터에 포함해 배포합니다.
1장 시간표(1-1 `CLAUDE.md` 다듬기 / 1-2 이동 / 1-3 사격)에는 아레나를 만드는 시간이 없고,
학생은 완성된 아레나 위에서 코드부터 시작합니다.

### 1-5. MCP for Unity 설치 및 버전 고정

```
Window > Package Management > Package Manager
  → 툴바의 Add (+) → Install package from git URL
```

```
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#v10.1.2
```

URL은 `?path=` 가 앞, `#리비전` 이 뒤입니다. 순서가 반대면 해석되지 않습니다.
태그는 `v` 를 포함한 `v10.1.2` 입니다.

`#main` 을 쓰면 업스트림 변경이 그대로 반영되어 학생마다 버전이 달라집니다. 태그로 고정합니다.
현재 태그 목록은 다음으로 확인합니다.

```bash
git ls-remote --tags --refs https://github.com/CoplayDev/unity-mcp.git | tail -5
```

리허설에서 검증한 태그가 있으면 그 번호를 사용합니다.

**이 단계가 1-6보다 먼저인 이유**

설치하면 `Packages/manifest.json` 에 아래 항목이 추가됩니다.

```json
"com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#v10.1.2"
```

이 줄이 커밋에 포함되어야 학생이 프로젝트를 열 때 Unity가 **같은 버전을 자동으로 설치**합니다.
그래서 학생 세팅 가이드에는 MCP 수동 설치 단계가 없습니다.
`Packages/packages-lock.json` 도 함께 커밋합니다.

**연결 — 창을 열고, 서버를 켜고, 클라이언트를 등록합니다**

`Window > MCP for Unity` 는 하위 메뉴입니다. 창은 `Toggle MCP Window` 로 엽니다.

```
1. Window > MCP for Unity > Toggle MCP Window        (단축키 Ctrl+Shift+M / ⌘⇧M)
2. Connect 탭 (창을 열면 기본 선택)
3. Server > Local Server 줄의 [Start Server] 클릭
4. Client Configuration > [Configure All Detected Clients] 클릭
5. 상태 표시가 Disconnected 에서 연결 상태로 바뀌는지 확인
```

`Configure All Detected Clients` 는 실행 이력이 있는 클라이언트만 잡습니다.
Claude Code가 목록에 없으면 터미널에서 `claude` 를 한 번 실행한 뒤 다시 누릅니다.

**Advanced 탭 — Auto-Start Server on Editor Load 를 켭니다**

이 값이 꺼져 있으면 Unity를 열 때마다 `Start Server` 를 눌러야 합니다.

```
Advanced 탭 > Auto-Start Server on Editor Load 체크
```

이 설정은 **EditorPrefs(PC별 설정)에 저장되어 리포에 커밋되지 않습니다.**
학생 각자가 자기 PC에서 켜야 하므로 `docs/00-SETUP.md` 에 단계로 넣어두었습니다.

**동작 확인 — Claude Code는 반드시 마지막에 실행**

Claude Code는 시작할 때 MCP 설정을 한 번 읽습니다.
`Start Server` 보다 먼저 띄워 두면 그 세션은 Unity 도구를 잡지 못합니다.
이미 실행 중이었다면 종료 후 다시 실행하세요.

```
Unity 열기 → Start Server → Configure All Detected Clients → claude 실행
```

터미널에서 프로젝트 폴더로 이동해 `claude` 를 실행하고 물어봅니다.

```
Unity 콘솔에 지금 어떤 메시지가 있는지 읽어줘.
```

응답 첫 줄에 **`Called UnityMCP`** 가 뜨면 성공입니다.

> **검증 완료**: 템플릿 → clone → Unity 열기(MCP 자동 설치) → Start Server → claude 연결까지
> 실제 테스트 리포로 전 과정을 확인했습니다 (2026-08).
> MCP 등록은 `.mcp.json` 이 아니라 **사용자 전역 설정**에 기록되므로, 한 PC에서 Unity 프로젝트를
> 여러 개 열면 마지막에 등록한 프로젝트를 가리킵니다.

> CoplayDev 리포의 README에는 `Window → MCP for Unity → Configure All Detected Clients` 로 적혀 있으나,
> v10.1.2 기준 실제 메뉴에는 그런 항목이 없습니다. 위 절차는 v10.1.2 소스에서 확인한 것입니다.

### 1-6. Git 초기화 및 푸시

```bash
git init
git add -A
git commit -m "chore: ARENA BREAK 스타터 프로젝트 초기 설정"
git branch -M main
git remote add origin https://github.com/abback-go/Arena-Break.git
git push -u origin main
```

커밋 전에 `git add -A` 후 목록을 확인합니다.

```bash
git add -A
git diff --cached --name-only
```

| 포함되어야 함 | 제외되어야 함 |
|---|---|
| `Assets/Scenes/Arena.unity` (+ `.meta`) | `Library/` |
| `Assets/Scenes/Arena/NavMesh-Floor.asset` (+ `.meta`) | `Temp/`, `Logs/`, `obj/` |
| `Assets/Settings/` (URP 렌더러 에셋) | `UserSettings/` |
| `Assets/InputSystem_Actions.inputactions` | `*.csproj`, `*.sln` |
| `Packages/manifest.json` | |
| `Packages/packages-lock.json` | |
| `ProjectSettings/` | |
| 모든 `.meta` 파일 | |

`.gitignore` 와 `.gitattributes` 는 이 구성으로 실제 커밋을 돌려 확인했습니다.
위 제외 항목이 목록에 나타나면 `.gitignore` 가 적용되지 않은 상태입니다.

> **Git LFS는 사용하지 않습니다.** 스캐폴드 초기 버전의 `.gitattributes` 에 LFS 규칙이 있었으나 제거했습니다.
> LFS를 켜면 `git-lfs` 가 없는 PC에서 clone 시 이미지가 포인터 텍스트로 내려와 Unity 임포트가 깨지고,
> GitHub 무료 계정의 LFS 대역폭(월 1GB)이 수강생 clone으로 소진됩니다.
> 이 프로젝트는 프리미티브 위주라 LFS가 필요 없습니다.

### 1-7. GitHub에서 Template 설정

리포 페이지 → **Settings** → 상단 **Template repository** 체크

학생 화면에 `Use this template` 버튼이 나타납니다.

### 1-8. 검증 근거

이 장의 메뉴 경로는 아래 문서로 확인했습니다 (2026-08 기준).

| 항목 | 출처 |
|---|---|
| Package Manager 메뉴 위치 | [Unity 6.3 Manual — Package Manager window](https://docs.unity3d.com/6000.3/Documentation/Manual/upm-ui.html) |
| Git URL로 패키지 설치 | [Unity 6.3 Manual — Install from Git URL](https://docs.unity3d.com/6000.3/Documentation/Manual/upm-ui-giturl.html) |
| `?path=` / `#revision` 순서 | [Unity 6.3 Manual — Git URLs and extended syntax](https://docs.unity3d.com/6000.3/Documentation/Manual/upm-git.html) |
| Unity Registry에서 설치 | [Unity 6.3 Manual — Install a package](https://docs.unity3d.com/6000.3/Documentation/Manual/upm-ui-install.html) |
| Active Input Handling 위치·재시작 | [Input System — Installation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/Installation.html) |
| NavMesh Surface로 굽기 | [AI Navigation 2.0 — Create a NavMesh](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/CreateNavMesh.html) |
| NavMesh Surface 속성 | [AI Navigation 2.0 — NavMesh Surface](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html) |
| Navigation 창(Agents/Areas 탭) | [AI Navigation 2.0 — Navigation window](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavigationWindow.html) |
| 씬 뷰 파란 표시(AI Navigation 오버레이) | [AI Navigation 2.0 — Navigation overlay](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavigationOverlay.html) |
| 오버레이 켜기(백틱 / ⋮ 버튼) | [Unity 6.3 Manual — Overlays](https://docs.unity3d.com/6000.3/Documentation/Manual/overlays.html) |
| Build Settings → Build Profiles | [Unity 6.3 Manual — Build profiles](https://docs.unity3d.com/6000.3/Documentation/Manual/build-profiles.html) |
| 빌드 씬 목록(Add Open Scenes) | [Unity 6.3 Manual — Manage scenes in a build](https://docs.unity3d.com/6000.3/Documentation/Manual/build-profile-scene-list.html) |
| MCP 메뉴·버튼 이름 | MCP for Unity **v10.1.2 소스** — `Editor/MenuItems/MCPForUnityMenu.cs`, `Editor/Windows/Components/Connection/McpConnectionSection.uxml`, `.../ClientConfig/McpClientConfigSection.uxml`, `.../Advanced/McpAdvancedSection.uxml` |

---

## 2. 단계별 스냅샷 태그 만들기

> **이 절은 이미 완료되어 있습니다.** 태그 7개가 `solution` 브랜치에 만들어져 push되어 있습니다.
> 아래는 다음 학기에 처음부터 다시 만들 때를 위한 기록입니다.

D-14에 **전체 게임을 혼자 한 번 만들면서** 각 단계마다 태그를 찍습니다.
이 태그들이 수업 중 진도 밀린 학생을 구제하는 안전망입니다.

```bash
git tag -a w1-step1    -m "플레이어 이동 (아레나 씬은 스타터에 포함됨)"
git tag -a w1-complete -m "1장 완료: 사격, 탄약, 재장전"
git tag -a w2-step1    -m "Health / IDamageable"
git tag -a w2-step2    -m "NavMesh 적 AI"
git tag -a w2-complete -m "2장 완료: 웨이브 시스템, 오브젝트 풀링"
git tag -a w3-step1    -m "GameManager 상태 머신 + HUD"
git tag -a w3-complete -m "3장 완료: 폴리시, 창 모드 빌드 설정"

git push origin --tags
```

> **태그는 별도 브랜치에 만드세요.** `main`에는 빈 스캐폴드만 두고,
> 완성 코드는 `solution` 브랜치에 두고 태그를 찍는 방식이 깔끔합니다.
> 학생이 `main`을 받으면 빈 상태, 필요할 때만 태그로 점프하게 됩니다.

```bash
git checkout -b solution
# ... 게임 제작 및 단계별 태그 ...
git push origin solution --tags
git checkout main
```

## 3. 수업 직전 최종 점검

### 3-1. 내 PC — 시연 준비 (10분)

```
□ C:\Abback\demo-build\ArenaBreak.exe 가 창 모드(1280x720)로 뜬다
□ 제목 표시줄의 X 로 닫힌다. Esc 로도 닫힌다
□ 프로젝터 해상도에서 HUD 네 개가 다 보인다 (체력·탄약·웨이브·킬)
□ 인터넷이 끊겨도 되도록 zip을 USB에 하나 복사해 둔다
```

### 3-2. 내 PC — 실습 환경 (10분)

**순서를 지켜야 합니다.** Claude Code는 시작할 때 MCP 설정을 한 번만 읽습니다.

```
□ Unity 6.3 으로 프로젝트가 열리고 콘솔 에러 0개
□ Window > MCP for Unity > Toggle MCP Window > [Start Server]
□ 그다음에 터미널에서 claude 실행
□ "Unity 콘솔에 지금 어떤 메시지가 있는지 읽어줘" → Called UnityMCP 가 뜬다
```

수업 중 자주 걸리는 두 가지는 미리 꺼두거나 바꿔둡니다.

```
□ Console 창의 Collapse 를 끈다   ← 켜져 있으면 같은 로그가 안 늘어난다
□ Game 뷰 해상도를 Free Aspect 로 둔다   ← 고정 해상도면 HUD가 잘려 안 보인다
```

### 3-3. 리포 상태 (5분)

```
□ github.com/abback-go/Arena-Break 에 Use this template 버튼이 보인다
□ main 의 Assets/Scripts/ 하위 네 폴더가 비어 있다
□ 태그 7개가 있다      git ls-remote --tags origin
□ Releases 에서 ArenaBreak-demo.zip 이 받아진다
```

### 3-4. 구제 경로 리허설 (5분)

진도 밀린 학생이 나오면 쓸 경로입니다. **한 번은 직접 돌려보고 들어가세요.**

```
□ Unity 를 닫는다
□ ./tools/rescue.sh w1-complete       (Git Bash)
□ ./tools/rescue.ps1 w1-complete      (PowerShell)
□ Assets/Scripts/Player 에 파일 두 개가 생겼다
□ git checkout main 으로 돌아온다
```

Git Bash에서 `.\tools\rescue.ps1` 로 쓰면 백슬래시가 이스케이프로 먹혀
`.toolsrescue.ps1` 이 됩니다. 학생에게는 슬래시로 안내하세요.

**점프해도 최신 상태로 남는 것이 있습니다.** 태그는 지난 시점이라 창 모드 설정·최신 문서·
rescue 스크립트가 들어 있지 않습니다. 그래서 스크립트가 점프 직후
`tools` `docs` `.claude` `ProjectSettings` `CLAUDE.md` `README.md` 를 원래 브랜치 것으로
되살리고 커밋합니다. 코드와 씬만 태그 시점입니다.

### 3-5. 학생에게 보낼 안내 (수업 전날)

```
1. github.com/abback-go/Arena-Break 에서 [Use this template] → 리포 이름 arena-break
2. clone 한 뒤 docs/00-SETUP.md 의 체크리스트 7개를 끝내 올 것
3. 수업 당일에는 docs/01-WEEK1.md 를 열고 시작
```

### 3-6. 알고 들어갈 것 — 검증되지 않은 지점

이 셋은 리허설로 확인할 수 없었습니다. 수업이 막히지는 않지만 미리 알고 계세요.

| 항목 | 왜 미검증인가 | 어떻게 되든 |
|---|---|---|
| 1-1 public 필드 실험 / 2-1 씬 조작 질문 / 3-3 "1번만 만들기" | AI가 규약을 지키는지 보는 관찰 항목인데, 리허설 세션은 문서를 이미 읽은 상태였다 | 문서에 **"안 지켜졌으면 이렇게 해석하라"** 표가 있어 어느 쪽이 나와도 수업이 성립한다 |
| 시간 배분 (20 / 35 / 30분 등) | 사람이 코드를 읽고 질문하는 시간을 재지 않았다 | 밀리면 rescue 스크립트로 다음 단계로 넘긴다 |
| 각 장 과제 | 학생이 프롬프트를 직접 쓰는 활동이라 정답이 없다 | 실패 기록 자체가 평가 대상이다 |

---

## 4. 수업 후

- 새로 나온 문제를 `docs/TROUBLESHOOTING.md`에 추가하고 푸시
- 잘 먹힌 프롬프트 / 실패한 프롬프트를 `docs/PROMPT-CARDS.md`에 반영
- **실제 소요 시간을 기록해 다음 학기 시간표를 조정** — 지금 시간표는 실측값이 아니다
