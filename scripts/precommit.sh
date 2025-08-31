#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR=$(git rev-parse --show-toplevel)
cd "$ROOT_DIR"

echo "[precommit] Starting pre-commit checkup: cleanup + tests"

SOLUTION=${1:-}
if [ -z "$SOLUTION" ]; then
  SOLUTION=$(ls *.sln 2>/dev/null | head -n1 || true)
fi

if [ -z "$SOLUTION" ]; then
  echo "No solution file found in repo root. Please provide path to .sln as first arg." >&2
  exit 1
fi

echo "[precommit] Using solution: $SOLUTION"

# Ensure dotnet tools path is available
export PATH="$HOME/.dotnet/tools:$PATH"

RESHARPER_TOOL_VERSION="2025.2.0.1"

run_resharper_cleanup() {
  echo "[precommit] Attempting ReSharper cleanup..."
  # Try existing binaries first
  # run cleanup with a timeout to avoid indefinite runs (300s)
  TIMEOUT_SECONDS=300
  if command -v cleanupcode >/dev/null 2>&1; then
    echo "[precommit] Found 'cleanupcode' binary - running with timeout ${TIMEOUT_SECONDS}s"
    if timeout ${TIMEOUT_SECONDS} cleanupcode "$SOLUTION"; then
      return 0
    else
      echo "[precommit] cleanupcode failed or timed out"
      return 1
    fi
  fi

  if command -v jb >/dev/null 2>&1; then
    echo "[precommit] Found 'jb' binary - running 'jb cleanupcode' with timeout ${TIMEOUT_SECONDS}s"
    if timeout ${TIMEOUT_SECONDS} jb cleanupcode "$SOLUTION"; then
      return 0
    else
      echo "[precommit] jb cleanupcode failed or timed out"
      return 1
    fi
  fi

  # Try to install the global tool if missing
  echo "[precommit] ReSharper tool not on PATH — attempting to install JetBrains.ReSharper.GlobalTools (version $RESHARPER_TOOL_VERSION)"
  dotnet tool install --global JetBrains.ReSharper.GlobalTools --version "$RESHARPER_TOOL_VERSION" || true
  export PATH="$HOME/.dotnet/tools:$PATH"

  # run installed cleanup with timeout
  TIMEOUT_SECONDS=300
  if command -v cleanupcode >/dev/null 2>&1; then
    echo "[precommit] Running installed cleanupcode with timeout ${TIMEOUT_SECONDS}s"
    if timeout ${TIMEOUT_SECONDS} cleanupcode "$SOLUTION"; then
      return 0
    else
      return 1
    fi
  fi

  if command -v jb >/dev/null 2>&1; then
    echo "[precommit] Running installed jb cleanupcode with timeout ${TIMEOUT_SECONDS}s"
    if timeout ${TIMEOUT_SECONDS} jb cleanupcode "$SOLUTION"; then
      return 0
    else
      return 1
    fi
  fi

  return 2
}

run_dotnet_format() {
  echo "[precommit] Running dotnet format as fallback (will apply changes)"
  dotnet tool install --global dotnet-format --version 6.0.250901 || true
  export PATH="$HOME/.dotnet/tools:$PATH"
  dotnet format
}

PRE_FMT_STATUS=0
if run_resharper_cleanup; then
  PRE_FMT_STATUS=0
else
  STATUS=$?
  if [ "$STATUS" -eq 2 ]; then
    echo "[precommit] ReSharper cleanup unavailable — using dotnet-format fallback"
    run_dotnet_format
  else
    echo "[precommit] ReSharper cleanup failed with status $STATUS — attempting dotnet-format fallback"
    run_dotnet_format
  fi
fi

# If formatting changed files, stage them so the commit will include formatting fixes
if [ -n "$(git status --porcelain)" ]; then
  echo "[precommit] Formatting modified files. Staging changes."
  git add -A
  echo "[precommit] Changes staged. Please review staged formatting changes before committing."
else
  echo "[precommit] No formatting changes detected."
fi

echo "[precommit] Running dotnet restore/build/tests"
dotnet restore
dotnet build --configuration Release

echo "[precommit] Running unit tests"
if ! dotnet test --configuration Release --no-build --verbosity minimal; then
  echo "[precommit] Unit tests failed. Aborting commit." >&2
  exit 1
fi

echo "[precommit] All checks passed. Ready to commit."
exit 0
