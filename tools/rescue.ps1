# ==========================================================
#  ARENA BREAK - 완성 시점으로 점프 (Windows PowerShell)
#  사용법: ./tools/rescue.ps1 w2-complete
#  실행 정책 오류가 나면:
#     powershell -ExecutionPolicy Bypass -File .\tools\rescue.ps1 w2-complete
#
#  실행 전에 Unity를 닫으세요. 씬 파일이 바뀌는데 Unity가
#  그것을 메모리에 들고 있으면 충돌합니다.
# ==========================================================

param([string]$Tag)

$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot

function Ok   ($m) { Write-Host "[OK]   $m" -ForegroundColor Green }
function Bad  ($m) { Write-Host "[실패] $m" -ForegroundColor Red }
function Warn ($m) { Write-Host "[주의] $m" -ForegroundColor Yellow }
function Info ($m) { Write-Host "       $m" -ForegroundColor DarkGray }

$Tags = @(
    @{ Name = 'w1-step1';    Desc = '플레이어 이동' },
    @{ Name = 'w1-complete'; Desc = '+ 사격, 탄약, 재장전, 표적' },
    @{ Name = 'w2-step1';    Desc = '+ Health / IDamageable' },
    @{ Name = 'w2-step2';    Desc = '+ 적 AI, Enemy 프리팹' },
    @{ Name = 'w2-complete'; Desc = '+ 웨이브 5개, 오브젝트 풀링' },
    @{ Name = 'w3-step1';    Desc = '+ 게임 상태 머신, HUD' },
    @{ Name = 'w3-complete'; Desc = '+ 피격 플래시, Esc 종료' }
)

Set-Location $Root

Write-Host ""
Write-Host "=========================================="
Write-Host " ARENA BREAK 점프"
Write-Host "=========================================="
Write-Host ""

if (-not $Tag) {
    Write-Host "어느 시점으로 갈지 골라서 다시 실행하세요."
    Write-Host ""
    foreach ($t in $Tags) { Write-Host ("  {0,-14} {1}" -f $t.Name, $t.Desc) }
    Write-Host ""
    Write-Host "  예) ./tools/rescue.ps1 w2-complete"
    Write-Host ""
    exit 0
}

if ($Tags.Name -notcontains $Tag) {
    Bad "'$Tag' 는 없는 태그입니다."
    Info ("쓸 수 있는 것: " + ($Tags.Name -join ', '))
    exit 1
}

# ---------- 1. upstream 확인 ----------
$Remotes = git remote
if ($Remotes -notcontains 'upstream') {
    Warn "upstream 리모트가 없어 등록합니다"
    git remote add upstream https://github.com/abback-go/Arena-Break.git
}
Ok "upstream 확인"

# ---------- 2. 태그 받기 ----------
git fetch upstream --tags --quiet
if ($LASTEXITCODE -ne 0) { Bad "태그를 받지 못했습니다. 인터넷 연결을 확인하세요"; exit 1 }
Ok "태그 받음"

# ---------- 3. 하던 작업 저장 ----------
# 커밋하지 않은 변경이 있으면 체크아웃이 막히거나 작업이 섞인다
$Dirty = git status --porcelain
if ($Dirty) {
    git add -A
    git commit -q -m "wip: 점프 전 자동 저장"
    Ok "하던 작업을 wip 커밋으로 저장했습니다"
    Info "돌아간 뒤 'git log' 에서 확인할 수 있습니다"
} else {
    Ok "저장할 변경 없음"
}

# ---------- 4. 점프 ----------
$Branch = "rescue-$Tag"
git rev-parse --verify --quiet $Branch > $null
if ($LASTEXITCODE -eq 0) {
    Warn "$Branch 브랜치가 이미 있어 그쪽으로 이동합니다"
    git checkout -q $Branch
} else {
    git checkout -q -b $Branch $Tag
}
if ($LASTEXITCODE -ne 0) { Bad "점프에 실패했습니다"; exit 1 }

$Before = git rev-parse --abbrev-ref '@{-1}' 2>$null
if (-not $Before) { $Before = 'main' }

# 태그는 이 스크립트보다 앞선 시점이라 tools/ 가 들어 있지 않다.
# 되살려두지 않으면 여기서 다시 점프할 수 없는 막다른 길이 된다
git checkout $Before -- tools/ 2>$null

# 되살린 tools/ 는 커밋까지 해둔다. 스테이징 상태로 두면 추적되지 않은 파일이 되어
# 돌아갈 때 'git checkout' 이 덮어쓰기를 거부한다
$Staged = git diff --cached --name-only
if ($Staged) {
    git commit -q -m "chore: 점프용 도구 복원"
}

Write-Host ""
Ok "$Tag 시점으로 이동했습니다  (브랜치: $Branch)"
Write-Host ""
Write-Host "다음에 할 일" -ForegroundColor Cyan
Write-Host "  1. Unity를 다시 엽니다"
Write-Host "  2. Assets/Scenes/Arena 씬을 엽니다"
Write-Host "  3. 여기서 그대로 이어서 작업하면 됩니다"
Write-Host ""
Write-Host "원래 하던 곳으로 돌아가려면:  git checkout $Before"
Write-Host "제출은 이 브랜치를 올려도 됩니다:  git push origin $Branch"
Write-Host ""
