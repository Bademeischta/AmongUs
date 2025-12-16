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

1.  **Repository klonen.**
2.  **.csproj-Datei anpassen:** Öffne `MyCustomRolesMod.csproj` und passe den `<AmongUsGamePath>` an dein lokales Spielverzeichnis an.
3.  **Projekt kompilieren:** Baue das Projekt in deiner IDE. Dies erstellt die `MyCustomRolesMod.dll`.
4.  **Distribution erstellen:** Führe das `build.sh`-Skript aus. Es nimmt die kompilierte DLL und verpackt sie zusammen mit den notwendigen BepInEx-Dateien in ein distributionsfertiges ZIP-Archiv im Hauptverzeichnis.
