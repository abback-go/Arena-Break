# ARENA BREAK

**Claude Code × Unity 게임 개발 실습** 스타터 프로젝트
명지전문대 AI게임학과 · 3시간 × 3주 모듈

3D 1인칭 웨이브 방어 슈터를 Claude Code와 함께 만들면서, **AI와 협업하는 개발 워크플로우**를 익힙니다.

---

## 이 수업에서 배우는 것

게임을 만드는 것은 수단입니다. 실제 목표는 아래 세 가지입니다.

| 주차 | 게임 기능 | AI 협업 역량 |
|---|---|---|
| 1주차 | 플레이어 이동 + 사격 | **컨텍스트 설계** — `CLAUDE.md`, 명세 우선 |
| 2주차 | 적 AI + 웨이브 시스템 | **작업 분할과 검증** — 작게 요청, 즉시 테스트 |
| 3주차 | UX 폴리시 + 빌드 | **코드 리뷰와 판단** — AI 코드 읽고 고치기 |

> **"AI에게 무엇을 시킬지 정하는 것이 실력이고, AI가 만든 것을 검증하는 것이 책임이다."**

---

## 빠른 시작 (학생용)

### 0. 사전 준비

수업 전에 **[docs/00-SETUP.md](docs/00-SETUP.md)** 를 끝까지 따라 하세요.
세팅이 안 된 상태로 수업에 오면 3시간 동안 아무것도 못 합니다.

필요한 것: Unity 6.3 LTS · Git · Node.js · Claude Code · Python 3.10+ · uv

### 1. 내 리포 만들기

이 리포 상단의 **`Use this template` → `Create a new repository`** 를 누릅니다.
Fork가 아닙니다. 템플릿으로 만들어야 커밋 히스토리가 깨끗하게 시작됩니다.

```
Repository name : arena-break        ← 이 이름으로 만드세요
공개 범위        : Public
```

기본값으로 `Arena-Break` 가 채워져 있으면 `arena-break` 로 바꿔 주세요.
아래 명령들이 이 이름을 기준으로 되어 있습니다.

### 2. 클론하고 upstream 등록

```bash
# <내계정> 부분만 본인 GitHub 아이디로 바꾸세요
git clone https://github.com/<내계정>/arena-break.git
cd arena-break

# 교수 리포 등록 — 단계별 스냅샷을 받기 위해 필요합니다 (이 줄은 그대로 복사)
git remote add upstream https://github.com/abback-go/Arena-Break.git
git fetch upstream --tags
```

### 3. Unity로 열기

Unity Hub → `Add` → 이 폴더 선택 → **Unity 6.3 LTS**로 열기
첫 임포트는 3~10분 걸립니다. **콘솔 에러가 0개인지 확인**하세요.

### 4. 환경 점검

```bash
# Windows (PowerShell)
./tools/setup-check.ps1

# macOS / Linux
bash tools/setup-check.sh
```

모든 항목에 `[OK]`가 뜨면 준비 완료입니다.

### 5. 수업 당일

**[docs/01-WEEK1.md](docs/01-WEEK1.md)** 를 열어 처음부터 순서대로 따라가세요.
3시간 실습이 그 문서 하나로 진행됩니다. 프롬프트, 씬 세팅, 검증 체크리스트가 전부 들어 있습니다.

---

## 진도가 밀렸을 때 — 두 줄이면 따라잡습니다

수업 중 막혔다고 포기하지 마세요. 내 작업을 보존한 채로 교수님 스냅샷으로 점프할 수 있습니다.

```bash
git add -A && git commit -m "wip"      # 내가 하던 작업 저장
git fetch upstream --tags              # 최신 스냅샷 받기
git checkout -b rescue w2-complete     # 원하는 시점으로 점프
```

원래 작업으로 돌아가려면 `git checkout main`.

### 단계별 스냅샷 태그

| 태그 | 상태 |
|---|---|
| `w1-step1` | 플레이어 이동 (아레나 씬은 스타터에 이미 포함) |
| `w1-complete` | + Raycast 사격, 탄약/재장전 |
| `w2-step1` | + Health / IDamageable |
| `w2-step2` | + NavMesh 적 AI |
| `w2-complete` | + 웨이브 시스템, 오브젝트 풀링 |
| `w3-step1` | + GameManager 상태 머신, HUD |
| `w3-complete` | + 폴리시, 빌드 설정 완료 |

---

## 폴더 구조

```
Assets/
  Scripts/
    Player/     PlayerController, WeaponSystem
    Enemy/      EnemyAI
    Core/       GameManager, Health, IDamageable, WaveSpawner, WaveData
    UI/         HUDController
  Prefabs/  Scenes/  Data/  Materials/  Audio/

docs/         수업 자료 (세팅 가이드, 프롬프트 카드, 트러블슈팅)
tools/        환경 점검 · AI 패키지 정리 스크립트
reference/    필요 패키지 목록
CLAUDE.md     ← Claude Code가 읽는 프로젝트 규약. 1주차에 함께 다듬습니다
```

`Assets/Scripts/` 하위는 **비어 있습니다.** 여러분이 Claude Code와 함께 채워 넣을 자리입니다.

`Assets/Scenes/Arena.unity` 는 **NavMesh까지 구워진 상태로 이미 들어 있습니다.**
아레나를 만드느라 시간을 쓰지 않고, 첫 시간부터 바로 코드 작성에 들어갑니다.

---

## 수업 자료

- **[docs/00-SETUP.md](docs/00-SETUP.md)** — 0주차 환경 세팅 (수업 전 필수)
- **[docs/01-WEEK1.md](docs/01-WEEK1.md)** — **1주차 실습 진행** (이동 + 사격) ← 수업 당일 이 문서를 엽니다
- **[docs/02-WEEK2.md](docs/02-WEEK2.md)** — **2주차 실습 진행** (적 AI + 웨이브)
- **[docs/03-WEEK3.md](docs/03-WEEK3.md)** — **3주차 실습 진행** (상태 머신 + HUD + AI 코드 리뷰)
- **[docs/PROMPT-CARDS.md](docs/PROMPT-CARDS.md)** — 검증된 프롬프트 카드 9장
- **[docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)** — 자주 나는 문제와 해결
- **[docs/INSTRUCTOR.md](docs/INSTRUCTOR.md)** — 교수·조교용 운영 노트
- **[reference/PACKAGES.md](reference/PACKAGES.md)** — 필요 패키지 / 제거할 패키지

---

## 제출물

| 항목 | 형식 |
|---|---|
| 게임 빌드 | Windows `.exe` (zip) |
| 소스 코드 | 이 리포지토리 URL |
| **프롬프트 로그** | `docs/my-prompts.md` 에 사용한 프롬프트 전문 기록 |
| 회고 | `docs/retrospective.md` — AI 협업 중 실패한 사례와 원인 분석 1쪽 |

### 평가 배점

| 항목 | 배점 |
|---|---|
| 게임 완성도 | 25 |
| **프롬프트 품질** | 25 |
| **코드 이해도** (구두 질의) | 25 |
| 독자 기능 확장 | 15 |
| 회고 | 10 |

프롬프트 품질과 코드 이해도가 절반입니다.
**"AI가 만들어준 결과물"이 아니라 "AI를 다룬 과정"을 평가합니다.**
구두 질의에서 "이 코드가 왜 이렇게 짜여 있나요?"를 물어봅니다. 모르는 코드를 커밋하지 마세요.

---

## 규칙 세 가지

1. **3회 시도해도 안 되면 되돌린다** — `git checkout .` 후 다시 요청하세요. 무한 루프에 시간을 쓰지 마세요.
2. **플레이 테스트는 사람이 한다** — AI가 "정상 동작합니다"라고 해도 직접 눌러보기 전엔 믿지 마세요.
3. **모르는 코드는 커밋하지 않는다** — 이해가 안 되면 Claude에게 "이 코드를 한 줄씩 설명해줘"라고 물어보세요.

---

## 라이선스

교육 목적으로 자유롭게 사용·수정·배포할 수 있습니다.
