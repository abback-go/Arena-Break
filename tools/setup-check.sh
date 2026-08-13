#!/usr/bin/env bash
# ==========================================================
#  ARENA BREAK — 환경 점검 (macOS / Linux)
#  사용법: bash tools/setup-check.sh
# ==========================================================

# 실패해도 계속 진행 (점검이 목적이므로 set -e 를 쓰지 않는다)
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PASS=0
FAIL=0

green() { printf '\033[32m%s\033[0m\n' "$1"; }
red()   { printf '\033[31m%s\033[0m\n' "$1"; }
yellow(){ printf '\033[33m%s\033[0m\n' "$1"; }

ok()   { green "[OK]   $1"; PASS=$((PASS+1)); }
bad()  { red   "[실패] $1"; FAIL=$((FAIL+1)); }
warn() { yellow "[주의] $1"; }

echo ""
echo "=========================================="
echo " ARENA BREAK 환경 점검"
echo " 프로젝트: $ROOT"
echo "=========================================="
echo ""

# ---------- 1. 명령줄 도구 ----------
echo "--- 명령줄 도구 ---"

if command -v git >/dev/null 2>&1; then
  ok "git — $(git --version)"
else
  bad "git 없음 → https://git-scm.com"
fi

if command -v node >/dev/null 2>&1; then
  ok "Node.js — $(node --version)"
else
  bad "Node.js 없음 → https://nodejs.org (LTS)"
fi

if command -v claude >/dev/null 2>&1; then
  ok "Claude Code 설치됨"
else
  bad "Claude Code 없음 → npm install -g @anthropic-ai/claude-code"
fi

PY=""
for c in python3 python; do
  if command -v "$c" >/dev/null 2>&1; then PY="$c"; break; fi
done
if [ -n "$PY" ]; then
  PYV="$($PY -c 'import sys; print("%d.%d"%sys.version_info[:2])' 2>/dev/null)"
  if $PY -c 'import sys; sys.exit(0 if sys.version_info >= (3,10) else 1)' 2>/dev/null; then
    ok "Python $PYV"
  else
    bad "Python $PYV — 3.10 이상이 필요합니다"
  fi
else
  bad "Python 없음 → https://www.python.org/downloads/"
fi

if command -v uv >/dev/null 2>&1; then
  ok "uv — $(uv --version)"
else
  bad "uv 없음 → curl -LsSf https://astral.sh/uv/install.sh | sh (설치 후 터미널 재시작)"
fi

# ---------- 2. 프로젝트 구조 ----------
echo ""
echo "--- 프로젝트 구조 ---"

for f in "CLAUDE.md" ".gitignore" "Assets"; do
  if [ -e "$ROOT/$f" ]; then ok "$f 존재"; else bad "$f 없음"; fi
done

if [ -f "$ROOT/Packages/manifest.json" ]; then
  ok "Packages/manifest.json 존재 — Unity 프로젝트로 초기화됨"
  UNITY_READY=1
else
  bad "Packages/manifest.json 없음"
  echo "       → 아직 Unity 프로젝트가 아닙니다."
  echo "         Unity Hub에서 'Universal 3D' 템플릿으로 프로젝트를 만든 뒤"
  echo "         이 스캐폴드를 그 위에 복사하세요. (docs/INSTRUCTOR.md 참조)"
  UNITY_READY=0
fi

if [ -f "$ROOT/ProjectSettings/ProjectVersion.txt" ]; then
  VER="$(grep -m1 'm_EditorVersion:' "$ROOT/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
  case "$VER" in
    6000.3.*) ok "Unity 버전 $VER (6.3 LTS)" ;;
    "")       warn "Unity 버전을 읽지 못했습니다" ;;
    *)        bad "Unity 버전 $VER — 이 수업은 6000.3.x (6.3 LTS) 기준입니다" ;;
  esac
fi

# ---------- 3. 패키지 구성 ----------
if [ "$UNITY_READY" = "1" ] && [ -n "$PY" ]; then
  echo ""
  echo "--- 패키지 구성 ---"
  "$PY" - "$ROOT" <<'PYEOF'
import json, sys
from pathlib import Path

root = Path(sys.argv[1])
deps = json.loads((root / "Packages" / "manifest.json").read_text(encoding="utf-8")).get("dependencies", {})

GREEN, RED, YELLOW, RESET = "\033[32m", "\033[31m", "\033[33m", "\033[0m"

BAD = [p for p in sorted(deps)
       if p.startswith("com.unity.ai.") and p != "com.unity.ai.navigation"]
BAD += [p for p in ("com.unity.asset-manager-for-unity",) if p in deps]

for p in BAD:
    print(f"{RED}[실패] 제거 대상 패키지가 남아 있습니다: {p}{RESET}")
if BAD:
    print("       → python tools/strip-unity-ai.py . 를 실행하세요")
else:
    print(f"{GREEN}[OK]   AI Assistant 계열 패키지 없음{RESET}")

need = {
    "com.unity.ai.navigation": "AI Navigation (NavMesh)",
    "com.unity.inputsystem": "Input System",
    "com.unity.render-pipelines.universal": "Universal RP",
}
for pid, label in need.items():
    if pid in deps:
        print(f"{GREEN}[OK]   {label}{RESET}")
    else:
        print(f"{RED}[실패] {label} 없음 → Package Manager에서 설치하세요{RESET}")

mcp = [p for p in deps if "unity-mcp" in str(deps[p]) or "MCPForUnity" in str(deps[p])]
if mcp:
    url = deps[mcp[0]]
    print(f"{GREEN}[OK]   MCP for Unity 설치됨{RESET}")
    if "#" not in url:
        print(f"{YELLOW}[주의] MCP 버전이 고정되어 있지 않습니다 (#태그 없음){RESET}")
        print("       학생마다 동작이 달라질 수 있습니다")
else:
    print(f"{RED}[실패] MCP for Unity 없음{RESET}")
    print("       Package Manager → + → Install package from git URL")
    print("       https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#<태그>")

if (root / "Packages" / "packages-lock.json").exists():
    print(f"{YELLOW}[주의] packages-lock.json 존재 — 의도한 것이면 무시하세요{RESET}")
PYEOF
fi

# ---------- 4. Git ----------
echo ""
echo "--- Git ---"
if [ -d "$ROOT/.git" ]; then
  ok "git 리포지토리로 초기화됨"
  if git -C "$ROOT" remote | grep -q upstream; then
    ok "upstream 등록됨 — $(git -C "$ROOT" remote get-url upstream)"
  else
    warn "upstream 미등록 — 단계별 스냅샷을 받으려면 필요합니다"
    echo "       git remote add upstream <교수리포URL>"
    echo "       git fetch upstream --tags"
  fi
  if git -C "$ROOT" ls-files --error-unmatch Library >/dev/null 2>&1; then
    bad "Library/ 가 git에 추적되고 있습니다 → git rm -r --cached Library/"
  else
    ok "Library/ 미추적"
  fi
else
  warn "git 리포지토리가 아닙니다 (git init 필요)"
fi

# ---------- 결과 ----------
echo ""
echo "=========================================="
if [ "$FAIL" -eq 0 ]; then
  green " 통과 $PASS 개 · 실패 0 개 — 준비 완료!"
else
  red   " 통과 $PASS 개 · 실패 $FAIL 개"
  echo " 위의 [실패] 항목을 해결한 뒤 다시 실행하세요."
  echo " 막히면 docs/TROUBLESHOOTING.md 를 확인하세요."
fi
echo "=========================================="
echo ""

[ "$FAIL" -eq 0 ] || exit 1
