# GP-OYUN — Game Design Document
## File 08: Gameplay, Story & Narrative Design

---

## 1. Story Premise

> **You are nobody. You just arrived. And nobody knows your name — because nobody here speaks.**

The player is a **young photojournalist** who has just moved to a small, isolated town square. They carry nothing but a camera and the need to make a living. The town already has its people, its rhythms, its grudges, and its friendships — all built long before the player ever arrived.

The player's job is simple: **take photographs and publish them in the daily newspaper**. That's how they eat. That's how they exist. But every photograph is an editorial decision. Every publication reshapes how the townspeople feel, who they trust, and whether they'll let the player near them again.

There is no dialogue. There are no quest markers. There are no tutorials beyond the first morning. The town breathes on its own — the player simply decides what to amplify.

---

## 2. The Opening — Day 1

### Morning
The player wakes up on the edge of the town square. They are standing near the **Alley** (the quiet entry zone). Nobody notices them. The 8 residents are already mid-routine:

- **Agop** is kneading dough inside the bakery
- **Fatma Hanım** is at the newspaper board — but there's no paper yet (it's the player's first day)
- **Leila** is sketching at the park bench
- **Hüseyin** and **Mustafa** are mid-chess game
- **Selin** is pushing the pram through the square
- **Mihail** is arranging flowers at his stall
- **Ayşe** is also new — she stands at the Post Box, uncertain

The player must **walk into the square** and begin to exist. NPCs notice the player through **proximity triggers** — when the player enters an NPC's detection radius for the first time:

1. The NPC pauses their routine
2. They look at the player (head-turn gesture)
3. Their face shows **Curiosity** (Surprise 0.3 + neutral baseline)
4. After 2 seconds, they return to routine

This is the **Introduction**. The player is unknown. Not threatening, not welcome — just new.

### Midday
The player discovers they can **hold the camera button** and a viewfinder frame appears. They can aim at anything — an object, an NPC, a moment between NPCs — and take a photo. A soft shutter click confirms the capture.

On Day 1, the player has **5 shots** to learn the mechanic.

### Evening
The **Newspaper Office** door glows. Walking to it opens the **Publishing Interface**. The player can drag up to 3 of their photos into the newspaper slots, assign categories (or accept auto-suggestions), and hit PUBLISH.

### Night
A 5-second transition plays. The town goes quiet. Stars appear. The paper is printed.

### Day 2 Morning — The First Reaction
The newspaper board now has content. Every NPC in town walks to the board. One by one, they read. And one by one, they **react** — with body language, with facial expressions, with their entire emotional state reshaped by what the player chose to publish.

**This is the moment the player understands the game.**

---

## 3. Core Gameplay Loop

```
┌─────────────────────────────────────────────────────────────┐
│                    ONE DAY = ONE CYCLE                       │
│                                                             │
│  ┌─────────┐    ┌─────────┐    ┌──────────┐    ┌────────┐  │
│  │ OBSERVE │ →  │ CAPTURE │ →  │ PUBLISH  │ →  │ REACT  │  │
│  │         │    │         │    │          │    │        │  │
│  │ Watch   │    │ Frame & │    │ Choose   │    │ Watch  │  │
│  │ NPCs    │    │ shoot   │    │ what to  │    │ what   │  │
│  │ live    │    │ moments │    │ print    │    │ you    │  │
│  │ their   │    │         │    │          │    │ caused │  │
│  │ lives   │    │         │    │          │    │        │  │
│  └─────────┘    └─────────┘    └──────────┘    └────────┘  │
│       ↑                                             │       │
│       └─────────────────────────────────────────────┘       │
│                   NEXT MORNING                              │
└─────────────────────────────────────────────────────────────┘
```

### What Makes Each Phase Engaging

**OBSERVE**: The town is a stage play running without the player. NPCs interact, argue silently, share quiet moments, have mini-dramas. The player is a voyeur with a camera. The tension: *which moment is worth capturing?*

**CAPTURE**: Limited shots force choice. You can't photograph everything. Some moments are fleeting — Leila crying at the fountain lasts 4 seconds. Miss it, and it's gone. The camera forces attention.

**PUBLISH**: The editorial choice — do you publish Leila crying? It will get a reaction. Everyone will feel something. But Leila may never let you near her again. Or do you publish the pigeons? Safe. Boring. But nobody gets hurt.

**REACT**: The consequence. Morning comes, and every NPC reads the paper. Some laugh. Some cry. Some approach others in new ways they never did before. You reshaped their world.

---

## 4. What Makes This A Game?

### 4.1 Challenge: The Survival Mechanic
The player must **work to survive**. Taking photos and publishing newspapers is not optional — it's a livelihood. If the player publishes nothing for 3 days, the Newspaper Office closes. Game over.

But if the player publishes *recklessly* — exploiting vulnerable moments, publishing scandal after scandal — NPCs develop **distrust**. They flee when they see the camera. They turn their backs. They refuse to be near the player.

**No subjects = no photos = no newspaper = no survival.**

The player must balance **ethical journalism** with **survival needs**.

### 4.2 Challenge: The Social Map
Every NPC has relationships with every other NPC. These relationships are alive:

- Publishing a photo of Agop giving bread to Selin → both gain happiness, their bond strengthens
- Publishing a photo of Mustafa losing at chess → Mustafa becomes irritable, his relationship with Hüseyin strains
- Publishing Leila in a private moment → Leila becomes guarded, avoids public spaces

The player must **read the social map** through body language alone. Who waves to whom? Who sits together at the cafe? Who avoids whose gaze? These are all signals.

### 4.3 Challenge: Emotional Management
The **Town Mood Index** represents the collective emotional health. It drifts based on what happens:

- Too many scandals → Town enters collective anxiety → NPCs stop their routines, cluster nervously
- Too many celebrations → Artificial positivity → NPCs become fragile; one bad event shatters them
- Balanced coverage → Resilient town → NPCs develop natural emotional rhythms

### 4.4 Challenge: Timing & Attention
8 NPCs live simultaneously. The player has ONE camera and a fixed number of shots. Events happen in parallel:

- While you're photographing Agop's bakery, Leila might be having a breakdown at the fountain
- While you're watching Fatma gossip, Ayşe might be receiving a letter at the Post Box that changes her personality
- While you're at the Newspaper Office, Hüseyin might finally lose his composure at the chess table — a once-in-a-week event

**You cannot see everything. You must choose what to witness.**

### 4.5 Win/Lose/Continue States

| State | Condition | Consequence |
|---|---|---|
| **Thriving** | Town Mood > 0.7 for 7 days | Major celebration event — chapter complete |
| **Crisis** | Town Mood < 0.2 for 3 days | NPCs start leaving town — cascading loss |
| **Exile** | Player reputation < 0.1 | All NPCs flee from player — game over |
| **Newspaper Closed** | No publication for 3 days | Loss of livelihood — game over |
| **Harmony** | All relationships > 0.5 | Secret ending — synchronized dance event |

---

## 5. Story Arcs (Chapters)

### Chapter 1: "The Stranger with a Camera" (Days 1–7)
**Theme**: Arrival and first impressions.

- Player learns the mechanics
- NPCs are neutral toward player — curious but cautious
- Ayşe is also new — a parallel newcomer the player can observe
- First story beat: **Agop's bakery has a fire scare** (scripted event, Day 3)
  - How the player photographs this event defines the town's first opinion of them
  - Compassionate photo (Agop rescuing bread) → town warms to player
  - Exploitative photo (Agop panicking) → town becomes wary

### Chapter 2: "The Feud" (Days 8–14)
**Theme**: Conflict and the power of framing.

- Hüseyin and Mustafa's chess rivalry escalates beyond the game
- Fatma's gossip amplifies whatever the player publishes
- Player discovers their editorial power: same event, different category = vastly different reactions
- Story beat: **Hüseyin refuses to play chess** (Day 10)
  - Mustafa wanders the square, lost without his partner
  - Player's coverage determines if they reconcile or if the rift deepens

### Chapter 3: "The Secret" (Days 15–21)
**Theme**: Privacy and the ethics of observation.

- Leila is drawing something she hides from everyone
- Selin's pram triggers more and more alert reactions — what is she protecting?
- Ayşe starts receiving letters that shift her personality dramatically
- Story beat: **Player finds Leila's hidden drawing** (photo opportunity)
  - Publishing it = powerful story, massive reaction — but violates Leila's trust forever
  - Not publishing = Leila eventually shows it herself — weaker news, but stronger bond

### Chapter 4: "The Storm" (Days 22–28)
**Theme**: Crisis and collective resilience.

- A weather event (rain) changes the entire scene aesthetic
- All NPCs seek shelter — routines collapse
- Relationships forged in previous chapters determine who helps whom
- Story beat: **Mihail's flower stall is destroyed by wind**
  - Town's response depends entirely on accumulated mood and relationships
  - If the player built a compassionate town → NPCs help rebuild together
  - If the player exploited them → Mihail is alone

### Chapter 5: "The Town Photo" (Days 29–30)
**Theme**: Legacy and reflection.

- Player is given ONE final photo opportunity
- All NPCs gather in the town square for the first time
- Their arrangement — who stands near whom, who smiles, who turns away — is a living portrait of every choice the player made
- The final newspaper front page = the player's legacy

---

## 6. Player Actions — Complete List

### Movement
| Action | Input | Description |
|---|---|---|
| Walk | WASD | Free movement, physics-based |
| Sprint | Shift + WASD | Faster movement — but NPCs notice running |
| Sit | E near bench | Player sits, time feels slower |

### Camera
| Action | Input | Description |
|---|---|---|
| Aim Camera | Hold Right Mouse | Viewfinder overlay appears |
| Take Photo | Left Click (while aiming) | Shutter snap, photo stored |
| Review Roll | Tab | Open today's photo strip |

### Publishing
| Action | Input | Description |
|---|---|---|
| Enter Office | Walk to Newspaper Office (Evening) | Opens Publishing UI |
| Drag Photo | Mouse drag | Place in Slot 1, 2, or 3 |
| Set Category | Click dropdown | Scandal / Celebration / Disaster / Local / Global |
| Publish | Confirm button | Triggers night transition |

### Social
| Action | Input | Description |
|---|---|---|
| Wave | Q near NPC | Simple greeting gesture |
| Show Photo | F near NPC | Show most recent photo to an NPC (they react) |

---

## 7. NPC Daily Routines — Narrative Flow

Each NPC's routine tells a micro-story every single day. Even without the player's interference, the town has drama:

```
MORNING: The town wakes up
  Agop opens bakery → smell draws NPCs one by one
  Fatma sprints to newspaper board → dramatic reading
  Hüseyin walks slowly to chess → waits for Mustafa
  Everyone passes through the square at least once

MIDDAY: Peak social activity
  Leila and Fatma at cafe → silent gossip
  Selin crosses the square with pram → gets acknowledged
  Mihail calls attention with flower arrangements
  Maximum intersection of routines → best photo opportunities

AFTERNOON: Ripple effects
  Fatma has spread the morning's news to everyone
  Relationships shift — NPCs approach or avoid each other
  Emotional states settle into new patterns
  Quieter moments — confessional gestures at fountain

EVENING: Wind-down
  Hüseyin walks home slowly
  Lamp posts turn on
  The office glows — reminding the player of their duty
  Remaining NPCs have final encounters before night
```

---

## 8. Emergent Narrative — The Butterfly Effect

The game is designed so that **small editorial choices cascade**:

```
Day 3:  Player publishes photo of Fatma whispering to Leila
Day 4:  Leila reads the paper → feels exposed → sadness spike
Day 5:  Leila skips cafe → Fatma notices → guilt gesture → seeks Leila
Day 6:  Fatma finds Leila crying at fountain → comfort attempt
Day 7:  If player photographs the comfort → publishes as "Celebration"
         → Both NPCs heal, town sees reconciliation
         → OR player publishes as "Scandal"
         → Fatma's reputation drops, she becomes a pariah
Day 8:  Without Fatma as the gossip node, news stops spreading
Day 9:  NPCs stop visiting the newspaper board
Day 10: Town Mood stagnates → routines become robotic, lifeless
```

**One photo. One label. Ten days of consequences.**

---

## 9. The Themes

1. **Observation changes everything** — the act of watching and recording is never neutral
2. **Silence speaks louder** — when nobody can use words, every gesture carries weight
3. **Media is power** — and power demands responsibility
4. **Community is fragile** — built on trust, broken by carelessness
5. **Everyone has a story** — but not every story is yours to tell

---

## 10. Why This Game Matters

GP-OYUN is not an action game. It's not a puzzle game. It's a **social simulation where the player is both observer and author**. The game asks a question that real journalism, social media, and human communities face every day:

> **When you hold a camera, what do you choose to show the world?**

The answer is never right or wrong — but it always has consequences.
