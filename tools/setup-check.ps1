# ==========================================================
#  ARENA BREAK - 환경 점검 (Windows PowerShell)
#  사용법: ./tools/setup-check.ps1
#  실행 정책 오류가 나면:
#     powershell -ExecutionPolicy Bypass -File .\tools\setup-check.ps1
# ==========================================================

$ErrorActionPreference = 'Continue'
$Root = Split-Path -Parent $PSScriptRoot
$script:Pass = 0
$script:Fail = 0

function Ok   ($m) { Write-Host "[OK]   $m" -ForegroundColor Green;  $script:Pass++ }
function Bad  ($m) { Write-Host "[실패] $m" -ForegroundColor Red;    $script:Fail++ }
function Warn ($m) { Write-Host "[주의] $m" -ForegroundColor Yellow }
function Info ($m) { Write-Host "       $m" -ForegroundColor DarkGray }

function Has-Cmd($name) { $null -ne (Get-Command $name -ErrorAction SilentlyContinue) }

Write-Host ""
Write-Host "=========================================="
Write-Host " ARENA BREAK 환경 점검"
Write-Host " 프로젝트: $Root"
Write-Host "=========================================="
Write-Host ""

# ---------- 1. 명령줄 도구 ----------
Write-Host "--- 명령줄 도구 ---"

if (Has-Cmd git)  { Ok "git - $(git --version)" }
else { Bad "git 없음"; Info "https://git-scm.com/download/win" }

if (Has-Cmd node) { Ok "Node.js - $(node --version)" }
else { Bad "Node.js 없음"; Info "https://nodejs.org (LTS)" }

if (Has-Cmd claude) { Ok "Claude Code 설치됨" }
else {
    Bad "Claude Code 없음"
    Info "npm install -g @anthropic-ai/claude-code"
    Info "권한 오류가 나면 관리자 권한 PowerShell로 실행하세요"
}

$Py = $null
foreach ($c in @('python','py','python3')) { if (Has-Cmd $c) { $Py = $c; break } }
if ($Py) {
    $pv = & $Py -c "import sys; print('%d.%d' % sys.version_info[:2])" 2>$null
    $okv = & $Py -c "import sys; print(1 if sys.version_info >= (3,10) else 0)" 2>$null
    if ($okv -eq '1') { Ok "Python $pv" }
    else { Bad "Python $pv - 3.10 이상이 필요합니다" }
} else {
    Bad "Python 없음"
    Info "https://www.python.org/downloads/  (설치 시 'Add python.exe to PATH' 체크)"
}

if (Has-Cmd uv) { Ok "uv - $(uv --version)" }
else {
    Bad "uv 없음"
    Info 'powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"'
    Info "설치 후 터미널을 새로 열어야 인식됩니다"
}

# ---------- 2. 프로젝트 구조 ----------
Write-Host ""
Write-Host "--- 프로젝트 구조 ---"

foreach ($f in @('CLAUDE.md', '.gitignore', 'Assets')) {
    if (Test-Path (Join-Path $Root $f)) { Ok "$f 존재" } else { Bad "$f 없음" }
}

$ManifestPath = Join-Path $Root 'Packages\manifest.json'
$UnityReady = Test-Path $ManifestPath
if ($UnityReady) {
    Ok "Packages/manifest.json 존재 - Unity 프로젝트로 초기화됨"
} else {
    Bad "Packages/manifest.json 없음"
    Info "아직 Unity 프로젝트가 아닙니다."
    Info "Unity Hub에서 'Universal 3D' 템플릿으로 프로젝트를 만든 뒤"
    Info "이 스캐폴드를 그 위에 복사하세요. (docs/INSTRUCTOR.md 참조)"
}

$VerPath = Join-Path $Root 'ProjectSettings\ProjectVersion.txt'
if (Test-Path $VerPath) {
    $line = Select-String -Path $VerPath -Pattern 'm_EditorVersion:' | Select-Object -First 1
    if ($line) {
        $ver = ($line.Line -split '\s+')[1]
        if ($ver -like '6000.3.*') { Ok "Unity 버전 $ver (6.3 LTS)" }
        else { Bad "Unity 버전 $ver - 이 수업은 6000.3.x (6.3 LTS) 기준입니다" }
    }
}

# ---------- 3. 패키지 구성 ----------
if ($UnityReady) {
    Write-Host ""
    Write-Host "--- 패키지 구성 ---"

    $manifest = Get-Content $ManifestPath -Raw | ConvertFrom-Json
    $deps = $manifest.dependencies
    $names = $deps.PSObject.Properties.Name

    $badPkgs = @($names | Where-Object {
        ($_ -like 'com.unity.ai.*' -and $_ -ne 'com.unity.ai.navigation') -or
        ($_ -eq 'com.unity.asset-manager-for-unity')
    })

    if ($badPkgs.Count -gt 0) {
        foreach ($p in $badPkgs) { Bad "제거 대상 패키지가 남아 있습니다: $p" }
        Info "python tools/strip-unity-ai.py . 를 실행하세요"
    } else {
        Ok "AI Assistant 계열 패키지 없음"
    }

    $need = @{
        'com.unity.ai.navigation'              = 'AI Navigation (NavMesh)'
        'com.unity.inputsystem'                = 'Input System'
        'com.unity.render-pipelines.universal' = 'Universal RP'
    }
    foreach ($pid in $need.Keys) {
        if ($names -contains $pid) { Ok $need[$pid] }
        else { Bad "$($need[$pid]) 없음 - Package Manager에서 설치하세요" }
    }

    $mcp = @($names | Where-Object { "$($deps.$_)" -match 'unity-mcp|MCPForUnity' })
    if ($mcp.Count -gt 0) {
        Ok "MCP for Unity 설치됨"
        $url = "$($deps.($mcp[0]))"
        if ($url -notmatch '#') {
            Warn "MCP 버전이 고정되어 있지 않습니다 (#태그 없음)"
            Info "학생마다 동작이 달라질 수 있습니다"
        }
    } else {
        Bad "MCP for Unity 없음"
        Info "Package Manager -> + -> Install package from git URL"
        Info "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#<태그>"
    }
}

# ---------- 4. Git ----------
Write-Host ""
Write-Host "--- Git ---"
if (Test-Path (Join-Path $Root '.git')) {
    Ok "git 리포지토리로 초기화됨"
    $remotes = git -C $Root remote 2>$null
    if ($remotes -contains 'upstream') {
        Ok "upstream 등록됨 - $(git -C $Root remote get-url upstream)"
    } else {
        Warn "upstream 미등록 - 단계별 스냅샷을 받으려면 필요합니다"
        Info "git remote add upstream <교수리포URL>"
        Info "git fetch upstream --tags"
    }
    git -C $Root ls-files --error-unmatch Library 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Bad "Library/ 가 git에 추적되고 있습니다"
        Info "git rm -r --cached Library/"
    } else {
        Ok "Library/ 미추적"
    }
} else {
    Warn "git 리포지토리가 아닙니다 (git init 필요)"
}

# ---------- 결과 ----------
Write-Host ""
Write-Host "=========================================="
if ($script:Fail -eq 0) {
    Write-Host " 통과 $($script:Pass) 개 · 실패 0 개 - 준비 완료!" -ForegroundColor Green
} else {
    Write-Host " 통과 $($script:Pass) 개 · 실패 $($script:Fail) 개" -ForegroundColor Red
    Write-Host " 위의 [실패] 항목을 해결한 뒤 다시 실행하세요."
    Write-Host " 막히면 docs/TROUBLESHOOTING.md 를 확인하세요."
}
Write-Host "=========================================="
Write-Host ""

if ($script:Fail -ne 0) { exit 1 }
