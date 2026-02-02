# Changelog

Alle wichtigen Änderungen an diesem Projekt werden in dieser Datei dokumentiert. Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/).

## [4.1.0] - 2024-08-02

### Hinzugefügt (Added)
- **Neue Rolle: Dendrochronologist (Crewmate):** Implementierung eines Raum-Tracking-Systems, das die letzten 4 Besucher pro Raum speichert. Inklusive "Extract Core" und "Sample Age" Fähigkeiten.
- **Neue Rolle: Solipsist (Impostor):** Implementierung eines Wahrnehmungs-Manipulationssystems. Ermöglicht das "Zensieren" von Leichen für spezifische Crewmates, wodurch diese für das Opfer unsichtbar und nicht meldbar werden.
- **Erweitertes RPC-System:** Neue Netzwerk-Pakete zur Synchronisation von zensierten Objekten und Raum-Historien.
- **Zentralisierte Raum-Erkennung:** Ein Proxy-System zur Identifizierung von Räumen basierend auf der Nähe zu Schiffssystemen.

## [4.0.0] - 2024-08-01

### Behoben (Fixed)
- **Kritisches Speicherleck behoben:** Das `MessageWriter`- und `MessageReader`-Recycling wurde im gesamten Netzwerk-Stack implementiert, um Speicherlecks zu verhindern. Dies beinhaltet eine `OnDestroy`-Methode im `RpcManager` und `try-finally`-Blöcke im `RpcPatch`.
- **Performance-Problem im `Update`-Loop behoben:** Die `ToList()`-Allokation in der `Update`-Schleife des `RpcManager` wurde entfernt, um unnötigen Garbage Collection Druck zu vermeiden.
- **RPC-Patch-Sicherheit:** Der `RpcPatch` wurde von einem gefährlichen `Prefix`- zu einem sicheren `Postfix`-Patch umgestaltet, der das Blockieren von Spiel-Nachrichten bei einem Mod-internen Fehler verhindert.

### Geändert (Changed)
- **Architektur-Härtung:** Die Datenintegrität wurde durch eine robustere CRC32-Prüfsumme, `try-catch`-Blöcke für den Late-Join-Check und vollständige Enum-Validierung in allen RPC-Handlern verbessert.
- **Code-Konsistenz:** Die gesamte Code-Basis wurde auf konsistente Namenskonventionen und Best Practices vereinheitlicht.

## [3.0.0] - 2024-08-01

### Hinzugefügt (Added)
- **Vollständige Thread-Sicherheit:** Der `RoleManager` wurde neu geschrieben, um ein `lock`-basiertes System zu verwenden, das atomare Operationen im gesamten State Management garantiert.
- **Robustes Netzwerkprotokoll:** Ein `RpcManager` wurde implementiert, der ein **Acknowledgment (ACK) und Retry-System mit exponentiellem Backoff** für zuverlässige Nachrichtenübermittlung bietet.
- **Late-Join-Support:** Spieler, die einem laufenden Spiel beitreten, werden nun korrekt mit dem aktuellen Rollen-Status synchronisiert.
- **Protokoll-Versions-Aushandlung:** Ein Handshake-System wurde hinzugefügt, das sicherstellt, dass nur Clients mit einer kompatiblen Mod-Version beitreten können.
- **Sichere Custom Options:** Die Synchronisation der Spieleinstellungen erfolgt nun über ein eigenes, versioniertes Paket mit Magic Bytes und Checksumme.

### Geändert (Changed)
- **Behebung der fundamentalen Race Condition:** Der Zustand wird nun immer lokal gesetzt, *bevor* die entsprechende Netzwerk-Nachricht gesendet wird.

## [2.0.0] - 2024-08-01

### Behoben (Fixed)
- **Kritischer Host-Check-Bug behoben:** Die falsche `AmOwner`-Prüfung wurde durch die korrekte `AmongUsClient.Instance.AmHost`-Prüfung ersetzt.
- **Speicherleck im UI behoben:** Das UI-Lifecycle-Management wurde implementiert, indem UI-Elemente in `HudManager.OnDestroy` korrekt zerstört werden.
- **Grundlegendes RPC-Error-Handling:** Der RPC-Patch wurde mit einem `try-catch`-Block versehen, der die `MessageReader`-Position bei einem Fehler zurücksetzt, um Desynchronisationen zu verhindern.

### Geändert (Changed)
- **Architektur-Verbesserung:** Der `RoleManager` wurde als Singleton implementiert und nutzt nun ein `Dictionary` für performante O(1)-Abfragen. Die Code-Struktur wurde in `Core`, `Networking` und `Patches` aufgeteilt.
- **Zentrales Config-System:** Eine `ModConfig`-Klasse wurde hinzugefügt, um Einstellungen über die BepInEx-API zu verwalten.
- **Umfassendes Logging:** An allen kritischen Stellen wurde detailliertes Logging hinzugefügt.

## [1.0.0] - 2024-08-01

### Hinzugefügt (Added)
- **Initiales Release des Jester Role Mods.**
- Grundlegende Implementierung der Jester-Rolle, der Gewinnbedingung und der UI-Anzeigen.
- Erstes Netzwerk-System zur Rollen-Synchronisation.
- Hinzufügen einer Custom Game Option für die Jester-Wahrscheinlichkeit.
