# GP-OYUN — Game Design Document
## File 03: NPC Profiles, Personalities & Routines

---

> All NPCs exist **in parallel**. Every NPC is always doing *something*, regardless of whether the player is watching. Their routines, emotions, and interactions run simultaneously — the player is an observer first, a participant second.

---

## 1. NPC Roster (8 Core Characters)

---

### NPC-01: THE BAKER — "Agop"
```
Age:        58
Build:      Round, strong arms, flour-dusted apron
Archetype:  The Generous Patriarch
```

**Personality (OCEAN)**
| Trait | Score | Meaning |
|---|---|---|
| Openness | 0.4 | Resistant to change |
| Conscientiousness | 0.9 | Never late, never lazy |
| Extraversion | 0.7 | Warm, enjoys greeting people |
| Agreeableness | 0.85 | Generous, conflict-averse |
| Neuroticism | 0.2 | Emotionally stable, slow to anger |

**Daily Routine**
- `06:00` Opens bakery, kneads dough gesture at counter
- `07:00–12:00` Serves anyone who passes — waves, smile, free sample gestures
- `12:00` Takes a bench break, reads newspaper board (if news)
- `14:00` Returns to bakery for afternoon prep
- `18:00` Closes shutters, waves goodbye to square

**News Reactions**
| Category | Emotion | Behaviour |
|---|---|---|
| Celebration | Happy (0.9) | Does a little dance behind counter |
| Scandal | Worried (0.6) | Stops serving, stares at door |
| Disaster | Fearful (0.8) | Hands out bread to everyone nearby |
| Local | Curious (0.5) | Leans over counter to look at board |
| Global | Neutral | Shrugs, returns to work |

**Special Interactions**
- If another NPC is crying near the bakery → Agop brings them something (gesture)
- If angry NPC passes → Agop makes calming gesture, occasionally diffuses conflict
- Memory: If the same NPC gets bad news 3+ days in a row, Agop starts waiting for them at the door each morning

---

### NPC-02: THE GOSSIP — "Fatma Hanım"
```
Age:        64
Build:      Slight, quick-moving, always holding something (bag, flowers, papers)
Archetype:  The Information Node
```

**Personality (OCEAN)**
| Trait | Score |
|---|---|
| Openness | 0.8 |
| Conscientiousness | 0.3 |
| Extraversion | 1.0 |
| Agreeableness | 0.4 |
| Neuroticism | 0.75 |

**Daily Routine**
- `07:30` First at the newspaper board every morning — reads with dramatic gestures
- `08:00–12:00` Moves between every NPC on the map, performs news recap gestures
- `12:00` Cafe table with Leila for "lunch gossip"
- `15:00` Flower stall small talk with Vendor
- `17:00` Benchsitting — observes the square, reactive idle animations

**News Reactions**
| Category | Emotion | Behaviour |
|---|---|---|
| Scandal | Shocked → Excited (1.0) | Immediately runs to closest NPC to re-enact |
| Celebration | Happy (0.7) | Claps, skips slightly |
| Disaster | Fearful → Gossip Mode | Runs a circuit touching every NPC |
| Local | Curious (1.0) | Inspects photo at board up-close, tilts head |

**Special Mechanic: Gossip Propagation**
- After reading news, Fatma "carries" the emotion category for 30 minutes
- Any NPC she touches shoulder during this window receives a reduced version of that emotion
- This is the town's word-of-mouth system — emotion without language

---

### NPC-03: THE YOUNG ARTIST — "Leila"
```
Age:        24
Build:      Tall, loose clothing, always sketchbook under arm
Archetype:  The Sensitive Observer
```

**Personality (OCEAN)**
| Trait | Score |
|---|---|
| Openness | 0.98 |
| Conscientiousness | 0.4 |
| Extraversion | 0.35 |
| Agreeableness | 0.7 |
| Neuroticism | 0.85 |

**Daily Routine**
- `09:00` Arrives at park bench with sketchbook
- `09:00–13:00` Idle drawing animation — reacts visually to any scene near her
- `12:00` Cafe with Fatma
- `14:00–17:00` Wanders between fountain and old tree
- Near fountain → pulls out sketchbook and draws the water

**News Reactions**
| Category | Emotion | Behaviour |
|---|---|---|
| Disaster | Distressed (0.95) | Presses sketchbook to chest, sits on ground |
| Celebration | Joyful (0.85) | Starts gesture-sketching in the air |
| Scandal | Disgusted (0.7) | Raises hand, walks away from board |
| Local | Curious (0.9) | Studies photo intensely, head tilts |

**Special: Expressive Mirror**
- Leila's face is the most expressive in the cast — she is the emotional "canary"
- When her mood changes, it is the most readable, most dramatic
- Good to use as a photography subject to capture the town's emotional state

---

### NPC-04: THE ELDER — "Hüseyin Bey"
```
Age:        75
Build:      Lean, dignified, cane, flat cap
Archetype:  The Living Memory
```

**Personality (OCEAN)**
| Trait | Score |
|---|---|
| Openness | 0.3 |
| Conscientiousness | 0.95 |
| Extraversion | 0.5 |
| Agreeableness | 0.6 |
| Neuroticism | 0.15 |

**Daily Routine**
- `08:00` Slow walk from left edge to chess table
- `08:30–12:00` Chess game with Mustafa (see below)
- `12:00` Paper read — slow, deliberate, long pause at each photo
- `14:00` Bench sit in sun, feeds pigeons
- `16:30` Slow walk home (exit right edge)

**News Reactions**
- Hüseyin reacts with the smallest visible range — but his reactions last twice as long
- A slight nod = Approval | Stillness = Concern | One slow head shake = Serious Disapproval
- Memory: He accumulates mood across 7 days — slow to fully anger, devastating when he does

---

### NPC-05: THE CHESS PARTNER — "Mustafa"
```
Age:        72
Build:      Stocky, grey beard, always in the same sweater
Archetype:  The Contrarian
```

**Personality**
- Always opposes whatever Hüseyin reacts to
- If Hüseyin nods at news → Mustafa shakes head
- If Hüseyin is calm → Mustafa is animated
- They have been friends for 50 years; their conflict IS their friendship

**Special Mechanic: The Opposing Pair**
- Both NPCs share a **RelationshipChannel**: each other's emotion modifies the other's in the *opposite direction*
- If both end up in the same emotional state → rare "synchronised" event plays → beautiful photo moment

---

### NPC-06: THE YOUNG MOTHER — "Selin"
```
Age:        31
Build:      Pushes a pram, always slightly tired, warm smile
Archetype:  The Protector
```

**Special Mechanic**
- The pram is its own physics object with a collision trigger
- If a pushed prop hits the pram → Selin enters HighAlert state
- Selin is the most physically reactive NPC — her body language space is the widest

---

### NPC-07: THE FLOWER VENDOR — "Mihail"
```
Age:        45
Build:      Mediterranean, loud gestures even in silence
Archetype:  The Town Anchor
```

- Stationed at flower stall all day
- His gestures function as a **reaction amplifier** — if he's happy, his arms make it obvious
- NPCs passing his stall get a small mood modifier (proximity aura: +0.1 happiness if he's happy)

---

### NPC-08: THE NEW ARRIVAL — "Ayşe"
```
Age:        27
Build:      Cautious posture, keeps distance, slowly warms to square
Archetype:  The Unknown Quantity
```

- Starts the game with blank personality (all traits 0.5)
- Her profile shifts slightly each day based on the emotional events she witnesses
- She is the "evolving NPC" — her personality is written by the game's events, not by the designer
- Most unpredictable — great photo subject

---

## 2. NPC Parallel Execution Model

```
Each NPC runs on its own coroutine stack:

  [NPC Coroutine]
    ├── RoutineScheduler   — what to do and when
    ├── EmotionStateMachine — current face/body state
    ├── ProximityDetector   — who is nearby
    ├── NewsMemory          — accumulated reactions
    └── RelationshipMap     — per-NPC sentiment value
```

All coroutines run simultaneously. The player's presence does **not** pause or alter NPC coroutines — they run regardless. The player is a silent observer embedded in a living world.
