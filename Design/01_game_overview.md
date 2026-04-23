# 🎭 GP-OYUN — Game Design Document
## File 01: Game Overview & Vision

---

## 1. Concept Statement

> **"A town full of people who never say a word — but never stop communicating."**

GP-OYUN is a 2.5D single-screen narrative simulation. The player inhabits a small, dense neighbourhood where every resident has a personality, a mood, and a daily rhythm. Nobody speaks. Nobody types. The entire social fabric of this world is woven through **body language, facial expressions, gestures, and reactions** — pure pantomime.

The player's most powerful tool is not a weapon or a spell — it is a **camera**. Photos you capture become news stories published in the town newspaper. Every resident reads the paper. And every resident reacts differently — based on who they are, what they value, and how fragile their emotional world is.

---

## 2. Genre & Pillars

| Pillar | Description |
|---|---|
| **Genre** | 2.5D Narrative Simulation / Social Sandbox |
| **Camera** | Fixed orthographic top-down/isometric (League of Legends perspective) |
| **Communication** | 100% non-verbal — facial expressions, gestures, posture |
| **Core Loop** | Observe → Capture → Publish → Watch Reactions |
| **Tone** | Warm, slightly absurd, deeply human |

---

## 3. Perspective & Rendering

```
Camera: Orthographic, isometric 45° tilt
Render:  2.5D — 3D characters and props on layered 2D planes
View:    Everything visible on ONE screen (no scrolling world)
Scale:   ~30×20 grid units visible at all times
```

The world is **always fully visible**. There is no fog of war, no scrolling, no loading screens. Everything that is about to happen, is happening, or just happened — is on screen simultaneously. This creates the feeling of watching a living stage play.

---

## 4. Core Mechanics Summary

### 4.1 Player (The Journalist)
- **A newcomer** — just arrived in town, knows nobody, must earn trust
- Moves freely through the town (WASD, physics-based)
- Carries a camera with a limited shot count per day (3–5 shots)
- Can capture moments: expressions, interactions, accidents, signs
- Submits photos to the Newspaper Office by end of day
- Cannot speak, shout, or type — they are also a silent character
- **Must work to survive** — publishing newspapers is their livelihood
  - No publication for 3 days → Newspaper Office closes → Game Over
  - Reckless publishing → NPCs distrust player → they flee from camera → no subjects → no work
- Players can also photograph objects and environment (other NPCs may appreciate the artistry)

### 4.2 NPCs (The Residents)
- 8–12 fully animated characters live in town
- Each has a **daily routine** (walk to bakery, sit in park, feed birds)
- Each has a **personality profile** (traits, sensitivities, memory)
- NPCs **interact with each other** through gesture and expression
- NPCs **react to the newspaper** with emotion the next "morning"

### 4.3 The Newspaper
- Player selects 1–3 photos per day to publish
- Each photo carries a category tag: Scandal, Celebration, Disaster, Local, Global
- Published images appear on the newspaper board visible in town
- All NPCs walk to the board, read, and react — publicly, emotionally

### 4.4 Day Cycle
```
Morning  → NPCs begin routines, yesterday's paper is on the board
Midday   → Peak social activity, best photo opportunities  
Afternoon→ Emotional ripples from the paper spread through NPC behaviour
Evening  → Player submits photos at Newspaper Office
Night    → "Develop" phase: reactions are processed, stored in memory
```

---

## 5. Win Condition / Progression

There is no traditional win state. Progression is measured by:

- **Town Mood Index** — collective emotional health of all NPCs
- **Story Beats** — scripted events triggered by specific emotional thresholds
- **Memory Depth** — how many past events NPCs remember and reference in behaviour
- **Photo Archive** — a growing gallery of the player's captured moments
- **Player Reputation** — how NPCs perceive the journalist (Unknown → Trusted → Exiled)

The player "finishes" a chapter when a major town event resolves (a feud, a celebration, a mystery) — driven entirely by the chain reactions they initiated with their photos.

### Failure States
| State | Condition | Result |
|---|---|---|
| **Exile** | Reputation drops below 0.1 | All NPCs flee from player — game over |
| **Newspaper Closed** | No publication for 3 consecutive days | Loss of livelihood — game over |
| **Town Collapse** | Town Mood < 0.2 for 3 days | NPCs start leaving — cascading failure |

---

## 5.5 Social Map

A living relationship graph connects every NPC to every other NPC:

- **28 unique NPC-NPC pairs** with sentiment values (-1.0 to 1.0)
- Relationships change based on shared events, newspaper reactions, and gossip
- The player can **read** relationships through body language: who sits together, who avoids whom, who waves and who turns away
- The social map is never shown explicitly — the player must observe and infer
- Ayşe (NPC-08) starts with blank relationships — her social map is written by gameplay events

---

## 6. Art Direction Reference

| Element | Style |
|---|---|
| Characters | Stylised 3D — **body movements are the priority**, faces are secondary at isometric distance |
| Body Language | Exaggerated gestures, emoji-spreading style, kimono/clothing physics |
| Face | Simple blend shapes — readable at distance, NOT micro-detailed (too much work, too small to see) |
| Good Rigging | **Critical** — weight painting, joint deformation, clothing movement must be natural |
| Environment | Warm-toned European village square, LoL-style camera — buildings SHORT, characters ALWAYS visible |
| Map Design | Trees may partially block view, but ALL architecture stays below camera sightline |
| Lighting | Soft baked light with dynamic day-time shadows |
| Animations | Weight-shifted, Miyazaki-influenced movement — deliberate, expressive |
| Colour Palette | Muted pastels with emotional accent colours (red for anger, gold for joy) |

---

## 7. Unique Selling Points

1. **Zero dialogue** — emotions are 100% visual, universally readable
2. **Butterfly effect** — one photo can reshape every NPC's week
3. **The newspaper as both tool and mirror** — you shape the town's reality
4. **Parallel NPC lives** — every character exists and reacts independently, simultaneously
5. **Memory system** — NPCs age emotionally; an NPC insulted twice becomes permanently guarded
