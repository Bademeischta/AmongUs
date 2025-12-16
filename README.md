# MyCustomRolesMod für Among Us

Dieses Projekt ist ein BepInEx/HarmonyLib-Mod für das Spiel "Among Us", der ein erweiterbares System für benutzerdefinierte Rollen hinzufügt. Die erste implementierte Rolle ist der **Jester** (Narr).

## Features

- **Erweiterbares Rollen-System:** Die Architektur (`RoleManager`, `BaseRole`) ermöglicht es Entwicklern, einfach neue Rollen hinzuzufügen.
- **Neue Rolle: Jester:**
    - **Ziel:** Lass dich von den anderen Spielern aus dem Spiel wählen (exiled).
    - **Gewinnbedingung:** Wenn der Jester während eines Meetings rausgewählt wird, gewinnt er sofort das Spiel.
- **Benutzerdefinierte Spieleinstellungen:** In der Lobby kann der Host die Wahrscheinlichkeit (0-100%) einstellen, mit der ein Jester im Spiel erscheint. Diese Einstellung wird mit allen Spielern synchronisiert.
- **Netzwerk-Synchronisation:** Rollen und Einstellungen werden zuverlässig über benutzerdefinierte RPC-Nachrichten an alle Clients gesendet.
- **UI-Integration:** Spieler mit der Jester-Rolle sehen ihren Namen in Pink und erhalten eine klare Anzeige ihrer Rolle auf dem HUD.

## Installation (für Spieler)

**Wichtiger Hinweis:** Alle Spieler in der Lobby müssen den Mod installiert haben, damit er korrekt funktioniert.

1.  **BepInEx installieren:** Lade die neueste Version von [BepInEx 6 (IL2CPP)](https://github.com/BepInEx/BepInEx/releases) herunter. Entpacke den Inhalt in dein Among Us-Spieleverzeichnis (dort, wo sich die `Among Us.exe` befindet).
2.  **Mod-DLL herunterladen:** Lade die `MyCustomRolesMod.dll` aus dem [Releases-Bereich](https.github.com.example.mycustomrolesmod/releases) dieses Projekts herunter.
3.  **Mod platzieren:** Lege die `MyCustomRolesMod.dll` in den folgenden Ordner in deinem Spielverzeichnis: `Among Us/BepInEx/plugins`.
4.  **Spiel starten:** Starte das Spiel. Wenn alles korrekt installiert ist, sollte der Mod automatisch geladen werden.

## Build-Anleitung (für Entwickler)

1.  **Repository klonen:** Klone dieses Repository auf deinen lokalen Rechner.
2.  **.csproj-Datei anpassen:** Öffne die `MyCustomRolesMod.csproj`-Datei in einem Texteditor.
3.  **Spielpfad festlegen:** Finde die Zeile `<AmongUsGamePath>...</AmongUsGamePath>` und ersetze den Platzhalterpfad mit dem Pfad zu deinem lokalen Among Us-Verzeichnis.
4.  **Abhängigkeiten wiederherstellen:** Öffne das Projekt in einer IDE wie Visual Studio oder JetBrains Rider und stelle die NuGet-Pakete wieder her.
5.  **Projekt erstellen:** Kompiliere das Projekt. Die fertige `MyCustomRolesMod.dll` wird im `bin/Debug` oder `bin/Release`-Ordner deines Projektverzeichnisses erstellt.
6.  **DLL kopieren:** Kopiere die kompilierte DLL-Datei in den `BepInEx/plugins`-Ordner deiner Among Us-Installation, um sie zu testen.

---
*Dieser Mod wurde zu Demonstrations- und Lernzwecken erstellt.*
