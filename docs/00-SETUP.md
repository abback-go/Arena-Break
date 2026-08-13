# 0주차 — 환경 세팅 가이드

> **수업 전에 반드시 끝내세요.** 세팅이 안 된 채로 오면 3시간 수업 내내 아무것도 하지 못합니다.
> 막히면 혼자 붙들지 말고 **온라인 세팅 클리닉**에 오거나 학습 게시판에 스크린샷과 함께 질문하세요.

예상 소요 시간: **40분~1시간** (Unity 설치 시간 제외)

---

## 체크리스트

작업하면서 하나씩 채우세요.

```
□ 1. Unity Hub + Unity 6.3 LTS 설치 (Windows Build Support 포함)
□ 2. Git 설치 + GitHub 계정
□ 3. Node.js LTS 설치
□ 4. Claude Code 설치 및 로그인
□ 5. Python 3.10+ 및 uv 설치
□ 6. 스타터 프로젝트 복제 후 Unity로 열기 (콘솔 에러 0개)
□ 7. MCP for Unity 창 열기 → Start Server → Configure All Detected Clients → Auto-Start 체크
□ 8. 인증 스크린샷 3장 제출
```

---

## 1. Unity 설치

### 1-1. Unity Hub 설치

<https://unity.com/download> 에서 Unity Hub를 받아 설치합니다.

### 1-2. Unity 6.3 LTS 설치

Unity Hub → **Installs** → **Install Editor** → **Unity 6.3 LTS (6000.3.x)** 선택

설치 옵션에서 아래를 **반드시 체크**하세요.

- ✅ **Windows Build Support (IL2CPP)** ← 3주차 빌드에 필요합니다
- ✅ Microsoft Visual Studio Community (이미 있으면 생략 가능)
- ✅ Documentation

> **버전 주의**: 6.5나 6.4가 아니라 **6.3 LTS** 입니다.
> 다른 버전으로 열면 프로젝트 업그레이드 프롬프트가 뜨고, 수업 화면과 달라집니다.
> 이미 다른 버전이 깔려 있어도 상관없습니다. Unity Hub는 여러 버전을 동시에 관리합니다.

설치는 20~40분 걸립니다. 그동안 아래 2~5번을 진행하세요.

---

## 2. Git + GitHub

### 2-1. Git 설치

- **Windows**: <https://git-scm.com/download/win> — 설치 중 옵션은 전부 기본값으로 두면 됩니다
- **macOS**: 터미널에서 `git --version` 실행 → 없으면 자동으로 설치 안내가 뜹니다

설치 확인:

```bash
git --version
# git version 2.4x.x  같은 출력이 나오면 성공
```

### 2-2. 사용자 정보 설정

```bash
git config --global user.name "홍길동"
git config --global user.email "본인@메일주소"
```

### 2-3. GitHub 계정

<https://github.com> 에서 계정을 만듭니다. 이미 있으면 그대로 사용하세요.

> **팁**: <https://education.github.com/pack> 에서 학생 인증을 하면 GitHub Student Developer Pack을 무료로 받을 수 있습니다. 필수는 아닙니다.

---

## 3. Node.js

<https://nodejs.org> 에서 **LTS** 버전을 받아 설치합니다.

```bash
node --version
# v20.x.x 또는 v22.x.x
```

---

## 4. Claude Code

### 4-1. 설치

```bash
npm install -g @anthropic-ai/claude-code
```

### 4-2. 실행 및 로그인

```bash
claude
```

처음 실행하면 브라우저가 열리고 로그인을 요구합니다. 안내에 따라 진행하세요.

### 4-3. 동작 확인

`claude`를 실행한 상태에서 아래를 입력해 봅니다.

```
안녕, 지금 어떤 폴더에서 실행 중이야?
```

답변이 돌아오면 성공입니다. `/exit` 또는 `Ctrl+C` 두 번으로 종료합니다.

> **이 화면을 스크린샷으로 찍어두세요.** (인증 제출용)

---

## 5. Python + uv

MCP for Unity가 Python으로 동작합니다.

### 5-1. Python 3.10 이상

```bash
python --version    # Windows
python3 --version   # macOS / Linux
```

3.10 미만이거나 없으면 <https://www.python.org/downloads/> 에서 설치하세요.
Windows 설치 화면에서 **"Add python.exe to PATH"** 를 체크한 뒤 설치합니다.

### 5-2. uv 설치

```bash
# Windows (PowerShell)
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"

# macOS / Linux
curl -LsSf https://astral.sh/uv/install.sh | sh
```

설치 후 **터미널을 새로 열고** 확인합니다.

```bash
uv --version
```

---

## 6. 프로젝트 복제 및 열기

### 6-1. 내 리포 만들기

스타터 리포 페이지 상단의 **`Use this template` → `Create a new repository`** 를 클릭합니다.

- Repository name: `arena-break`
- 공개 범위: **Public** (수업 운영상 공개로 합니다)

### 6-2. 클론

```bash
# <내계정> 부분만 본인 GitHub 아이디로 바꾸세요
git clone https://github.com/<내계정>/arena-break.git
cd arena-break

# 아래 두 줄은 그대로 복사
git remote add upstream https://github.com/abback-go/Arena-Break.git
git fetch upstream --tags
```

### 6-3. Unity로 열기

1. Unity Hub → **Projects** → **Add** → `arena-break` 폴더 선택
2. 버전 드롭다운에서 **6.3 LTS** 확인
3. 프로젝트를 엽니다 — 첫 임포트는 **3~10분** 걸립니다

### 6-4. 콘솔 확인

`Window → General → Console` 을 열고 **에러(빨간색)가 0개**인지 확인합니다.
노란색 경고는 있어도 괜찮습니다.

> **이 화면을 스크린샷으로 찍어두세요.** (인증 제출용)

에러가 있다면 → [TROUBLESHOOTING.md](TROUBLESHOOTING.md) 를 확인하세요.

---

## 7. MCP for Unity 연결

Claude Code가 Unity 에디터와 직접 대화할 수 있게 해주는 다리입니다.

### 7-1. 설치 확인

**패키지는 따로 설치하지 않습니다.** 스타터 프로젝트에 버전이 지정되어 있어,
프로젝트를 처음 열 때 Unity가 자동으로 받습니다(1~3분).

```
Window → Package Management → Package Manager → In Project
```

목록에 **MCP for Unity** 가 있으면 됩니다.
동작에 필요한 것은 앞서 설치한 **Python 3.10+ 와 uv** 입니다.

### 7-2. 창 열기 · 서버 시작 · 클라이언트 등록

`Window → MCP for Unity` 는 하위 메뉴입니다. 창은 `Toggle MCP Window` 로 엽니다.

```
1. Window → MCP for Unity → Toggle MCP Window     (단축키 Ctrl+Shift+M / ⌘⇧M)
2. Connect 탭 (기본 선택)
3. Server → Local Server 줄의 [Start Server] 클릭
4. Client Configuration → [Configure All Detected Clients] 클릭
```

Claude Code가 목록에 뜨고 상태 표시가 연결됨으로 바뀌면 됩니다.

`Configure All Detected Clients` 는 실행 이력이 있는 클라이언트만 잡습니다.
Claude Code가 안 보이면 터미널에서 `claude` 를 한 번 실행한 뒤 다시 누릅니다.

### 7-3. Auto-Start 켜기

이 설정이 꺼져 있으면 Unity를 열 때마다 `Start Server` 를 눌러야 합니다.

```
MCP for Unity 창 → Advanced 탭 → Auto-Start Server on Editor Load 체크
```

이 값은 PC별 설정이라 프로젝트에 저장되지 않습니다. 본인 PC에서 한 번 켜두면 됩니다.

### 7-4. 연결 테스트

> ### 순서가 중요합니다 — Claude Code는 마지막에 실행
> Claude Code는 **시작할 때 MCP 설정을 한 번 읽습니다.**
> 그래서 `Start Server` 보다 먼저 켜 두면 Unity 도구를 잡지 못합니다.
>
> ```
> Unity 열기 → Start Server → Configure All Detected Clients → 그다음 claude 실행
> ```
>
> 이미 `claude` 가 떠 있었다면 **종료했다가 다시 실행**하세요.

프로젝트 폴더에서 `claude`를 실행하고 아래를 물어보세요.

```
Unity 콘솔에 지금 어떤 메시지가 있는지 읽어줘.
```

응답 첫 줄에 **`Called UnityMCP`** 가 뜨고 Unity 콘솔 내용을 읽어오면 성공입니다.

한 번 더 확인:

```
현재 열려 있는 씬의 오브젝트 목록을 알려줘.
```

`Arena`, `Floor`, `Wall`, `SpawnPoints` 가 나오면 완료입니다.

---

## 8. 인증 제출

아래 스크린샷 **3장**을 학습 게시판에 제출하세요.

1. **Unity 버전 화면** — Unity Hub의 Installs 탭에서 6.3 LTS가 보이는 화면
2. **Claude Code 실행 화면** — 4-3에서 찍은 응답 화면
3. **콘솔 클린 화면** — 6-4에서 찍은 에러 0개 화면

---

## 자주 막히는 지점

| 증상 | 해결 |
|---|---|
| `uv: command not found` | 터미널을 완전히 닫았다 다시 여세요. PATH 반영에 재시작이 필요합니다 |
| `claude: command not found` | `npm install -g @anthropic-ai/claude-code` 를 다시 실행. Windows는 관리자 권한 터미널로 |
| Unity 임포트가 10분 넘게 안 끝남 | 백신 실시간 검사가 원인인 경우가 많습니다. 프로젝트 폴더를 예외로 등록해 보세요 |
| MCP 목록에 Claude Code가 안 뜸 | Claude Code를 한 번이라도 실행한 적이 있어야 감지됩니다. 먼저 `claude`를 실행하세요 |
| 콘솔에 빨간 에러 | [TROUBLESHOOTING.md](TROUBLESHOOTING.md) 참조 |

더 자세한 내용은 **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** 에 있습니다.
