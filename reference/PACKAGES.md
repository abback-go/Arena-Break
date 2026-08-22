# 패키지 구성

> 버전 번호는 Unity 6.3 LTS의 패치 버전에 따라 달라집니다.
> **이 문서에는 일부러 버전 번호를 적지 않았습니다.** Package Manager가 Unity 6.3에 맞는 버전을 자동으로 잡아줍니다.
> 아래는 "무엇이 있어야 하고, 무엇이 없어야 하는가"의 목록입니다.

---

## 반드시 있어야 하는 패키지

`Window → Package Manager → In Project` 에서 확인하세요.

| 패키지 이름 | 패키지 ID | 용도 | 설치 경로 |
|---|---|---|---|
| Universal RP | `com.unity.render-pipelines.universal` | URP 렌더링 | Universal 3D 템플릿에 포함 |
| Input System | `com.unity.inputsystem` | WASD·마우스·키 입력 | Unity Registry |
| **AI Navigation** | `com.unity.ai.navigation` | **NavMesh / NavMeshAgent** | Unity Registry |
| Unity UI (uGUI) | `com.unity.ugui` | HUD | 기본 포함 |
| TextMeshPro | uGUI에 포함 | HUD 텍스트 | 기본 포함 |
| MCP for Unity | Git URL (아래 참조) | Claude Code ↔ Unity 브리지 | 수동 설치 |

### AI Navigation 주의

이름에 `ai`가 들어가지만 **인공지능과 무관한 길찾기(NavMesh) 패키지**입니다.
2장 적 AI에 반드시 필요하므로 **절대 제거하지 마세요.**

Universal 3D 템플릿에 기본 포함되지 않는 경우가 있습니다.
`Window → Package Manager → Unity Registry → AI Navigation → Install`

### MCP for Unity 설치

```
Window → Package Manager → + → Install package from git URL
```

```
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#<버전태그>
```

**버전 태그를 고정하세요.** `#main`으로 두면 학기 중 업스트림이 바뀌어 학생마다 동작이 달라집니다.
교수님이 안내한 태그를 그대로 사용하세요.

요구 사항: **Python 3.10 이상 + uv**

---

## 반드시 제거해야 하는 패키지

| 패키지 ID | 왜 제거하는가 |
|---|---|
| `com.unity.ai.assistant` | MCP for Unity와 `System.Collections.Immutable` 버전 충돌 (v10 vs v9) |
| `com.unity.ai.inference` | AI Assistant 의존성 |
| `com.unity.ai.generators` | AI Assistant 의존성 |
| `com.unity.ai.toolkit` | AI Assistant 의존성 |
| `com.unity.asset-manager-for-unity` | AI Assistant와 함께 딸려 오는 경우가 있음 |

### 충돌의 실제 내용

Unity에는 NuGet 같은 의존성 리졸버가 없어서 DLL 버전 충돌을 자동 해결하지 못합니다.

```
Unity AI Assistant  →  System.Collections.Immutable v10
MCP for Unity       →  System.Collections.Immutable v9  (CodeAnalysis 의존)
Unity 내장          →  System.Collections.Immutable v8
```

세 개가 한 프로젝트에 있으면 컴파일이 깨집니다.
수동 해결은 `Assets/Plugins/`에 v9.0.0 DLL을 넣는 것이지만,
**이 수업에서는 AI Assistant 자체를 쓰지 않으므로 제거가 정답입니다.**

Claude Code가 이미 AI 어시스턴트 역할을 하고 있고,
둘을 동시에 켜면 "어느 AI가 무엇을 했는지" 구분되지 않아 학습에 방해가 됩니다.

---

## 제거 방법

### 1단계 — Package Manager에서 한꺼번에 제거 ★권장

**이것이 가장 확실한 방법입니다.**

```
Window → Package Manager → In Project
```

아래 세 개를 **Ctrl(⌘) 클릭으로 동시에 선택**한 뒤 한 번에 `Remove` 합니다.

- **Assistant** (`com.unity.ai.assistant`)
- **Generators** (`com.unity.ai.generators`)
- **2D Enhancers**

**AI Toolkit은 따로 지우지 않습니다.** 위 세 개의 의존성으로만 들어와 있어서, 셋이 빠지면 자동으로 사라집니다.

> ### 하나씩 지우면 실패합니다
> 의존성 체인이 **AI Toolkit → Generators → 2D Enhancers** 로 얽혀 있어서,
> 하나만 제거하면 나머지가 몇 초 안에 다시 끌어옵니다.
> "지웠는데 다시 생겼다"는 대부분 이 경우입니다. **반드시 동시에 선택해서 제거하세요.**

> ### 이름 주의
> Package Manager 목록에는 "AI Assistant"가 아니라 그냥 **"Assistant"** 로 표시됩니다.
> "AI"로 검색하면 안 나올 수 있습니다.

> ### AI Navigation은 건드리지 마세요
> 이름에 `ai`가 들어가지만 NavMesh 패키지입니다. 2장 적 AI에 반드시 필요합니다.

### 2단계 — 잔여물 정리 및 검증

Unity를 **완전히 종료**한 뒤 실행합니다.

```bash
python tools/strip-unity-ai.py . --dry-run   # 무엇이 남았는지 먼저 확인
python tools/strip-unity-ai.py .             # 정리 실행
```

이 스크립트가 하는 일:

1. `Packages/manifest.json`에 남은 AI 패키지 제거 (`com.unity.ai.navigation`은 **보존**)
2. `Packages/packages-lock.json` 삭제 — 남아 있으면 의존성이 되살아납니다
3. `Library/` 삭제 — 캐시를 비워야 깨끗하게 다시 임포트됩니다

> **스크립트는 1단계의 대체재가 아니라 뒷정리·검증 도구입니다.**
> manifest.json 직접 편집과 lock 파일 삭제만으로는 해결되지 않았다는 사례가 보고되어 있습니다.
> Package Manager에서 먼저 제거하고, 스크립트로 마무리하세요.

### 순서 요약

```
Unity 실행 → Package Manager에서 3개 동시 Remove → Unity 완전 종료
  → python tools/strip-unity-ai.py . → Unity 다시 열기 → 콘솔 에러 0 확인
```

---

## 검증

Unity를 다시 열고 확인하세요.

```
□ 콘솔 에러 0개
□ Window → Package Manager → In Project 에 AI Assistant 없음
□ AI Navigation 있음
□ Window → MCP for Unity 메뉴 존재
□ Edit → Project Settings → Player → Active Input Handling = Input System Package (New)
```

또는 점검 스크립트를 돌리세요.

```bash
# Windows
./tools/setup-check.ps1

# macOS / Linux
bash tools/setup-check.sh
```
