# Proposed Roles for MyCustomRolesMod

This document contains conceptual designs for novel Among Us roles that follow the "MANDATORY THINKING PHASE" and originality constraints.

---

## Role Name: The Tecton
**Team:** Crewmate
**Core Concept:**
Social interactions generate invisible structural "stress" that eventually fractures. The Tecton monitors the stability of group clusters rather than individual movements.

**Primary Novel Mechanic:**
**Social Stress Mapping:** The Tecton sees thin, translucent "fault lines" connecting players who remain in proximity to one another. The color of these lines shifts from white to deep crimson as the "stress" (time spent in proximity without a game-state change) increases. When a kill or a major sabotage occurs, the fault lines "snap" and vanish, but leave a temporary "Epicenter" (a lingering visual distortion) at the point where the social stress was most concentrated.

**Active Abilities:**
- **Release Tension:** The Tecton can target a "fault line" to dissipate its stress, gaining a short burst of speed but losing the ability to see that cluster's epicenter if a kill occurs there.

**Passive Effects:**
- **Subduction Awareness:** When a body is reported, the Tecton sees the "Stress Magnitude" of the round—a numerical value representing how much collective time players spent in high-stress clusters. Low magnitude suggests a roaming/isolated killer; high magnitude suggests a "stack-killer" or someone hiding in groups.

**Constraints & Failure States:**
If players are constantly moving and never clustering, the Tecton gains no information. If the Tecton "Releases Tension" on the wrong cluster, they might accidentally erase the evidence of an Impostor's "stack-kill."

**Counterplay & Meta Interaction:**
Impostors can counter the Tecton by constantly rotating partners and avoiding long-term "stacking," or by "overloading" a cluster with so many players that the Tecton cannot distinguish which specific pair is generating the most stress.

**Uniqueness Proof:**
Unlike a Tracker (who follows individuals) or a Seer (who sees roles), the Tecton monitors a meta-property of the group (clustering density/duration). It turns "grouping up for safety" into a readable data point that can backfire if the group is compromised.

---

## Role Name: The Noumenon
**Team:** Impostor
**Core Concept:**
The separation of the "phenomenon" (the observed event) from the "noumenon" (the event itself). The Impostor manipulates the "where" and "when" of player perception.

**Primary Novel Mechanic:**
**Perceptual Decoherence:** The Noumenon can "split" a target's presence. They create a "Phenomenal Echo" of a player—a visual clone that mimics the player's movements with a 5-second delay. This echo is visible to everyone. The "Real" player (the Noumenon or a victim) becomes invisible to everyone except the Noumenon while the echo exists.

**Active Abilities:**
- **Collapse State:** The Noumenon terminates the Echo. The "Real" player (who was invisible) instantly snaps to the Echo's current position. If the Noumenon uses this on a victim and then kills them, the body appears at the Echo's location, regardless of where the victim actually was.

**Passive Effects:**
- **Observer Effect:** When the Noumenon is being watched by a camera or a player in line-of-sight, their Echo duration is halved, but the "Collapse" range is doubled.

**Constraints & Failure States:**
The Echo does not interact with the environment (it doesn't open doors or stand on task spots). Sharp-eyed players will notice a "player" walking through a closed door or failing to trigger a pressure plate, revealing the Noumenon's influence. If the Noumenon miscalculates the "Collapse," they might accidentally teleport themselves or a body into a room full of witnesses.

**Counterplay & Meta Interaction:**
Crewmates must verify "task-syncing"—checking if a player's movements match the logic of the map. If someone is "lagging" exactly 5 seconds behind their expected path, they are an Echo.

**Uniqueness Proof:**
Unlike the Morphling (who changes appearance) or the Phantom (who is invisible), the Noumenon creates a temporal-spatial displacement. It doesn't just hide the truth; it provides a false, delayed truth that can be "collapsed" into reality at a chosen moment.

---
*Note: All previously generated roles (The Mnemonist, The Sophist, The Relic, The Paradox, The Auditor, The Schismatic, The Dendrochronologist, The Solipsist) are considered part of the established meta and are excluded from future generation.*
