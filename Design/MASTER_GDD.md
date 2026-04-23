# MASTER GAME DESIGN DOCUMENT — GP-OYUN

## 1. Project Identity
- **Title**: GP-OYUN
- **Tagline**: "A town full of people who never say a word — but never stop communicating."
- **Genre**: 2.5D Narrative Social Simulation
- **Core Loop**: Observe → Capture → Publish → Watch Reactions
- **Styling**: Apple/UberEats aesthetic (Invisible Interface, Visible Content)

---

## 2. Technical Pillars
| Pillar | Specification |
|---|---|
| **Architecture** | Event-Driven (Decoupled EventBus) |
| **NPC Brain** | Two-Layer FSM: Emotional (Internal) + Behavioural (Physical) |
| **Movement** | Unity NavMeshAgent (Parallel NPC routines) |
| **Communication** | 100% Non-Verbal (Gestures, Blend Shapes) |
| **Camera** | Fixed Orthographic Isometric (LoL perspective) |

---

## 3. Social Map & NPC Roster
There are **8 core residents** each with a unique OCEAN personality model.
- **Agop (Baker)**: Generous Patriarch, stability node.
- **Fatma (Gossip)**: Information node, carries and spreads emotions.
- **Leila (Artist)**: Sensitive observer, emotional "canary".
- **Hüseyin (Elder)**: Living memory, slow to react but deep impact.
- **Mustafa (Partner)**: Contrarian, emotional mirror to Hüseyin.
- **Selin (Mother)**: Protector, physically reactive (pram collision).
- **Mihail (Vendor)**: Town anchor, passive mood field provider.
- **Ayşe (Arrival)**: Blank slate, personality written by player's choices.

---

## 4. Finite State Machine (FSM) Spec
### 4.1 Emotional States (Internal)
- **Joy / Sadness / Anger / Fear / Surprise / Disgust**
- Decay naturally toward personality baseline.
- Drive facial blend shapes and body posture additives.

### 4.2 Behavioural States (Physical)
- **IDLE**: Default state, look-around animations.
- **WALKING**: Moving via NavMesh to routine targets.
- **READING_NEWS**: Interaction with the Newspaper Board.
- **REACTING**: High-intensity gesture play post-news.
- **CONVERSING**: Social exchange between two NPCs.
- **WORKING**: Stationed at shop (Agop, Mihail).

---

## 5. Gameplay & Survival
- **The Browser/Camera**: Limited shots (3-5), cooldown-based.
- **The Newspaper**: Editorial choices (Categories: Scandal, Celebration, Disaster, Local, Global) drive town mood.
- **Survival**: No publication = Bankruptcy. Exile = Reputation < 0.1.
- **Consequences**: NPCs flee from the camera if player is exploitative.

---

## 6. Implementation Status (MVP)
### [x] Foundation
- EventBus (Pub/Sub)
- NPC Manager (Registry)
- Game Manager (Lifecycle)
- Time Manager (Phases: Morning to Night)

### [x] Player Systems
- WASD Movement (Physics-based)
- Capture Trigger (Mock event system)

### [/] NPC Systems
- **Emotional FSM**: Core logic implemented.
- **Behavioural FSM**: *Planning Phase*. NPCs currently move to targets via simple scripts.
- **Routine Scheduler**: *Partially Implemented*.

### [ ] Advanced Features
- Relationship Sentiment Graph
- Gossip Propagation
- Photo Tag Detection (RenderTexture analysis)

---

## 7. Reference Documents
1. [Game Overview](file:///Users/emre/dev/ai/gp-oyun/Design/01_game_overview.md)
2. [World Objects](file:///Users/emre/dev/ai/gp-oyun/Design/02_world_objects.md)
3. [NPC Profiles](file:///Users/emre/dev/ai/gp-oyun/Design/03_npc_profiles.md)
4. [Newspaper System](file:///Users/emre/dev/ai/gp-oyun/Design/04_newspaper_system.md)
5. [Emotion Engine](file:///Users/emre/dev/ai/gp-oyun/Design/05_emotion_pantomime_engine.md)
6. [Collision & Events](file:///Users/emre/dev/ai/gp-oyun/Design/06_collision_events_listeners.md)
7. [FSM Specifications](file:///Users/emre/dev/ai/gp-oyun/Design/09_fsm_specifications.md)
8. [Project Report](file:///Users/emre/dev/ai/gp-oyun/Design/10_project_report.md)
9. [Asset Setup Guide](file:///Users/emre/dev/ai/gp-oyun/Documentation/Asset_Setup_Guide.md)
