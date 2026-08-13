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

### MCP for Unity에 Claude Code가 감지되지 않음

Claude Code를 **한 번이라도 실행한 적이 있어야** 감지됩니다.
터미널에서 `claude`를 한 번 실행한 뒤, Unity에서 `Window → MCP for Unity → Configure All Detected Clients`를 다시 누르세요.

### "No Unity Instances Found"

1. Unity 에디터가 실행 중인지 확인
2. `claude`를 껐다 켜기
3. Unity의 `Window → MCP for Unity` 상태 패널에서 초록불 확인

### Claude Code가 Unity 콘솔을 못 읽음

MCP 브리지가 끊긴 것입니다. Unity와 Claude Code를 모두 재시작하세요.

**끝까지 안 되면 파일 기반으로 폴백하세요.** 수업은 계속 진행됩니다.

```
지금 MCP 연결이 안 되니까 파일만 직접 수정해줘.
씬 작업은 절차만 알려주면 내가 손으로 할게.
```

### `System.Collections.Immutable` 버전 충돌 에러

**Unity AI Assistant 패키지가 설치된 것입니다.** 이 프로젝트에서는 사용하지 않습니다.

**1) Package Manager에서 세 개를 동시에 선택해 한 번에 제거합니다.**

```
Window → Package Manager → In Project
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

`Edit → Project Settings → Player → Active Input Handling`이
**`Input System Package (New)`** 로 되어 있는지 확인하세요.
변경하면 에디터 재시작을 요구합니다.

### `The type or namespace name 'NavMeshAgent' could not be found`

AI Navigation 패키지가 없습니다.
`Window → Package Manager → Unity Registry`에서 **AI Navigation**을 설치하세요.
코드 상단에 `using Unity.AI.Navigation;`이 필요할 수 있습니다.

---

## 실행 / 동작 문제

### 마우스 시점이 안 움직임 / 커서가 화면 밖으로 나감

`Cursor.lockState = CursorLockMode.Locked;` 가 없는 경우입니다.
게임 뷰를 한 번 클릭해 포커스를 준 뒤 테스트하세요.
에디터에서 커서를 풀려면 `Esc`를 누르세요.

### 적이 움직이지 않음

NavMesh가 베이크되지 않았습니다.

1. 바닥 오브젝트를 선택
2. `Window → AI → Navigation`
3. 바닥을 **Navigation Static**으로 설정
4. `Bake` 클릭

씬 뷰에 파란 영역이 보이면 성공입니다.

### 총알이 아무것도 못 맞힘

- 대상에 **Collider**가 있는지 확인
- Raycast의 `LayerMask` 설정 확인
- 발사 원점이 카메라 위치인지 확인 (플레이어 몸통에 먼저 맞고 있을 수 있음)

디버그가 필요하면:

```
Raycast가 실제로 어디에 맞는지 확인하고 싶어.
Debug.DrawRay로 시각화하는 코드를 추가해줘.
```

### 에디터에서는 되는데 빌드하면 안 됨

`UnityEditor` 네임스페이스를 런타임 코드에서 쓴 경우가 대부분입니다.

```
빌드에서만 실패하는 문제야.
UnityEditor 네임스페이스를 런타임 스크립트에서 쓰고 있는지 전체를 훑어봐.
```

---

## Git 문제

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

```bash
git add -A && git commit -m "wip"
git fetch upstream --tags
git checkout -b rescue w2-complete
```

원래 작업으로 돌아가려면 `git checkout main`.

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
