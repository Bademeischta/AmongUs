# Proposed Custom Roles

This document records the conceptual designs for new, novel custom roles for the Among Us mod. These roles are designed with a focus on original mechanics that avoid existing archetypes.

---

## Role 1: The Auditor
**Team:** Crewmate
**Core Concept:**
Truth is not found in what is seen, but in the mathematical reconciliation of energy spent within a closed system.

**Primary Novel Mechanic: Thermodynamic Sensors (Action Units)**
The game map is treated as a series of closed systems (rooms). Every player action—entering a room, completing a task, venting, killing, or sabotaging—generates a fixed, hidden value of "Action Units" (AU). The Auditor quantifies the "weight" of history in a room without seeing the actors.

**Active Abilities:**
*   **Deploy Sensor:** Places an invisible sensor in the current room (Max: 3). Sensors record the *aggregate* AU of all players who enter or perform actions within its radius.
*   **Audit Room:** Remote-accesses a sensor to read the current AU count and then resets it to zero.

**Passive Effects:**
*   **Self-Calibration:** The Auditor is "thermally neutral"—their own movements and actions do not generate AU, making them the only player who can move through a monitored room without corrupting the data.

**Constraints & Failure States:**
The sensor does not distinguish between a single player performing multiple tasks and multiple players moving through. If the Auditor fails to account for a Crewmate's pathing, they may misinterpret a high AU count as a kill (10 AU) when it was actually just three players passing through (3x3 AU + 1).

**Counterplay & Meta Interaction:**
Impostors can "flood" a room with noise by repeatedly entering/exiting to mask a kill's AU signature. Crewmates must coordinate "Quiet Zones" to allow the Auditor to get a clean reading: "Nobody enter Nav for 30 seconds; let the Auditor check for phantom units."

**Uniqueness Proof:**
Unlike the Tracker or Security, the Auditor never receives identity-linked data. It transforms social deduction into a resource-accounting problem where players must reconcile their personal alibis with a mathematical total.

---

## Role 2: The Schismatic
**Team:** Impostor
**Core Concept:**
A master of spatial paradoxes who weaponizes the possibility of where people *could* be to hide where they actually are.

**Primary Novel Mechanic: Reality Fracture (Instance Branching)**
The Schismatic can force the game state to branch into two parallel instances (Timeline Alpha and Timeline Beta) for a short duration. This is not a visual illusion; it is a temporary split of the game's lobby into two distinct, non-interacting "bubbles."

**Active Abilities:**
*   **Fracture:** Splits all players into Timeline Alpha or Timeline Beta randomly. Players in Alpha cannot see or interact with those in Beta. The Schismatic exists in *superposition*, appearing and acting in both timelines simultaneously.
*   **Collapse:** Ends the split. The Schismatic chooses which timeline’s *player positions* become the "True" state. The other timeline's movement data is discarded, but any *state changes* (kills, completed tasks) from both timelines are merged into the final reality.

**Passive Effects:**
*   **Event Leakage:** During a Fracture, a "Ghost Trace" (a flickering, unreportable body) appears in the opposite timeline whenever a kill occurs.

**Constraints & Failure States:**
The Schismatic must be careful which timeline they collapse into. If they collapse into Alpha where they were seen standing over a body, but their alibi was built in Beta, they will be caught immediately.

**Counterplay & Meta Interaction:**
Crewmates must perform a "Timeline Reconciliation" during meetings. "I was with Blue in Alpha-Cafeteria." "But I was with Blue in Beta-Medbay!" If a player was seen in two places at once, the Schismatic (or their partner) is exposed through the violation of the Pauli Exclusion Principle.

**Uniqueness Proof:**
This is not invisibility, teleportation, or a map-swap. It is the simultaneous existence of two conflicting game-states. It forces the Crew to move beyond "Who did I see?" to "In which version of reality was this action possible?"
