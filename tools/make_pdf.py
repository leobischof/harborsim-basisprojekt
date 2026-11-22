# Macht aus den Angaben der Übungsprüfungen (README.md) je eine Angabe.pdf.
# Nur Standard-Python: ein kleiner Markdown->HTML-Umwandler, gedruckt wird mit Chrome im Headless-Modus.
# Aufruf: python tools/make_pdf.py              (alle Aufgaben unter uebungspruefungen/)
#         python tools/make_pdf.py <ordner> ... (nur diese Aufgaben)
# Kann nur, was die Angaben brauchen: Überschriften, Absätze, Listen, Tabellen, Codeblöcke,
# `code`, **fett** und [Links](...).
import html
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
TASKS = ROOT / "uebungspruefungen"
CHROME_CANDIDATES = [
    r"C:\Program Files\Google\Chrome\Application\chrome.exe",
    r"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
    r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
]

STYLE = """
body { font-family: "Segoe UI", Arial, sans-serif; font-size: 11pt; line-height: 1.45; margin: 0; }
h1 { font-size: 18pt; border-bottom: 2px solid #1f4e79; padding-bottom: 4px; color: #1f4e79; }
h2 { font-size: 13.5pt; color: #1f4e79; margin-top: 18px; }
h3 { font-size: 11.5pt; }
code { font-family: Consolas, monospace; font-size: 9.5pt; background: #f1f3f5; padding: 0 3px; }
pre { background: #f1f3f5; padding: 8px 10px; font-size: 9pt; line-height: 1.3;
      white-space: pre-wrap; border-left: 3px solid #1f4e79; page-break-inside: avoid; }
pre code { background: none; padding: 0; }
table { border-collapse: collapse; margin: 8px 0; }
th, td { border: 1px solid #999; padding: 3px 8px; text-align: left; }
th { background: #e7eef6; }
a { color: #1f4e79; }
"""


def inline(text):
    """Code, Fett und Links innerhalb einer Zeile."""
    parts = re.split(r"(`[^`]+`)", text)
    result = []
    for part in parts:
        if part.startswith("`") and part.endswith("`") and len(part) > 1:
            result.append("<code>" + html.escape(part[1:-1]) + "</code>")
            continue
        part = html.escape(part)
        part = re.sub(r"\*\*(.+?)\*\*", r"<strong>\1</strong>", part)
        part = re.sub(r"\[([^\]]+)\]\(([^)]+)\)", r'<a href="\2">\1</a>', part)
        result.append(part)
    return "".join(result)


def table(rows):
    """Markdown-Tabelle: erste Zeile Kopf, zweite Zeile |---|, Rest Daten."""
    cells = [[c.strip() for c in row.strip().strip("|").split("|")] for row in rows]
    out = ["<table><tr>" + "".join(f"<th>{inline(c)}</th>" for c in cells[0]) + "</tr>"]
    for row in cells[2:]:
        out.append("<tr>" + "".join(f"<td>{inline(c)}</td>" for c in row) + "</tr>")
    out.append("</table>")
    return "\n".join(out)


def to_html(markdown):
    lines = markdown.split("\n")
    out = []
    paragraph = []
    list_tag = None
    i = 0

    def flush_paragraph():
        if paragraph:
            out.append("<p>" + inline(" ".join(paragraph)) + "</p>")
            paragraph.clear()

    def flush():
        nonlocal list_tag
        flush_paragraph()
        if list_tag:
            out.append(f"</{list_tag}>")
            list_tag = None

    while i < len(lines):
        line = lines[i]
        stripped = line.strip()
        if stripped.startswith("```"):
            # eingerückter Codeblock in einer offenen Liste gehört zum letzten Listenpunkt
            in_list = list_tag is not None and line.startswith("  ")
            if not in_list:
                flush()
            indent = len(line) - len(line.lstrip())
            code = []
            i += 1
            while i < len(lines) and not lines[i].strip().startswith("```"):
                code.append(lines[i][indent:])
                i += 1
            block = "<pre><code>" + html.escape("\n".join(code)) + "</code></pre>"
            if in_list:
                out[-1] = out[-1][:-5] + block + "</li>"
            else:
                out.append(block)
        elif stripped.startswith("|"):
            flush()
            rows = []
            while i < len(lines) and lines[i].strip().startswith("|"):
                rows.append(lines[i])
                i += 1
            out.append(table(rows))
            continue
        elif re.match(r"#{1,3} ", stripped):
            flush()
            level = len(stripped.split(" ")[0])
            out.append(f"<h{level}>{inline(stripped[level + 1:])}</h{level}>")
        elif re.match(r"([-*]|\d+\.) ", stripped):
            if paragraph:
                flush()
            tag = "ol" if stripped[0].isdigit() else "ul"
            if list_tag != tag:
                flush()
                out.append(f"<{tag}>")
                list_tag = tag
            out.append("<li>" + inline(stripped.split(" ", 1)[1]) + "</li>")
        elif stripped == "":
            # Leerzeile beendet einen Absatz, aber keine Liste: die kann danach weitergehen
            flush_paragraph()
        elif list_tag and line.startswith("  "):
            # eingerückte Fortsetzung des letzten Listenpunkts
            out[-1] = out[-1][:-5] + " " + inline(stripped) + "</li>"
        else:
            if list_tag:
                flush()
            paragraph.append(stripped)
        i += 1
    flush()
    return "\n".join(out)


def find_chrome():
    for candidate in CHROME_CANDIDATES:
        if Path(candidate).exists():
            return candidate
    found = shutil.which("chrome") or shutil.which("google-chrome") or shutil.which("msedge")
    if not found:
        sys.exit("Kein Chrome/Edge gefunden.")
    return found


def make_pdf(task_dir, chrome):
    readme = task_dir / "README.md"
    page = ("<!doctype html><html lang='de'><head><meta charset='utf-8'><style>" + STYLE
            + "</style></head><body>" + to_html(readme.read_text(encoding="utf-8")) + "</body></html>")
    with tempfile.TemporaryDirectory() as temp:
        source = Path(temp) / "angabe.html"
        source.write_text(page, encoding="utf-8")
        target = task_dir / "Angabe.pdf"
        subprocess.run([chrome, "--headless", "--disable-gpu", "--no-pdf-header-footer",
                        f"--user-data-dir={Path(temp) / 'profil'}",
                        f"--print-to-pdf={target}", source.as_uri()],
                       check=True, capture_output=True)
    print(f"PDF     {target.relative_to(ROOT)}")


def main():
    chrome = find_chrome()
    if len(sys.argv) > 1:
        dirs = [Path(arg).resolve() for arg in sys.argv[1:]]
    else:
        # Aufgabenordner = Ordner mit README.md zwei Ebenen unter uebungspruefungen/
        dirs = sorted(readme.parent for readme in TASKS.glob("*/*/README.md"))
    for task_dir in dirs:
        make_pdf(task_dir, chrome)


if __name__ == "__main__":
    main()
