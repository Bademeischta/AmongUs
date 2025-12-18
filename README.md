# MyCustomRolesMod für Among Us (v4.1 - Production Grade)

Dieses Projekt ist ein BepInEx/HarmonyLib-Mod für das Spiel "Among Us", der ein extrem robustes und erweiterbares System für benutzerdefinierte Rollen hinzufügt. Die erste implementierte Rolle ist der **Jester** (Narr).

Diese Version (v4.1+) wurde von Grund auf neu geschrieben, mit einem Fokus auf **maximale Stabilität, Sicherheit und Performance**, um ein produktionsreifes Erlebnis zu gewährleisten.

## Features

- **Erweiterbares Rollen-System:** Die Architektur ist sauber, thread-sicher und für die einfache Integration neuer Rollen ausgelegt.
- **Neue Rolle: Jester:**
    - **Ziel:** Lass dich von den anderen Spielern aus dem Spiel wählen (exiled).
    - **Gewinnbedingung:** Wenn der Jester während eines Meetings rausgewählt wird, gewinnt er sofort das Spiel.
- **Robustes Netzwerkprotokoll:**
    - **ACK & Retry-System:** Garantiert die Zustellung wichtiger Nachrichten auch bei schlechter Netzwerkverbindung.
    - **Late-Join-Support:** Spieler, die einem laufenden Spiel beitreten, werden vollständig synchronisiert.
    - **Versions-Validierung:** Stellt sicher, dass nur Spieler mit der gleichen Mod-Version zusammen spielen können.
- **Benutzerdefinierte Spieleinstellungen:** In der Lobby kann der Host die Wahrscheinlichkeit für einen Jester einstellen. Diese wird sicher an alle Clients synchronisiert.
- **UI-Integration:** Spieler mit der Jester-Rolle sehen ihren Namen in Pink und erhalten eine klare Anzeige ihrer Rolle auf dem HUD.

## Installation (für Spieler)

**Wichtiger Hinweis:** Alle Spieler in der Lobby müssen exakt die gleiche Version des Mods installiert haben.

1.  **ZIP-Archiv herunterladen:** Lade die `MyCustomRolesMod-vX.X-Distribution.zip` von der [Release-Seite](https://github.com/example/mycustomrolesmod/releases) des Projekts herunter.
2.  **In das Spielverzeichnis entpacken:** Entpacke den Inhalt der ZIP-Datei direkt in dein Among Us-Hauptverzeichnis (der Ordner, der die `Among Us.exe` enthält).
3.  **Spiel starten:** Das war's! Starte das Spiel. Der Mod ist jetzt vollständig installiert.

## Konfiguration

Nach dem ersten Start des Spiels mit dem Mod wird eine Konfigurationsdatei erstellt unter:
`Among Us/BepInEx/config/com.example.mycustomrolesmod.paranoid.cfg`

In dieser Datei können fortgeschrittene Einstellungen wie Netzwerk-Timeouts und Debug-Logging angepasst werden.

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
Wenn die `AMONG_US_GAME_PATH` Variable nicht gesetzt ist, versucht das Skript intelligent zu sein: Es sucht automatisch in den `bepinex_files`-Verzeichnissen nach den benötigten Spieldateien. In vielen Fällen findet es sie dort und kopiert sie für den Build-Prozess an die richtige Stelle. Sie müssen also eventuell nichts weiter tun.

**Wichtiger Hinweis:** Die BepInEx-Distribution enthält nicht immer alle spiel-spezifischen Dateien (wie `Assembly-CSharp.dll`). Wenn das Skript diese Dateien nicht finden kann, müssen Sie Option A verwenden.

**Zusätzliche Abhängigkeit: BepInEx**
Das Skript benötigt außerdem die BepInEx-Dateien, um das finale ZIP-Paket zu erstellen.
1.  Laden Sie `BepInEx 6.0.0-pre.1 IL2CPP` von der [offiziellen BepInEx-Release-Seite](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1) herunter.
2.  Entpacken Sie den Inhalt in einen Ordner namens `bepinex_files` im Hauptverzeichnis dieses Projekts.

### Schritt 2: Build ausführen

Nachdem alle Abhängigkeiten erfüllt sind, führen Sie einfach das Build-Skript aus, um eine vollständige `Distribution.zip`-Datei zu erstellen:
```cmd
build.bat
```
Das Skript wird Sie am Ende mit "pause" anhalten, damit Sie die Ausgabe in Ruhe lesen können.
