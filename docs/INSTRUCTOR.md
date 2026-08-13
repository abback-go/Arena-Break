# 교수·조교용 운영 노트

> 이 문서는 학생에게 배포해도 무방하지만, 주 독자는 수업을 운영하는 사람입니다.

---

## 1. 스타터 리포 만드는 순서 (최초 1회)

이 스캐폴드는 **Unity 프로젝트 위에 덮어씌우는 껍데기**입니다.
Unity 프로젝트 본체(ProjectSettings, Packages, URP 에셋)는 Unity Hub가 생성해야 합니다.

### 1-1. Unity Hub에서 프로젝트 생성

```
Unity Hub → New Project
  Editor Version : Unity 6.3 LTS (6000.3.x)
  Template       : Universal 3D          ← URP 템플릿. "3D (Built-In)" 아님
  Project name   : arena-break-starter
```

### 1-2. 스캐폴드 덮어쓰기

이 폴더의 내용물을 방금 만든 프로젝트 루트에 복사합니다.
`Assets/` 안의 빈 폴더들은 그대로 합쳐지고, 나머지 파일은 새로 추가됩니다.

```bash
# 예시 (Windows PowerShell)
Copy-Item -Path ".\arena-break-starter\*" -Destination "C:\Unity\arena-break-starter" -Recurse -Force
```

### 1-3. AI 패키지 정리 ★필수

**이 단계를 건너뛰면 학생들이 `System.Collections.Immutable` 충돌을 만납니다.**

#### (1) Package Manager에서 한꺼번에 제거 — 이게 본 작업입니다

```
Window → Package Manager → In Project
```

아래 세 개를 **Ctrl 클릭으로 동시에 선택**한 뒤 한 번에 `Remove`:

- **Assistant** (`com.unity.ai.assistant`)
- **Generators** (`com.unity.ai.generators`)
- **2D Enhancers**

AI Toolkit은 의존성으로만 들어와 있어서 자동으로 사라집니다. 따로 지우지 마세요.

> **하나씩 지우면 몇 초 만에 되살아납니다.** 의존성 체인
> (AI Toolkit → Generators → 2D Enhancers)이 서로를 다시 끌어오기 때문입니다.
> 반드시 **동시 선택 후 일괄 제거**하세요.
>
> 목록에는 "AI Assistant"가 아니라 **"Assistant"** 로 표시됩니다.
>
> **`AI Navigation`은 절대 지우지 마세요.** NavMesh 패키지이고 2주차에 필요합니다.

#### (2) Unity 종료 후 잔여물 정리

```bash
cd C:\Unity\arena-break-starter
python tools/strip-unity-ai.py . --dry-run   # 남은 것 확인
python tools/strip-unity-ai.py .             # lock / Library 정리
```

스크립트는 **제거 수단이 아니라 검증·뒷정리 도구**입니다.
manifest.json 직접 편집만으로는 해결되지 않은 사례가 보고되어 있어,
Package Manager를 먼저 쓰고 스크립트로 마무리하는 순서를 권합니다.

자세한 내용은 [reference/PACKAGES.md](../reference/PACKAGES.md) 참조.

### 1-4. Unity로 열어 확인

- 콘솔 에러 0개
- `Edit → Project Settings → Player → Active Input Handling` = **Input System Package (New)**
- `Window → Package Manager`에 **AI Navigation** 설치 확인 (NavMesh용)

### 1-5. 아레나 씬 만들기

`Assets/Scenes/Arena.unity`로 저장합니다. 최소 구성:

| 오브젝트              | 내용                                               |
| --------------------- | -------------------------------------------------- |
| `Floor`             | Plane, Scale (4, 1, 4), Navigation Static          |
| `Walls`             | Cube 4개로 둘러싸기, Navigation Static             |
| `SpawnPoints`       | 빈 오브젝트 + 자식 Transform 6개를 가장자리에 배치 |
| `Directional Light` | 기본                                               |

**NavMesh 베이크**: `Window → AI → Navigation → Bake`
바닥이 파랗게 덮이면 성공입니다.

> **2주 압축 버전으로 운영한다면** 이 씬과 NavMesh 베이크를 미리 완성해서 배포하세요. 약 30분을 절약할 수 있습니다.

### 1-6. MCP for Unity 설치 및 버전 고정

```
Window → Package Manager → + → Install package from git URL
https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity##v10.1.2
```

**반드시 버전 태그를 고정하세요.** `#main`으로 두면 학기 중 업스트림이 바뀌어
학생마다 동작이 달라집니다. 설치 후 `Packages/manifest.json`에 박힌 URL을 확인하고,
그 상태로 커밋해 배포하세요.

### 1-7. Git 초기화 및 푸시

```bash
git init
git add -A
git commit -m "chore: ARENA BREAK 스타터 프로젝트 초기 설정"
git branch -M main
git remote add origin https://github.com/<교수계정>/arena-break-starter.git
git push -u origin main
```

### 1-8. GitHub에서 Template 설정

리포 페이지 → **Settings** → 상단 **Template repository** 체크

이걸 켜야 학생 화면에 `Use this template` 버튼이 나타납니다.

---

## 2. 단계별 스냅샷 태그 만들기

D-14에 **전체 게임을 혼자 한 번 만들면서** 각 단계마다 태그를 찍습니다.
이 태그들이 수업 중 진도 밀린 학생을 구제하는 안전망입니다.

```bash
git tag -a w1-step1    -m "아레나 씬 + 플레이어 이동"
git tag -a w1-complete -m "1주차 완료: 사격, 탄약, 재장전"
git tag -a w2-step1    -m "Health / IDamageable"
git tag -a w2-step2    -m "NavMesh 적 AI"
git tag -a w2-complete -m "2주차 완료: 웨이브 시스템, 오브젝트 풀링"
git tag -a w3-step1    -m "GameManager 상태 머신 + HUD"
git tag -a w3-complete -m "3주차 완료: 폴리시, 빌드 설정"

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

---

## 3. 수업 진행 체크포인트

각 블록이 끝날 때 아래를 확인하고 넘어갑니다. 미달이면 다음 블록을 줄이세요.

| 시점   | 확인 사항                   | 미달 시                                           |
| ------ | --------------------------- | ------------------------------------------------- |
| 1주차  | 학생 80%가 MCP 연결 성공    | 실습 A를 10분 연장, 시연 B 축소                   |
| 1주차  | 학생 70%가 이동 + 사격 동작 | `w1-complete` 태그 배포 후 진행                 |
| 2주차  | 적이 플레이어를 추적함      | NavMesh 베이크 일괄 시연                          |
| 2주차  | 웨이브가 넘어감             | `w2-complete` 태그 배포                         |
| 3주차  | 코드 리뷰 세션 완료         | **이건 절대 생략 금지.** 빌드를 과제로 이전 |

---

## 4. 라이브 시연 사고 대응

| 상황                         | 즉시 대응                                                                                       |
| ---------------------------- | ----------------------------------------------------------------------------------------------- |
| AI가 리허설과 다른 결과를 냄 | 당황하지 말고 정면 활용 — "이게 AI의 실제 모습입니다" → 결과를 함께 읽고 후속 프롬프트로 교정 |
| MCP 연결 끊김                | 파일 기반으로 폴백 선언. 씬 작업은 수동으로 시연                                                |
| API 응답이 오지 않음         | 백업 녹화 영상 재생 → 그동안 복구 시도                                                         |
| 컴파일 에러 루프             | `git checkout .` 후 태그로 점프. **5분 이상 붙들지 말 것**                              |
| 진도 30분 이상 밀림          | 시연 B를 포기하고 태그 배포 → 실습 시간으로 전환                                               |

**백업 녹화**: 각 시연 구간(카드 #1~#9)을 미리 녹화해 두세요.
최악의 경우에도 수업은 진행됩니다.

---

## 5. 평가 운영

### 구두 질의 (25점) — 표절 판별의 핵심

제출된 코드에서 무작위로 한 부분을 골라 묻습니다.

- "이 `[SerializeField]`가 왜 필요한가요? 없으면 어떻게 되나요?"
- "여기서 event를 해제하는 코드가 있는데, 안 하면 무슨 일이 생기나요?"
- "이 값을 2배로 바꾸면 게임이 어떻게 달라지나요?"
- "이 부분을 AI에게 어떻게 요청했나요?"

AI 답변을 그대로 복붙한 학생은 **세 번째 질문에서 반드시 막힙니다.**

### 프롬프트 품질 (25점)

`docs/my-prompts.md` 제출물을 봅니다.

| 수준       | 특징                                                      |
| ---------- | --------------------------------------------------------- |
| 상 (21~25) | 제약 조건 명시, 작업 분할, 실패 후 프롬프트를 개선한 흔적 |
| 중 (15~20) | 요구사항은 있으나 범위 제한이 없음. 한 번에 다 요청       |
| 하 (~14)   | "OO 만들어줘" 수준. 실패 기록 없음                        |

**실패 기록이 없는 제출물을 의심하세요.** 3주 동안 한 번도 실패하지 않는 것은 불가능합니다.

---

## 6. 조교 브리핑 (D-1)

조교가 순회하며 할 일:

1. **먼저 답을 주지 않는다.** "콘솔 첫 번째 에러가 뭐예요?"부터 묻는다
2. 학생이 5분 이상 같은 자리에 있으면 태그 점프를 안내한다
3. 같은 질문이 3명 이상에게서 나오면 **즉시 교수에게 알린다** (전체 공지 사안)
4. 세팅 미완료 학생은 별도로 모아 뒤에서 처리한다 (수업 흐름을 끊지 않는다)

---

## 7. 수업 후

- 새로 나온 문제를 `docs/TROUBLESHOOTING.md`에 추가하고 푸시
- 잘 먹힌 프롬프트 / 실패한 프롬프트를 `docs/PROMPT-CARDS.md`에 반영
- 실제 소요 시간을 기록해 다음 학기 시간표를 조정
