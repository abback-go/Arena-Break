# 트러블슈팅

수업 중 자주 나오는 문제와 해결법입니다. 새로운 문제가 나오면 이 문서에 계속 추가합니다.

---

## 우선 이것부터

문제가 생기면 순서대로 시도하세요. 대부분 여기서 해결됩니다.

1. **Unity 콘솔의 첫 번째 빨간 에러**를 읽는다 (아래쪽 에러는 대개 그 여파다)
2. Unity 에디터를 껐다 켠다
3. `claude`를 껐다 켠다
4. 그래도 안 되면 → `git checkout .` 로 되돌리고 다시 요청한다

> **3회 시도해도 안 되면 되돌립니다.** 무한 루프에 30분을 쓰는 것이 가장 나쁜 선택입니다.

---

## Unity 6에서 메뉴가 바뀐 것들 ★먼저 확인

인터넷 튜토리얼과 화면이 다르다면 대부분 이것 때문입니다.
**튜토리얼이 아니라 아래 표를 믿으세요.**

| 하려는 것 | 예전 방식 (❌ Unity 6에서 없음) | Unity 6.3 방식 (✅) |
|---|---|---|
| 패키지 설치·확인 | `Window > Package Manager` | **`Window > Package Management > Package Manager`** |
| NavMesh 굽기 | `Static > Navigation Static` 체크 후 Navigation 창에서 Bake | **바닥에 `NavMesh Surface` 컴포넌트 추가 → 그 안의 `Bake`** |
| 빌드 | `File > Build Settings` | **`File > Build Profiles`** |
| 입력 설정 | Player 최상단 | **`Player > Other Settings > Active Input Handling`** (스크롤 필요, 변경 시 재시작) |

Claude Code도 학습 데이터에 옛날 방식이 훨씬 많아서 예전 경로를 안내할 때가 있습니다.
**AI가 알려준 메뉴 경로가 화면에 없으면, AI가 틀린 것입니다.** 위 표대로 하세요.

---

## 세팅 단계

### `uv: command not found` / `claude: command not found`

터미널을 **완전히 닫았다가** 새로 여세요. PATH 환경변수는 새 터미널부터 반영됩니다.
그래도 안 되면:

```bash
# Claude Code 재설치
npm install -g @anthropic-ai/claude-code

# uv 설치 경로 확인 (Windows)
echo $env:USERPROFILE\.local\bin
# 이 경로가 PATH에 없으면 시스템 환경변수에 추가
```

Windows에서 `npm install -g`가 권한 오류를 내면 **관리자 권한 PowerShell**로 실행하세요.

### Unity 임포트가 10분 넘게 끝나지 않음

대부분 **백신 실시간 검사**가 원인입니다. 프로젝트 폴더를 검사 예외로 등록해 보세요.
(Windows 보안 → 바이러스 및 위협 방지 → 설정 관리 → 제외 추가)

그래도 안 되면 Unity를 종료하고 `Library/` 폴더를 삭제한 뒤 다시 여세요.
`Library/`는 Unity가 자동 생성하므로 지워도 안전합니다.

### Unity 버전이 다르다는 경고가 뜸

**6.3 LTS가 아닌 버전으로 열려고 한 것입니다.** 업그레이드하지 마세요.
Unity Hub → 프로젝트 옆 버전 드롭다운 → 6.3 LTS 선택.

이미 다른 버전으로 열어버렸다면:

```bash
git checkout .        # 변경된 ProjectSettings 되돌리기
```

그리고 `Library/` 삭제 후 올바른 버전으로 다시 여세요.

---

## MCP 연결 문제

### Claude Code에 Unity 도구가 없음 (가장 흔합니다)

Claude Code가 *"MCP for Unity 도구가 이 세션에 없음"* 이라고 답하거나,
Unity 콘솔 대신 `Editor.log` 파일을 뒤지기 시작하면 이 경우입니다.

**원인: Claude Code를 MCP 등록보다 먼저 실행했습니다.**
Claude Code는 시작할 때 MCP 설정을 한 번 읽고, 이후 등록된 서버는 그 세션에 반영되지 않습니다.

**해결: Claude Code를 종료하고 다시 실행하세요.** 그게 전부입니다.

올바른 순서는 이렇습니다.

```
1. Unity 열기
2. Window → MCP for Unity → Toggle MCP Window
3. Connect 탭 → [Start Server]        ← Session Active 초록불 확인
4. [Configure All Detected Clients]
5. 그다음에 claude 실행
```

등록 상태는 터미널에서 확인할 수 있습니다.

```bash
claude mcp list
```

`UnityMCP: http://127.0.0.1:8080/mcp (HTTP) – Connected` 가 보이면 등록은 정상입니다.
그래도 도구가 없으면 `claude` 를 껐다 켜세요.

### MCP 창을 어디서 여는지 모르겠음

`Window → MCP for Unity` 는 하위 메뉴입니다. 창을 여는 항목은 **`Toggle MCP Window`** 입니다.

```
Window → MCP for Unity → Toggle MCP Window     (단축키 Ctrl+Shift+M / ⌘⇧M)
```

> CoplayDev 리포 README에는 `Window → MCP for Unity → Configure All Detected Clients` 로 적혀 있지만,
> v10.1.2 실제 메뉴에는 그 항목이 없습니다. `Configure All Detected Clients` 는 **창 안의 버튼**입니다.

### MCP for Unity에 Claude Code가 감지되지 않음

Claude Code를 **한 번이라도 실행한 적이 있어야** 감지됩니다.
터미널에서 `claude` 를 한 번 실행한 뒤, MCP 창의 Connect 탭에서
`Configure All Detected Clients` 를 다시 누르세요.

### "No Unity Instances Found" / 연결이 안 됨

**서버가 꺼져 있는 경우가 대부분입니다.**

```
Window → MCP for Unity → Toggle MCP Window
  → Connect 탭 → Server → Local Server → [Start Server]
```

그래도 안 되면:

1. Unity 에디터가 실행 중인지 확인
2. `claude` 를 껐다 켜기
3. Connect 탭의 상태 표시가 `Disconnected` 가 아닌지 확인

### Unity를 켤 때마다 Start Server를 눌러야 함

Auto-Start가 꺼져 있습니다.

```
MCP for Unity 창 → Advanced 탭 → Auto-Start Server on Editor Load 체크
```

PC별 설정이라 프로젝트에 저장되지 않습니다. 각자 자기 PC에서 켜야 합니다.

### Claude Code가 Unity 콘솔을 못 읽음

MCP 브리지가 끊긴 것입니다. Unity와 Claude Code를 모두 재시작하세요.

**끝까지 안 되면 파일 기반으로 폴백하세요.** 수업은 계속 진행됩니다.

```
지금 MCP 연결이 안 되니까 파일만 직접 수정해줘.
씬 작업은 절차만 알려주면 내가 손으로 할게.
```

### `System.Collections.Immutable` 버전 충돌 에러

**Unity AI Assistant 패키지가 설치된 것입니다.** 이 프로젝트에서는 사용하지 않습니다.

대부분의 프로젝트에는 애초에 없습니다. 먼저 진단부터 하세요.

```bash
python tools/strip-unity-ai.py . --dry-run
```

`[OK] 제거할 AI 패키지가 없습니다` 가 나오면 원인이 다른 곳에 있습니다.
콘솔의 **첫 번째** 빨간 에러 전문을 가지고 질문하세요.

제거 대상이 나왔다면 아래 순서로 진행합니다.

**1) Package Manager에서 세 개를 동시에 선택해 한 번에 제거합니다.**

```
Window → Package Management → Package Manager → In Project
  Assistant / Generators / 2D Enhancers  ← Ctrl 클릭으로 동시 선택 → Remove
```

AI Toolkit은 의존성으로만 있어서 자동으로 사라집니다.

**2) Unity를 완전히 종료한 뒤 잔여물을 정리합니다.**

```bash
python tools/strip-unity-ai.py .
```

- `Packages/manifest.json`에 남은 AI 패키지 제거
- `Packages/packages-lock.json` 삭제 (안 지우면 의존성으로 되살아남)
- `Library/` 삭제

**3) Unity를 다시 열고 콘솔 에러가 0개인지 확인합니다.**

### 지웠는데 AI 패키지가 다시 생김

**하나씩 지웠기 때문입니다.** 의존성 체인
(AI Toolkit → Generators → 2D Enhancers)이 서로를 다시 끌어옵니다.

**Assistant / Generators / 2D Enhancers 세 개를 동시에 선택해서 한 번에 제거**하세요.
하나씩 지우면 몇 초 안에 되살아납니다.

> 목록에는 "AI Assistant"가 아니라 그냥 **"Assistant"** 로 표시됩니다. "AI"로 검색하면 안 나올 수 있습니다.
>
> **`AI Navigation`은 절대 지우지 마세요.** 이름에 AI가 들어가지만 NavMesh 패키지이고
> 2주차 적 AI에 필수입니다. 스크립트도 이것만은 보존합니다.

---

## 코드 / 컴파일 문제

### 컴파일 에러가 났는데 원인을 모르겠음

바로 고쳐달라고 하지 말고 **원인부터** 물어보세요.

```
컴파일 에러가 났어. Unity 콘솔을 읽고 원인을 설명해줘.
바로 고치지 말고, 왜 이 에러가 났는지 먼저 알려줘.
```

### AI가 고칠수록 에러가 늘어남

가장 흔한 실패 패턴입니다. **즉시 멈추고 되돌리세요.**

```bash
git checkout .
```

그리고 더 작게 쪼개서 다시 요청하세요.

### Input System 관련 에러 (`InputSystem` 네임스페이스를 못 찾음)

`Edit → Project Settings → Player → **Other Settings** → Active Input Handling` 을 확인하세요.
새 Input System 항목(에디터 버전에 따라 `Input System Package (New)` 또는 `New Input System`)이어야 합니다.
**변경하면 에디터를 반드시 재시작해야 반영됩니다.**

### `The type or namespace name 'NavMeshAgent' could not be found`

AI Navigation 패키지가 없습니다.
`Window → Package Management → Package Manager → Unity Registry`에서 **AI Navigation**을 설치하세요.
코드 상단에 `using Unity.AI.Navigation;`이 필요할 수 있습니다.

---

## 실행 / 동작 문제

### 마우스 시점이 안 움직임 / 커서가 화면 밖으로 나감

`Cursor.lockState = CursorLockMode.Locked;` 가 없는 경우입니다.
게임 뷰를 한 번 클릭해 포커스를 준 뒤 테스트하세요.
에디터에서 커서를 풀려면 `Esc`를 누르세요.

### 적이 움직이지 않음

NavMesh가 베이크되지 않았습니다.

바닥(`Floor`)을 선택하고 **NavMesh Surface** 컴포넌트의 `Bake` 버튼을 누르세요.
씬 뷰에 파란 영역이 보이면 성공입니다.

컴포넌트가 아예 없다면 붙입니다.

```
Floor 선택 → Add Component → Navigation → NavMesh Surface
Collect Objects = All Game Objects  →  [Bake]
```

베이크 여부는 컴포넌트의 `NavMesh Data` 항목으로 확인합니다. `None` 이면 아직 안 구운 것입니다.

### Bake를 눌렀는데 파란 영역이 안 보임

베이크 실패가 아니라 **표시가 꺼진 것**일 가능성이 높습니다.
파란 표시는 씬 뷰의 **AI Navigation 오버레이**가 그리며, 기본값은 켜짐 + 씬 뷰 우측 하단 도킹입니다.

```
씬 뷰에서 ` (백틱) 키  또는  씬 뷰 우측 상단의 ⋮ (More) 버튼
  → 오버레이 목록에서 "AI Navigation" 켜기
  → AI Navigation 오버레이 > Surfaces > Show NavMesh 체크
```

그래도 안 보이면 진짜 베이크가 안 된 것입니다.
NavMesh Surface 컴포넌트의 `NavMesh Data` 항목을 보세요. `None` 이면 베이크되지 않은 상태입니다.

### NavMesh Data가 `Missing` 이 됨 / 베이크가 사라짐

**SampleScene 폴더를 지웠는데 그 안에 NavMesh 에셋이 있었던 경우입니다.**

베이크된 NavMesh는 **베이크 당시 씬 이름으로 된 폴더**에 `NavMesh-<오브젝트명>` 형태로 저장됩니다.
SampleScene 상태에서 굽고 나중에 씬 이름을 바꿨다면, 에셋은 여전히 `Scenes/SampleScene/` 안에 있습니다.

**해결**: Arena 씬을 연 상태에서 Floor를 선택하고 **다시 Bake** 하면 됩니다.
`Assets/Scenes/Arena/` 에 새 에셋이 생기고 참조가 복구됩니다. 데이터 손실은 없습니다(다시 구우면 그만입니다).

**예방**: 씬 이름을 확정하고 SampleScene을 정리한 **뒤에** 베이크하세요.

### `Static` 드롭다운에서 `Navigation Static`이 회색이라 못 누름

**정상입니다. 고장이 아닙니다.**

AI Navigation 2.x부터 Static 플래그 방식이 폐기되고 **NavMesh Surface 컴포넌트** 방식으로 바뀌었습니다.
인터넷의 오래된 튜토리얼(`Window → AI → Navigation`에서 Bake)은 Unity 6에 맞지 않습니다.

위의 「적이 움직이지 않음」 절차대로 NavMesh Surface를 쓰세요.

### 총알이 아무것도 못 맞힘

- 대상에 **Collider**가 있는지 확인
- Raycast의 `LayerMask` 설정 확인
- 발사 원점이 카메라 위치인지 확인 (플레이어 몸통에 먼저 맞고 있을 수 있음)

디버그가 필요하면:

```
Raycast가 실제로 어디에 맞는지 확인하고 싶어.
Debug.DrawRay로 시각화하는 코드를 추가해줘.
```

### 빌드했는데 빈 화면 / 아무것도 안 나옴

**빌드 씬 목록에 Arena가 등록되지 않은 것입니다.** 빌드는 목록에 있는 씬만 포함합니다.

```
Arena 씬을 연 상태에서
File > Build Profiles → 플랫폼 프로필 선택 → Scene List → [Add Open Scenes]
```

Scene List 항목이 안 보이면 `Add Settings > Scene List` 로 먼저 추가하세요.

### 에디터에서는 되는데 빌드하면 안 됨

`UnityEditor` 네임스페이스를 런타임 코드에서 쓴 경우가 대부분입니다.

```
빌드에서만 실패하는 문제야.
UnityEditor 네임스페이스를 런타임 스크립트에서 쓰고 있는지 전체를 훑어봐.
```

---

## Git 문제

### 교수님이 문서를 고쳤다는데 내 것은 그대로

내 리포는 `Use this template` 으로 **복사해서 만든 별개의 저장소**입니다.
교수 리포와 연결되어 있지 않아 자동으로 따라오지 않습니다.

`docs/` 만 가져옵니다.

```bash
git fetch upstream
git checkout upstream/main -- docs/
git commit -m "docs: 교수 문서 최신본 반영"
```

`Assets/` 는 건드리지 않으므로 **내 코드와 씬은 그대로 남습니다.**

> `git merge upstream/main` 은 쓰지 마세요. 두 저장소는 커밋 이력이 아예 달라서
> 병합이 되지 않고, 되더라도 `Arena.unity` 가 충돌합니다.

`tools/` 의 스크립트도 갱신됐다면 같은 방법으로 받습니다.

```bash
git checkout upstream/main -- tools/
```

### `Library/`가 커밋되려고 함

`.gitignore`가 적용되지 않은 상태입니다. 이미 추적 중이면 캐시를 비웁니다.

```bash
git rm -r --cached Library/
git commit -m "chore: untrack Library"
```

### 씬 파일 충돌 (`.unity` merge conflict)

Unity 씬은 텍스트지만 손으로 병합하기 어렵습니다. 팀 작업이 아니라면 한쪽을 고르세요.

```bash
git checkout --ours Assets/Scenes/Arena.unity    # 내 것 유지
git checkout --theirs Assets/Scenes/Arena.unity  # 상대 것 채택
git add Assets/Scenes/Arena.unity
```

### 진도가 밀려서 따라잡고 싶음

**Unity를 먼저 닫으세요.** 씬 파일이 바뀌는데 Unity가 그것을 메모리에 들고 있으면 충돌합니다.

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\rescue.ps1 w2-complete
```

Git Bash / macOS 라면:

```bash
./tools/rescue.sh w2-complete
```

하던 작업은 스크립트가 `wip` 커밋으로 자동 저장합니다.
끝나면 Unity를 다시 열고 `Assets/Scenes/Arena` 씬을 엽니다.

태그 이름을 빼고 실행하면 고를 수 있는 목록이 나옵니다.

### 점프한 뒤 원래 작업으로 돌아가기

**Unity를 먼저 닫고** 한 줄이면 됩니다.

```bash
git checkout main
```

점프해 둔 브랜치(`rescue-<태그>`)는 남겨두세요. 다시 그 시점을 보고 싶을 때
`git checkout rescue-w2-complete` 한 줄이면 갑니다.

Unity를 다시 열고 `Assets/Scenes/Arena` 씬을 엽니다.

### 돌아가려는데 `untracked working tree files would be overwritten`

```
error: The following untracked working tree files would be overwritten by checkout:
        tools/rescue.ps1
        tools/rescue.sh
```

**2026년 8월 이전 버전의 rescue 스크립트로 점프한 브랜치**에서만 납니다.
그때는 되살린 `tools/` 를 커밋하지 않아서, 추적되지 않은 파일로 남아 있었습니다.

지우고 넘어가면 됩니다. `main` 으로 가면서 원래 파일이 그대로 복원되니 잃는 것은 없습니다.

```bash
rm tools/rescue.ps1 tools/rescue.sh
git checkout main
```

### `-File 매개 변수에 대한 인수 '.toolsrescue.ps1'이(가) 없습니다`

Git Bash에서 PowerShell 명령을 실행한 것입니다. Git Bash는 `\` 를 이스케이프 문자로 처리해서
`.\tools\rescue.ps1` 이 `.toolsrescue.ps1` 로 뭉개집니다.

Git Bash에서는 셸 스크립트 쪽을 쓰세요.

```bash
./tools/rescue.sh w2-complete
```

PowerShell 명령은 **PowerShell 창에서** 실행합니다.

### 제목이 `Untitled` 이고 저장 창이 뜸

Unity를 켠 채로 브랜치를 옮겼을 때 생깁니다. 씬 파일이 통째로 바뀌었는데
Unity는 그것을 모르고 빈 씬으로 되돌아간 상태입니다.

**저장하지 마세요.** 여기서 `Arena.unity` 를 고르면 빈 씬이 아레나를 덮어씁니다.
바닥·벽·스폰 포인트가 전부 사라집니다.

1. 저장 창에서 **취소**
2. `Assets/Scenes/Arena` 더블클릭 — 또는 `File → Open Scene`

씬 파일은 멀쩡합니다. 다시 열기만 하면 됩니다.

브랜치를 옮길 때는 순서를 지키세요.

```
Unity 닫기 → 점프 또는 checkout → Unity 열기 → Arena 씬 열기
```

### 실수로 커밋을 날렸음

거의 항상 복구됩니다. 당황하지 말고:

```bash
git reflog          # 최근 이동 기록 확인
git checkout <해시>  # 그 시점으로 이동
```

---

## 그래도 안 될 때

아래 세 가지를 첨부해서 질문하세요. 이 세 가지가 없으면 아무도 도와줄 수 없습니다.

1. **Unity 콘솔 첫 번째 빨간 에러 전문** (스크린샷 말고 텍스트 복사)
2. **사용한 프롬프트 전문**
3. **무엇을 기대했고 실제로 무엇이 일어났는지**
