#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR=$(git rev-parse --show-toplevel)
HOOK_PATH="$ROOT_DIR/.git/hooks/pre-commit"
SRC="$ROOT_DIR/scripts/precommit.sh"

if [ ! -f "$SRC" ]; then
  echo "precommit script not found at $SRC" >&2
  exit 1
fi

mkdir -p "$(dirname "$HOOK_PATH")"
cp "$SRC" "$HOOK_PATH"
chmod +x "$HOOK_PATH"

echo "Installed pre-commit hook to $HOOK_PATH"
echo "Run '$SRC' manually or let git run the hook on commit."
