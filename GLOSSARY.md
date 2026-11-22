# HarborSim

Fachsprache der Hafensimulation. Englisch ist der Name im Code (laut Klassendiagramm), Deutsch der
Begriff in Ausgaben und Erklärungen. „Python“ sagt, wie man es in Python am ehesten bauen würde.

## Hafen

**Harbor** (Hafen):
Der Ort der Simulation mit einer festen Anzahl Docks, den Schiffen des Tages und dem Personal.
Python: Klasse mit Listen für offene, laufende und fertige Tasks.
_Avoid_: Port, Terminal

**Dock** (Liegeplatz):
Ein Platz, an dem genau ein Schiff festmachen kann; frei oder belegt.
Python: ein Eintrag einer Liste fester Länge, `None` = frei.
_Avoid_: Pier, Berth, Kai

## Schiffe

**Ship** (Schiff):
Ein angekündigtes Schiff mit Länge, Breite und Tiefgang; nie direkt, sondern immer als Frachtschiff
oder Passagierschiff.
Python: abstrakte Klasse (`ABC`).
_Avoid_: Vessel, Boot

**CargoShip** (Frachtschiff):
Ein Schiff, das Container lädt; braucht beim Beladen zusätzlich Kranführer.
Python: Unterklasse `class CargoShip(Ship)`.
_Avoid_: Containerschiff, Frachter

**PassengerShip** (Passagierschiff):
Ein Schiff, das Passagiere an Bord nimmt; braucht beim Beladen zusätzlich Passagierkoordinatoren.
Python: Unterklasse `class PassengerShip(Ship)`.
_Avoid_: Kreuzfahrtschiff, Fähre

**Angedockt** (IsDocked):
Zustand eines Schiffs, das an einem Dock festgemacht hat, vom Start des Dockings bis zum Ende des
Undockings.
_Avoid_: angelegt, im Hafen

**Beschäftigt** (IsBusy):
Zustand eines Schiffs oder Mitarbeiters, der gerade an einem laufenden Task beteiligt ist.
_Avoid_: belegt, aktiv

## Personal

**Worker** (Mitarbeiter):
Eine Person aus dem Personal mit genau einem Skilltyp und einem Level.
Python: Klasse mit `__lt__`, damit `sort()` nach Level sortiert.
_Avoid_: Arbeiter (allgemein), Angestellter, Employee

**SkillType** (Skilltyp):
Die Tätigkeit, die ein Mitarbeiter kann: Pilot (Lotse), DockWorker (Hafenarbeiter), CraneOperator
(Kranführer) oder PassengerCoordinator (Passagierkoordinator).
Python: `class SkillType(Enum)`.
_Avoid_: Rolle, Beruf

**SkillLevel** (Level):
Die Qualifikationsstufe: Beginner (0), Qualified (1, qualifiziert) oder Expert (2, Experte). Ein
höheres Level darf ein niedrigeres ersetzen.
Python: `class SkillLevel(IntEnum)`.
_Avoid_: Stufe, Rang

**StaffPool** (Personal):
Alle Mitarbeiter, die in diesem Simulationslauf eingeplant werden können.
Python: Klasse mit einer Liste von Workern.
_Avoid_: Belegschaft, Team

**Personalbedarf** (SkillMatrix):
Wie viele Mitarbeiter je Skilltyp und Level ein Task braucht.
Python: Liste von Listen bzw. `numpy`-Matrix, Zeile = Skilltyp, Spalte = Level.
_Avoid_: Anforderung, Requirement

**Personalauswahl** (TrySelect):
Das Zusammenstellen der Mitarbeiter für einen Task, immer mit dem niedrigsten noch ausreichenden
Level und nur ganz oder gar nicht.
_Avoid_: Zuteilung, Reservierung

## Simulation

**Task** (Arbeitsschritt):
Ein Arbeitsschritt an einem Schiff mit Typ, Mitarbeitern, Start- und Endzeit.
Python: Klasse; heißt im Code `Task` und hat nichts mit `asyncio.Task` zu tun.
_Avoid_: Auftrag, Job, Aufgabe

**TaskType** (Tasktyp):
Docking (Anlegen), Loading (Beladen) oder Undocking (Ablegen); jedes Schiff durchläuft alle drei in
dieser Reihenfolge.
Python: `class TaskType(Enum)`.
_Avoid_: Phase, Schritt

**Simulationslauf**:
Ein Durchgang von der Startzeit, bis alle Tasks fertig sind oder die Simulation blockiert ist.
_Avoid_: Runde, Durchlauf

**Startzeit**:
Der nächste Werktag (Montag bis Freitag) um 06:00, an dem der Simulationslauf beginnt.
_Avoid_: Schichtbeginn

**Blockiert**:
Zustand, in dem noch Tasks offen sind, aber keiner läuft und keiner mehr starten kann.
_Avoid_: Deadlock, hängen geblieben

**Verfügbarkeit**:
Ob ein Mitarbeiter anwesend bzw. ein Schiff eingeplant ist; wird im Menü vor dem Simulationslauf
umgeschaltet.
Python: `dict[str, bool]` (Name → verfügbar).
_Avoid_: Status, Anwesenheit
