#!/usr/bin/env python3
"""
strip-unity-ai.py — Unity AI Assistant 계열 패키지 제거

ARENA BREAK 프로젝트에서는 Unity AI Assistant를 사용하지 않는다.
MCP for Unity와 System.Collections.Immutable 버전이 충돌하기 때문이다.
  Unity AI Assistant -> v10
  MCP for Unity      -> v9  (CodeAnalysis 의존)
  Unity 내장         -> v8
Unity에는 NuGet 같은 의존성 리졸버가 없어 자동 해결되지 않는다.

이 스크립트가 하는 일:
  1. Packages/manifest.json 에서 AI Assistant 계열 패키지 제거
  2. Packages/packages-lock.json 삭제 (안 지우면 의존성으로 되살아남)
  3. Library/ 삭제 (캐시 초기화)

com.unity.ai.navigation 은 NavMesh 패키지이므로 반드시 보존한다.

사용법:
    python tools/strip-unity-ai.py .            # 실행
    python tools/strip-unity-ai.py . --dry-run  # 무엇이 바뀔지만 확인
    python tools/strip-unity-ai.py . --keep-library
"""

from __future__ import annotations

import argparse
import json
import shutil
import sys
from pathlib import Path

# 제거 대상 (정확히 일치)
BLOCKLIST = {
    "com.unity.ai.assistant",
    "com.unity.ai.inference",
    "com.unity.ai.generators",
    "com.unity.ai.toolkit",
    "com.unity.ai.material",
    "com.unity.ai.animate",
    "com.unity.ai.sound",
    "com.unity.ai.texture",
    "com.unity.asset-manager-for-unity",
}

# com.unity.ai.* 중에서도 절대 건드리면 안 되는 것
KEEPLIST = {
    "com.unity.ai.navigation",  # NavMesh — 2주차 적 AI에 필수
}

OK = "[OK]"
CHANGED = "[변경]"
WARN = "[주의]"
FAIL = "[실패]"


def should_remove(pkg: str) -> bool:
    if pkg in KEEPLIST:
        return False
    if pkg in BLOCKLIST:
        return True
    # 알려지지 않은 신규 AI 패키지도 걸러낸다 (navigation 제외)
    return pkg.startswith("com.unity.ai.")


def main() -> int:
    ap = argparse.ArgumentParser(
        description="Unity AI Assistant 계열 패키지를 제거합니다 (AI Navigation은 보존)."
    )
    ap.add_argument("project", nargs="?", default=".", help="Unity 프로젝트 루트 경로")
    ap.add_argument("--dry-run", action="store_true", help="변경하지 않고 결과만 출력")
    ap.add_argument("--keep-library", action="store_true", help="Library/ 를 삭제하지 않음")
    args = ap.parse_args()

    root = Path(args.project).resolve()
    manifest_path = root / "Packages" / "manifest.json"
    lock_path = root / "Packages" / "packages-lock.json"
    library_path = root / "Library"

    print(f"\n프로젝트: {root}")
    if args.dry_run:
        print("모드: DRY RUN — 아무것도 변경하지 않습니다\n")
    else:
        print()

    if not manifest_path.exists():
        print(f"{FAIL} Packages/manifest.json 을 찾을 수 없습니다.")
        print("      Unity 프로젝트 루트에서 실행했는지 확인하세요.")
        print("      (Unity Hub로 프로젝트를 먼저 생성해야 합니다)")
        return 1

    # ---- 1. manifest.json ----
    try:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as e:
        print(f"{FAIL} manifest.json 파싱 실패: {e}")
        return 1

    deps = manifest.get("dependencies")
    if not isinstance(deps, dict):
        print(f"{FAIL} manifest.json 에 dependencies 가 없습니다.")
        return 1

    removed = [p for p in sorted(deps) if should_remove(p)]
    kept_ai = [p for p in sorted(deps) if p in KEEPLIST]

    if removed:
        for pkg in removed:
            print(f"{CHANGED} 제거: {pkg}  ({deps[pkg]})")
        if not args.dry_run:
            for pkg in removed:
                deps.pop(pkg, None)
            manifest_path.write_text(
                json.dumps(manifest, indent=2, ensure_ascii=False) + "\n",
                encoding="utf-8",
            )
            print(f"{OK} manifest.json 갱신 완료")
    else:
        print(f"{OK} 제거할 AI 패키지가 없습니다 (이미 정리된 상태)")

    for pkg in kept_ai:
        print(f"{OK} 보존: {pkg}  ← NavMesh 패키지. 제거하면 안 됩니다")

    if "com.unity.ai.navigation" not in deps:
        print(f"{WARN} com.unity.ai.navigation 이 없습니다.")
        print("      2주차 적 AI에 필요합니다.")
        print("      Package Manager → Unity Registry → AI Navigation 을 설치하세요.")

    # ---- 2. packages-lock.json ----
    if lock_path.exists():
        print(f"{CHANGED} 삭제: Packages/packages-lock.json")
        print("         (이 파일을 남기면 제거한 패키지가 되살아납니다)")
        if not args.dry_run:
            lock_path.unlink()
    else:
        print(f"{OK} packages-lock.json 없음")

    # ---- 3. Library/ ----
    if args.keep_library:
        print(f"{OK} Library/ 유지 (--keep-library)")
    elif library_path.exists():
        print(f"{CHANGED} 삭제: Library/  (Unity가 다시 생성합니다)")
        if not args.dry_run:
            try:
                shutil.rmtree(library_path)
            except OSError as e:
                print(f"{FAIL} Library/ 삭제 실패: {e}")
                print("      Unity 에디터가 실행 중이면 완전히 종료한 뒤 다시 실행하세요.")
                return 1
    else:
        print(f"{OK} Library/ 없음")

    # ---- 마무리 ----
    print()
    if args.dry_run:
        print("DRY RUN 종료. 실제로 적용하려면 --dry-run 없이 다시 실행하세요.")
    else:
        print("완료. 이제 Unity로 프로젝트를 다시 여세요.")
        print("첫 임포트는 3~10분 걸립니다. 콘솔 에러가 0개인지 확인하세요.")
    print()
    return 0


if __name__ == "__main__":
    sys.exit(main())
