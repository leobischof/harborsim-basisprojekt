# Erzeugt eine kommentarfreie Kopie des Projekts (für die Prüfung, dort sind Kommentare verboten).
# Aufruf: python tools/strip_comments.py <zielordner>
# Kopiert HarborSim.sln, .gitignore, .gitattributes und den Ordner HarborSim/,
# entfernt in allen .cs-Dateien // und /* */ Kommentare und schreibt eine kurze README.
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
TOP_FILES = ["HarborSim.sln", ".gitignore", ".gitattributes"]
SKIP_DIRS = {"bin", "obj"}

README = """# HarborSim – ohne Kommentare

Automatisch erzeugt aus dem Branch `main` (tools/strip_comments.py).
Hier nichts von Hand ändern, der Branch wird bei jedem Push auf `main` überschrieben.

Starten: `dotnet run --project HarborSim`
"""


def strip_code(code):
    """Entfernt Kommentare, lässt Strings und Zeichen ('x') unverändert."""
    out = []
    i, n = 0, len(code)
    while i < n:
        c = code[i]
        nxt = code[i + 1] if i + 1 < n else ""
        if c == "/" and nxt == "/":
            # Zeilenkommentar: bis zum Zeilenende überspringen, den Zeilenumbruch behalten
            while i < n and code[i] != "\n":
                i += 1
        elif c == "/" and nxt == "*":
            end = code.find("*/", i + 2)
            i = n if end == -1 else end + 2
        elif c == "@" and nxt == '"' or c == "$" and code[i + 1:i + 3] == '@"' or c == "@" and code[i + 1:i + 3] == '$"':
            # Verbatim-String: "" ist ein Anführungszeichen, Backslash hat keine Bedeutung
            start = i
            i = code.index('"', i) + 1
            while i < n:
                if code[i] == '"' and i + 1 < n and code[i + 1] == '"':
                    i += 2
                elif code[i] == '"':
                    i += 1
                    break
                else:
                    i += 1
            out.append(code[start:i])
        elif c == '"' or c == "'":
            # Normaler String oder char, \ maskiert das nächste Zeichen
            start = i
            i += 1
            while i < n and code[i] != c and code[i] != "\n":
                i += 2 if code[i] == "\\" else 1
            i += 1
            out.append(code[start:i])
        else:
            out.append(c)
            i += 1
    return "".join(out)


def strip_file(text):
    """Entfernt Kommentare und Zeilen, die nur aus einem Kommentar bestanden."""
    original = text.split("\n")
    stripped = strip_code(text).split("\n")
    lines = []
    for before, after in zip(original, stripped):
        after = after.rstrip()
        if after == "" and before.strip() != "":
            continue
        lines.append(after)
    # Keine doppelten Leerzeilen und keine Leerzeile direkt nach "{" oder vor "}"
    result = []
    for line in lines:
        if line == "" and (not result or result[-1] == "" or result[-1].strip() == "{"):
            continue
        if line.strip() == "}" and result and result[-1] == "":
            result.pop()
        result.append(line)
    return "\n".join(result).rstrip("\n") + "\n"


def copy_project(target):
    if target.exists():
        shutil.rmtree(target)
    target.mkdir(parents=True)
    for name in TOP_FILES:
        shutil.copy2(ROOT / name, target / name)
    for src in (ROOT / "HarborSim").rglob("*"):
        rel = src.relative_to(ROOT)
        if src.is_dir() or SKIP_DIRS.intersection(rel.parts):
            continue
        dst = target / rel
        dst.parent.mkdir(parents=True, exist_ok=True)
        if src.suffix == ".cs":
            text = src.read_text(encoding="utf-8").replace("\r\n", "\n")
            dst.write_text(strip_file(text), encoding="utf-8", newline="\n")
        else:
            shutil.copy2(src, dst)
    (target / "README.md").write_text(README, encoding="utf-8", newline="\n")


if __name__ == "__main__":
    if len(sys.argv) != 2:
        sys.exit("Aufruf: python tools/strip_comments.py <zielordner>")
    copy_project(Path(sys.argv[1]).resolve())
