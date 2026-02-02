# Proposed Custom Roles

## Role Name: The Dendrochronologist
**Team:** Crewmate
**Core Concept:**
The environment is a living record of movement. By examining the "growth rings" of a room, the player can deduce the chronological order of occupants without ever seeing them.

**Primary Novel Mechanic:**
**"Laminar Room History."** Every room in the game maintains a "Core" (an invisible data point). This Core records the color of the last four players who entered the room, stored as concentric "Residue Rings" on the floor that only the Dendrochronologist can see.

**Active Abilities:**
*   **"Core Extraction":** When inside a room, the Dendrochronologist can "deepen" the rings, making them visible to all Crewmates for 10 seconds.
*   **"Sample Age":** A UI prompt that tells the Dendrochronologist exactly how many seconds ago the "Outer Ring" (the most recent visitor) was formed.

**Passive Effects:**
*   **"Environmental Memory":** The Dendrochronologist sees colored circles on the floor of every room. The outermost circle is the most recent visitor; the innermost is the oldest of the last four. If a player is currently in the room, their ring pulses.

**Constraints & Failure States:**
The rings do not show *how long* a player stayed or *where* they went inside the room—only the order of entry. If multiple players enter simultaneously (e.g., a stack), the rings "blur" (turn grey), providing no data and punishing the Crew for poor spatial discipline.

**Counterplay & Meta Interaction:**
Impostors can "pollute" the rings by entering and exiting rooms repeatedly to push a "incriminating" ring (the killer's) into the inner, older layers, effectively "burying" the evidence under new data.

**Uniqueness Proof:**
Unlike a *Tracker* (who follows a player) or a *Witness* (who hears events), the Dendrochronologist audits the *room*. It shifts the discussion from "Who did you see?" to "In what order did we enter Electrical?" and exposes "Phantom Entries" (vents) when a ring appears without a door-entry event.

---

## Role Name: The Solipsist
**Team:** Impostor
**Core Concept:**
Truth is not universal; it is granted. The Solipsist manipulates the "perceptual reality" of victims, making them unable to recognize specific physical truths of the game world.

**Primary Novel Mechanic:**
**"Selective Cognitive Erasure."** The Solipsist does not hide themselves; they hide *consequences*. They can choose a "Reality Anchor" (a dead body, a vent, or a sabotage station) and "Censure" it for a specific player. To that player, the object literally does not exist on their screen.

**Active Abilities:**
*   **"Censure Reality":** Target a player and a nearby object (e.g., a Body). For the next 30 seconds, that player cannot see the body, cannot report it, and has no collision with it.
*   **"Sensory Void":** The Solipsist can "erase" the sound and visual alert of a Sabotage for a target player. The player might stand in a room with a collapsing Reactor and see/hear absolutely nothing wrong.

**Passive Effects:**
*   **"Weightless Crime":** If the Solipsist kills a player, the body is automatically "Censured" for the nearest Crewmate for 15 seconds.

**Constraints & Failure States:**
If a player who *can* see the body reports it while the "Censured" player is standing on top of it, the Censured player is immediately identified as a "Perceptual Outlier," often leading to intense suspicion or a "Logic Trap" where they cannot explain why they didn't see a body under their feet.

**Counterplay & Meta Interaction:**
Crewmates must use "Double-Verification." If Player A says "There is no body in Nav," and Player B says "I am looking at a body in Nav," the Crew knows a Solipsist is active. This forces the Crew to move in pairs where one player acts as the "Reality Check" for the other.

**Uniqueness Proof:**
This is not *Invisibility* or *Shapeshifting*. The body is there for everyone else. It introduces "Subjective Testimony" to Among Us. It creates a scenario where a player can be "innocently blind," breaking the fundamental social deduction assumption that "If you were there and didn't report, you are the Impostor."
