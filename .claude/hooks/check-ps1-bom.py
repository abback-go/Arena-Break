#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""PowerShell 스크립트에 UTF-8 BOM을 자동으로 붙인다.

Windows PowerShell 5.1은 BOM이 없는 UTF-8 파일을 cp949로 읽는다.
그래서 한글 주석이 든 .ps1 을 BOM 없이 저장하면 글자가 깨지고,
깨진 글자 때문에 파서가 엉뚱한 곳에서 문법 오류를 낸다.

    + }
    + ~
    Unexpected token '}' in expression or statement.

파일을 열어봐도 멀쩡해 보이고 실행해야만 드러나므로, 편집 직후에 잡는다.
PostToolUse(Write|Edit) 훅으로 호출된다.
"""
import json
import os
import sys

BOM = b"\xef\xbb\xbf"


def main() -> int:
    try:
        payload = json.load(sys.stdin)
    except Exception:
        return 0  # 훅은 절대 작업을 막지 않는다

    path = (payload.get("tool_input") or {}).get("file_path")
    if not path or not path.lower().endswith(".ps1"):
        return 0
    if not os.path.isfile(path):
        return 0

    with open(path, "rb") as f:
        data = f.read()

    if data.startswith(BOM):
        return 0

    try:
        data.decode("utf-8")
    except UnicodeDecodeError:
        # UTF-8이 아닌 파일에 BOM을 붙이면 더 깨진다. 손대지 않고 알리기만 한다
        print("[ps1-bom] %s 가 UTF-8이 아닙니다. 자동 수정하지 않았습니다. "
              "인코딩을 UTF-8로 다시 저장하세요." % os.path.basename(path))
        return 0

    with open(path, "wb") as f:
        f.write(BOM + data)

    print("[ps1-bom] %s 에 UTF-8 BOM을 붙였습니다. "
          "PowerShell 5.1이 한글을 cp949로 읽는 문제를 막습니다." % os.path.basename(path))
    return 0


if __name__ == "__main__":
    sys.exit(main())
