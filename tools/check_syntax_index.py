# Prüft den Syntax-Index in README.md: Steht der Code-Ausschnitt wirklich in der angegebenen Zeile?
# Aufruf: python tools/check_syntax_index.py [--commits] [--fix]
# --fix trägt bei verrutschten Einträgen die neue Zeilennummer ein (nächstgelegener Treffer
# außerhalb von Kommentaren) und schreibt README.md zurück.
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


def nearest_code_line(lines, snippet, old_number):
    """Zeilennummer des Treffers, der der alten Nummer am nächsten liegt; Kommentarzeilen zählen nicht."""
    hits = [i + 1 for i, line in enumerate(lines)
            if snippet in line and not line.strip().startswith("//")]
    if not hits:
        return None
    return min(hits, key=lambda number: abs(number - old_number))


def main():
    check_commits = "--commits" in sys.argv
    fix = "--fix" in sys.argv
    readme = (ROOT / "README.md").read_text(encoding="utf-8")
    fixed_readme = readme
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
        # Ein Treffer in einer Kommentarzeile zählt nicht: der Index soll auf echten Code zeigen
        if (line_number > len(lines) or snippet not in lines[line_number - 1]
                or lines[line_number - 1].strip().startswith("//")):
            new_number = nearest_code_line(lines, snippet, line_number) if fix else None
            if new_number is None:
                print(f"FALSCH  {path}:{line_number} enthält nicht `{snippet}`")
                errors += 1
                continue
            name = Path(path).name
            old_link = f"[{name}:{line_number}]({path}#L{line_number})"
            fixed_readme = fixed_readme.replace(old_link, f"[{name}:{new_number}]({path}#L{new_number})", 1)
            print(f"KORR    {path}:{line_number} -> {new_number} `{snippet}`")
        if check_commits:
            found = first_commit(path, snippet)
            if found != commit:
                print(f"COMMIT  {path}:{line_number} `{snippet}`: angegeben {commit}, tatsächlich {found}")
                errors += 1
    if fixed_readme != readme:
        (ROOT / "README.md").write_text(fixed_readme, encoding="utf-8", newline="\n")
    print(f"{count - errors} von {count} Einträgen OK")
    sys.exit(1 if errors or count == 0 else 0)


if __name__ == "__main__":
    main()
