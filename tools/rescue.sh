#!/usr/bin/env bash
# ==========================================================
#  ARENA BREAK - 완성 시점으로 점프 (Git Bash / macOS / Linux)
#  사용법: ./tools/rescue.sh w2-complete
#
#  실행 전에 Unity를 닫으세요. 씬 파일이 바뀌는데 Unity가
#  그것을 메모리에 들고 있으면 충돌합니다.
# ==========================================================

set -u

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

GREEN='\033[0;32m'; RED='\033[0;31m'; YELLOW='\033[0;33m'; CYAN='\033[0;36m'; GRAY='\033[0;90m'; NC='\033[0m'
ok()   { printf "${GREEN}[OK]   %s${NC}\n" "$1"; }
bad()  { printf "${RED}[실패] %s${NC}\n" "$1"; }
warn() { printf "${YELLOW}[주의] %s${NC}\n" "$1"; }
info() { printf "${GRAY}       %s${NC}\n" "$1"; }

TAG_NAMES=(w1-step1 w1-complete w2-step1 w2-step2 w2-complete w3-step1 w3-complete)
TAG_DESCS=(
  "플레이어 이동"
  "+ 사격, 탄약, 재장전, 표적"
  "+ Health / IDamageable"
  "+ 적 AI, Enemy 프리팹"
  "+ 웨이브 5개, 오브젝트 풀링"
  "+ 게임 상태 머신, HUD"
  "+ 피격 플래시, Esc 종료"
)

echo
echo "=========================================="
echo " ARENA BREAK 점프"
echo "=========================================="
echo

TAG="${1:-}"

if [ -z "$TAG" ]; then
  echo "어느 시점으로 갈지 골라서 다시 실행하세요."
  echo
  for i in "${!TAG_NAMES[@]}"; do
    printf "  %-14s %s\n" "${TAG_NAMES[$i]}" "${TAG_DESCS[$i]}"
  done
  echo
  echo "  예) ./tools/rescue.sh w2-complete"
  echo
  exit 0
fi

FOUND=0
for name in "${TAG_NAMES[@]}"; do
  [ "$name" = "$TAG" ] && FOUND=1 && break
done
if [ "$FOUND" -eq 0 ]; then
  bad "'$TAG' 는 없는 태그입니다."
  info "쓸 수 있는 것: ${TAG_NAMES[*]}"
  exit 1
fi

# ---------- 1. upstream 확인 ----------
if ! git remote | grep -qx upstream; then
  warn "upstream 리모트가 없어 등록합니다"
  git remote add upstream https://github.com/abback-go/Arena-Break.git
fi
ok "upstream 확인"

# ---------- 2. 태그 받기 ----------
if ! git fetch upstream --tags --quiet; then
  bad "태그를 받지 못했습니다. 인터넷 연결을 확인하세요"
  exit 1
fi
ok "태그 받음"

# ---------- 3. 하던 작업 저장 ----------
# 커밋하지 않은 변경이 있으면 체크아웃이 막히거나 작업이 섞인다
if [ -n "$(git status --porcelain)" ]; then
  git add -A
  git commit -q -m "wip: 점프 전 자동 저장"
  ok "하던 작업을 wip 커밋으로 저장했습니다"
  info "돌아간 뒤 'git log' 에서 확인할 수 있습니다"
else
  ok "저장할 변경 없음"
fi

# ---------- 4. 점프 ----------
BRANCH="rescue-$TAG"
if git rev-parse --verify --quiet "$BRANCH" > /dev/null; then
  warn "$BRANCH 브랜치가 이미 있어 그쪽으로 이동합니다"
  git checkout -q "$BRANCH"
else
  git checkout -q -b "$BRANCH" "$TAG"
fi

BEFORE="$(git rev-parse --abbrev-ref '@{-1}' 2>/dev/null || echo main)"

# 태그는 지난 시점이라 최신 도구·문서·설정이 들어 있지 않다. 코드와 씬만 태그 것을 쓰고
# 나머지는 되살린다.
#   tools            없으면 여기서 다시 점프할 수 없는 막다른 길이 된다
#   ProjectSettings  옛것이면 빌드가 전체 화면으로 나온다 (창 모드 설정이 없다)
#   docs             옛 문서를 보고 따라가게 된다
#   CLAUDE.md        1장에서 직접 추가한 규약이 사라진다
for p in tools docs .claude ProjectSettings CLAUDE.md README.md; do
  git checkout "$BEFORE" -- "$p" 2>/dev/null || true
done

# 커밋까지 해둔다. 스테이징 상태로 두면 추적되지 않은 파일이 되어
# 돌아갈 때 'git checkout' 이 덮어쓰기를 거부한다
if [ -n "$(git diff --cached --name-only)" ]; then
  git commit -q -m "chore: 도구·문서·빌드 설정 복원"
fi

echo
ok "$TAG 시점으로 이동했습니다  (브랜치: $BRANCH)"
echo
printf "${CYAN}다음에 할 일${NC}\n"
echo "  1. Unity를 다시 엽니다"
echo "  2. Assets/Scenes/Arena 씬을 엽니다"
echo "  3. 여기서 그대로 이어서 작업하면 됩니다"
echo
echo "원래 하던 곳으로 돌아가려면:  git checkout $BEFORE"
echo "제출은 이 브랜치를 올려도 됩니다:  git push origin $BRANCH"
echo
