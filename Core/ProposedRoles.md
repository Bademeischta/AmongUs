# Proposed Custom Roles for Among Us

This document outlines the conceptual designs for novel custom roles, following the strict "MANDATORY THINKING PHASE" to ensure original mechanics that alter social dynamics and system-level gameplay.

---

## MANDATORY THINKING PHASE

1. **Abstraction of Known Mechanics:**
   - Information control (Witness, Echo, Puppeteer)
   - Movement (Vents, Speed)
   - Kill variation (Geist - delayed kill)
   - Task manipulation (Glitch - reverse progress)
   - Win conditions (Jester)
   - UI/Chat deception (Puppeteer)

2. **Fundamental Patterns:**
   - Delayed consequences, environmental signals, social spoofing, inverted objectives.

3. **Exclusion List:**
   - No delayed kills, no post-mortem info, no chat triggers, no progress bar reversal, no winning by being voted out.

4. **New Domains Explored:**
   - **Physics/Spatio-Temporal Logic:** Physicalizing memory into objects.
   - **Causality & Paradoxes:** Breaking the linear relationship between killer and victim.

---

## Role 1: The Relic
**Team:** Crewmate
**Core Concept:**
Physicalized environmental memory. Instead of information existing in the player's mind or UI, it is stored in a physical "Anchor" that can be manipulated or stolen.

**Primary Novel Mechanic:**
**Spatio-Temporal Anchoring.** The Relic can drop an "Anchor" that passively records the identities of the last three players who pass through its radius. The information is not transmitted to the Relic; it is stored *in the object*.

**Active Abilities:**
- **Place Anchor:** Deploy a physical object at the current location.
- **Harvest Anchor:** Pick up a deployed Anchor to view the stored data (takes 3 seconds of standing still).

**Passive Effects:**
- **Vulnerability:** If the Relic is killed while carrying a Harvested Anchor, the Anchor drops as a lootable item.

**Constraints & Failure States:**
- The Anchor only holds three slots. Newer entries overwrite older ones.
- If an Impostor finds an Anchor, they can "Corrupt" it, causing it to display the wrong names when harvested.

**Counterplay & Meta Interaction:**
- Impostors can actively hunt for Anchors to destroy or corrupt evidence.
- Crewmates can protect the Relic during the Harvest phase.

**Uniqueness Proof:**
Unlike the Tracker or Seer, the information is a *physical resource* in the game world. It can be lost, stolen, or tampered with by the opposing team, creating a "capture the flag" sub-game within the social deduction.

---

## Role 2: The Paradox
**Team:** Impostor
**Core Concept:**
Causal Inversion. The Paradox breaks the immediate link between an action (killing) and its consequence (a body appearing), creating a temporal discrepancy.

**Primary Novel Mechanic:**
**Retroactive Dissolution.** The Paradox can "Undo" a kill they just performed. The victim is brought back to life, but a "Causal Debt" is created.

**Active Abilities:**
- **Kill:** Standard kill.
- **Paradox Snap:** Available for 5 seconds after a kill. Revives the victim and hides the body.
- **Causal Discharge:** Automatically triggered 15 seconds after a "Snap." A random Crewmate (excluding the revived one) dies instantly, regardless of the Paradox's location.

**Passive Effects:**
- **Temporal Distortion:** During the 15-second window between a Snap and a Discharge, the Paradox's name appears slightly flickering/glitched to anyone within a short radius.

**Constraints & Failure States:**
- If a meeting is called during the "Causal Debt" window, the Discharge happens immediately during the meeting, causing a death in the cafeteria (extremely suspicious).

**Counterplay & Meta Interaction:**
- If players notice someone "glitching," they know a Paradox kill was just undone and a random person is about to die.
- The revived victim can testify that they *were* dead, even if they are now alive, but the Paradox can claim it was a "Glitch" role effect.

**Uniqueness Proof:**
This is not a "kill variant" or "teleport." It is a manipulation of the *causal timeline* of the game. It invalidates "I saw him kill" because the victim is suddenly alive, but it guarantees a death later, shifting the focus from "Who did it?" to "When did the debt start?"

---

## Previously Proposed Roles (Legacy)
- **The Auditor (Crewmate):** Social Economics / Trust Insurance.
- **The Schismatic (Impostor):** Information Bifurcation / Split Reality.
