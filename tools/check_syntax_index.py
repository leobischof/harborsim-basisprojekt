# Prüft den Syntax-Index in README.md: Steht der Code-Ausschnitt wirklich in der angegebenen Zeile?
# Aufruf: python tools/check_syntax_index.py [--commits]
# --commits prüft zusätzlich, ob der angegebene Commit der erste ist, der den Ausschnitt
# in diese Datei geschrieben hat (braucht die volle Git-Historie).
# Exit-Code 1, sobald ein Eintrag nicht stimmt.
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
# | Thema | Python | [Datei:Zeile](pfad#L12) | `Code` | Commit |
ROW = re.compile(r"\]\(([^)#]+)#L(\d+)\)\s*\|\s*`([^`]+)`\s*\|\s*([0-9a-f]{7})\s*\|")


def first_commit(path, snippet):
    """Ältester Commit, der den Ausschnitt in diese Datei gebracht hat."""
    result = subprocess.run(["git", "log", "--format=%h", "--abbrev=7", "--reverse", "-S", snippet, "--", path],
                            cwd=ROOT, capture_output=True, text=True, encoding="utf-8")
    lines = result.stdout.split()
    return lines[0] if lines else None


def main():
    check_commits = "--commits" in sys.argv
    readme = (ROOT / "README.md").read_text(encoding="utf-8")
    errors = 0
    count = 0
    for match in ROW.finditer(readme):
        path, line_number, snippet, commit = match.group(1), int(match.group(2)), match.group(3), match.group(4)
        count += 1
        file = ROOT / path
        if not file.exists():
            print(f"FEHLT   {path}")
            errors += 1
            continue
        lines = file.read_text(encoding="utf-8").split("\n")
        if line_number > len(lines) or snippet not in lines[line_number - 1]:
            print(f"FALSCH  {path}:{line_number} enthält nicht `{snippet}`")
            errors += 1
            continue
        if check_commits:
            found = first_commit(path, snippet)
            if found != commit:
                print(f"COMMIT  {path}:{line_number} `{snippet}`: angegeben {commit}, tatsächlich {found}")
                errors += 1
    print(f"{count - errors} von {count} Einträgen OK")
    sys.exit(1 if errors or count == 0 else 0)


if __name__ == "__main__":
    main()
