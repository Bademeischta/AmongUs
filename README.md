# MyCustomRolesMod für Among Us (v4.0.0 - Production Grade)

Dieses Projekt ist ein BepInEx/HarmonyLib-Mod für das Spiel "Among Us", der ein extrem robustes und erweiterbares System für benutzerdefinierte Rollen hinzufügt.

Diese Version zeichnet sich durch **maximale Stabilität, Sicherheit und Performance** aus. Das Herzstück ist ein eigenes Netzwerk-Protokoll, das Synchronisationsprobleme minimiert.

## Rollen

Der Mod fügt dem Spiel 6 neue Rollen hinzu (3 aktiv, 3 experimentell).

### Aktive Rollen (Im Spiel verfügbar)

Diese Rollen sind vollständig integriert und können über die Lobby-Optionen (Jester) oder die Konfigurationsdatei gesteuert werden.

*   **Jester (Narr) - Neutral**
    *   **Ziel:** Lass dich von den anderen Spielern aus dem Spiel wählen (exiled).
    *   **Gewinnbedingung:** Wenn der Jester während eines Meetings rausgewählt wird, gewinnt er sofort das Spiel.
    *   **Besonderheit:** Kann in den Lobby-Optionen konfiguriert werden ("Jester Chance").

*   **Geist - Impostor**
    *   **Ziel:** Eliminiere die Crew.
    *   **Fähigkeit:** Ersetzt den normalen Kill durch einen **Fluch (Markierung)**.
    *   **Effekt:** Ein markierter Spieler stirbt nicht sofort, sondern erst nach **45 Sekunden**. Dies verwirrt die Crew bezüglich des Zeitpunkts und Ortes des Angriffs.

*   **Echo - Crewmate**
    *   **Ziel:** Gewinne mit der Crew.
    *   **Fähigkeit:** Kann ein "infiziertes Wort" festlegen.
    *   **Befehl:** Schreibe `/infect <Wort>` in den Chat (z.B. `/infect sus`).
    *   **Effekt:** Wenn ein anderer Spieler dieses Wort im Chat verwendet, **leuchtet er kurzzeitig bläulich auf** (Shimmer-Effekt). Dies kann helfen, Trolle zu entlarven oder Informationen zu markieren.

### Experimentelle Rollen (Im Code enthalten)

Diese Rollen sind bereits technisch implementiert (inklusive UI und Networking), aber in der aktuellen Version standardmäßig **nicht** der Rotation zugewiesen. Sie demonstrieren die Möglichkeiten des Systems.

*   **Witness (Zeuge) - Crewmate**
    *   **Fähigkeit:** Hat einen speziellen Button, um ein **Testament** zu verfassen.
    *   **Effekt:** Wenn der Witness stirbt, wird sein Testament im nächsten Meeting für alle sichtbar beim toten Körper (oder im Chat) angezeigt.

*   **Puppeteer (Puppenspieler) - Neutral/Impostor**
    *   **Fähigkeit:** Kann anderen Spielern Worte in den Mund legen.
    *   **UI:** Über ein spezielles Menü kann der Puppeteer einen Spieler und eine vorgefertigte Nachricht auswählen (z.B. "I saw them vent!").
    *   **Effekt:** Im Meeting erscheint diese Nachricht so, als hätte der gewählte Spieler sie geschrieben.

*   **Glitch - Neutral/Impostor**
    *   **Fähigkeit:** Kann Schiffssysteme (Aufgaben) korrumpieren.
    *   **Effekt:** Wenn ein Crewmate versucht, eine korrumpierte Aufgabe zu lösen, wird der Fortschritt **umgekehrt** (z.B. Download-Balken läuft rückwärts).

### Zukünftige Konzepte (Roadmap)

Weitere innovative Rollenkonzepte, die sich aktuell in der Designphase befinden, sind in der Datei [Core/ProposedRoles.md](Core/ProposedRoles.md) dokumentiert. Dazu gehören unter anderem **The Relic** und **The Paradox**.

## Features

- **Robustes Netzwerkprotokoll:**
    - **ACK & Retry-System:** Garantiert die Zustellung wichtiger Nachrichten (wie Rollenzuweisung) auch bei Packet Loss.
    - **Late-Join-Support:** Spieler, die später beitreten, erhalten den korrekten Spielzustand.
    - **Versions-Validierung:** Verhindert Versionskonflikte.
- **Benutzerdefinierte UI:** Eigene Buttons und Menüs für spezielle Fähigkeiten (z.B. Witness-Testament, Puppeteer-Kontrolle).

## Installation (für Spieler)

**Wichtiger Hinweis:** Alle Spieler in der Lobby müssen exakt die gleiche Version des Mods installiert haben.

1.  **ZIP-Archiv herunterladen:** Lade die `MyCustomRolesMod-vX.X-Distribution.zip` von der Release-Seite herunter.
2.  **In das Spielverzeichnis entpacken:** Entpacke den Inhalt direkt in dein Among Us-Hauptverzeichnis.
3.  **Spiel starten:** Der Mod wird automatisch geladen.

## Konfiguration

Nach dem ersten Start wird die Datei `Among Us/BepInEx/config/com.example.mycustomrolesmod.paranoid.cfg` erstellt.
Hier können die Wahrscheinlichkeiten für **Geist** und **Echo** angepasst werden (Standard: 100%). Die Wahrscheinlichkeit für den **Jester** kann direkt in der Lobby eingestellt werden.

## Build-Anleitung (für Entwickler)

Um diesen Mod unter Windows zu kompilieren, benötigen Sie das **.NET 6.0 SDK**.

### Schritt 1: Abhängigkeiten bereitstellen

**Option A: Steam-Pfad angeben (Empfohlen)**
Der einfachste und zuverlässigste Weg ist, eine Umgebungsvariable `AMONG_US_GAME_PATH` zu setzen, die auf Ihr Spielverzeichnis zeigt.

*   **In PowerShell:**
    ```powershell
    $env:AMONG_US_GAME_PATH = "C:\Program Files (x86)\Steam\steamapps\common\Among Us"
    .\build.bat
    ```
*   **In der klassischen CMD:**
    ```cmd
    set AMONG_US_GAME_PATH="C:\Program Files (x86)\Steam\steamapps\common\Among Us"
    build.bat
    ```

**Option B: Automatische Erkennung (Fallback)**
Wenn die `AMONG_US_GAME_PATH` Variable nicht gesetzt ist, sucht das Skript automatisch in einem lokalen `bepinex_files`-Verzeichnis nach den benötigten Spieldateien.

**Zusätzliche Abhängigkeit: BepInEx**
Das Skript benötigt außerdem die BepInEx-Dateien, um das finale ZIP-Paket zu erstellen.
1.  Laden Sie `BepInEx 6.0.0-pre.1 IL2CPP` von der [offiziellen BepInEx-Release-Seite](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1) herunter.
2.  Entpacken Sie den Inhalt in einen Ordner namens `bepinex_files` im Hauptverzeichnis dieses Projekts.

### Schritt 2: Build ausführen

Nachdem alle Abhängigkeiten erfüllt sind, führen Sie das Build-Skript aus:
```cmd
build.bat
```
Das Skript erstellt eine `Distribution.zip`, die zur Installation verwendet werden kann.
