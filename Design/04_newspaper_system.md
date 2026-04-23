# GP-OYUN — Game Design Document
## File 04: The Newspaper System

---

## 1. Philosophy

> The newspaper is not just a mechanic. It is the town's **collective unconscious**.  
> What gets published shapes reality. What gets left out shapes secrets.

The player is a journalist with no words — only images. The single editorial decision of *which photos to publish* causes ripple effects that no NPC can anticipate, including the player.

---

## 2. Photo Capture System

### 2.1 The Camera
```
Resource:    3–5 shots per day (upgradeable)
Cooldown:    4 seconds between shots
Framing:     UI frame appears when player holds Capture button
Aim:         Any direction within player's radius
Result:      A RenderTexture snapshot stored in Session Memory
```

### 2.2 What Makes a Good Photo?

The photo-scoring system runs **silently** — the player never sees a score breakdown. But the category and weight of a photo is computed and stored.

| Condition | Tag Added | Effect on NPCs |
|---|---|---|
| Face of an NPC visible | `FaceCapture` | Emotional reaction intensity +20% |
| Two NPCs interacting | `InteractionCapture` | Affects both NPCs' reactions |
| An NPC in peak emotion | `PeakEmotion` | Category inherits that emotion type |
| A displaced prop visible | `Chaos` | Adds `Scandal` weight |
| Weather event in frame | `Atmospheric` | Ambient mood shift for all |
| Player in mirror/water | `META` | Hidden story beat trigger |

### 2.3 Photo Categories (Auto-detected, Player can Override)
```
Auto-tag logic runs on RenderTexture analysis (Unity custom Shader Pass):

  → If FaceCapture + PeakEmotion(Angry) → suggested: "Scandal"
  → If InteractionCapture + Happy ≥ 0.7 → suggested: "Celebration"
  → If Chaos + FaceCapture → suggested: "Disaster"
  → If no peak emotion → suggested: "Local"
  → If player near clock tower + weather → suggested: "Global"

Player can accept or override the suggested category before publishing.
```

---

## 3. The Publishing Interface

Triggered when player enters the **Newspaper Office** zone at evening.

```
┌───────────────────────────────────────────────┐
│           TOMORROW'S EDITION                  │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐    │
│  │  SLOT 1  │  │  SLOT 2  │  │  SLOT 3  │    │
│  │  (empty) │  │  (empty) │  │  (empty) │    │
│  └──────────┘  └──────────┘  └──────────┘    │
│                                               │
│  YOUR ROLL (today's captures):                │
│  [Photo1] [Photo2] [Photo3] [Photo4]          │
│                                               │
│  Category: [SCANDAL ▾]    [ PUBLISH ]         │
└───────────────────────────────────────────────┘
```

- Drag a photo to a slot
- Assign/confirm category
- Slot 1 = Front page (highest reach: all NPCs)
- Slot 2 = Second story (70% of NPCs check this)
- Slot 3 = Small story (30% of NPCs check this)
- Unpublished photos are archived (never trigger reactions)

---

## 4. The Newspaper Board (World Object)

```
Location:   Centre-left of town square
Visibility: Visible to all NPCs at all times
Reset:      Each morning, new paper is posted
State A:    Empty (before first publish)
State B:    Posted (photos pinned, text implied by category)
```

### Morning Sequence (Day Start)
```
1. Board transitions from Empty → Posted (animation: paper unfurls)
2. All NPCs register NewsPostedEvent
3. Each NPC adds the board to their RoutineQueue at priority: HIGH
4. One by one (or in small groups), NPCs walk to the board
5. At 0.5 tile distance from board → NewsReadEvent fires for that NPC
6. NPC reads for 3–8s (idle animation: scan-left, scan-right, tilt-head)
7. NPC's EmotionStateMachine transitions based on their personality × news category
8. NPC leaves board, carrying new emotional state
9. Emotional state influences remainder of daily routine
```

---

## 5. NPC Reaction Matrix

### Category × Personality Trait → Emotion Output

```
Category: SCANDAL
─────────────────────────────────────────────────────────────
  High Neuroticism    → Shocked (0.9), then Anxious
  High Agreeableness  → Worried (0.7), concerned look
  High Extraversion   → Outrage gesture, seeks other NPCs
  Low Openness        → Disgust (0.8), turns back to board
  High Openness       → Curious (0.6), inspects photo closely

Category: CELEBRATION
─────────────────────────────────────────────────────────────
  High Extraversion   → Happy (0.9), immediate clap/spin
  High Agreeableness  → Warm smile, seeks someone to share with
  High Neuroticism    → Cautious happiness; looks around nervously
  Low Agreeableness   → Indifference or Envy (subtle)

Category: DISASTER
─────────────────────────────────────────────────────────────
  High Neuroticism    → Fear (1.0), rushed movement, erratic
  High Conscientiousness → Calm assessment, purposeful movement
  High Agreeableness  → Grief expression, seeks others to comfort
  Hüseyin (Elder)     → Still. One exhale. Long stare.

Category: LOCAL
─────────────────────────────────────────────────────────────
  All NPCs            → Curiosity (0.4–0.7), head-tilt, read longer

Category: GLOBAL
─────────────────────────────────────────────────────────────
  Low Openness        → Neutral, quick dismissal
  High Openness       → Thoughtful (0.6), slow walk away
  Ayşe (New Arrival)  → Always reacts to Global → adds to her forming profile
```

---

## 6. Memory & Accumulation

Every NPC stores the last **7 days of news** in a `NewsMemory` struct.

```csharp
struct NewsMemory
{
    public NewsCategory Category;
    public EmotionType   ReactionEmotion;
    public float         Intensity;
    public int           DayIndex;
}
```

### Memory Effects
- 3 consecutive days of `Disaster` → NPC enters `ChronicAnxiety` baseline state
- 3 consecutive days of `Celebration` → NPC enters `OptimistBaseline` state
- Mixed news → NPC becomes `Volatile` — wider emotional swings per event
- Forgotten after 7 days (sliding window)

---

## 7. Player Moral Dimension

The player is **never told** how their editorial choices affect NPCs. There are no "good photo" / "bad photo" labels. The player must observe, infer, and decide.

> Publishing a photo of Leila crying during a private moment might get a reaction from every NPC — but Leila herself may never approach the player again.

> Publishing a false "celebration" photo during a real disaster may temporarily calm the town — but NPCs with high conscientiousness will eventually exhibit **distrust** animations whenever they pass the Newspaper Board.

These are moral consequences written in body language. No text. No score. Only behaviour.
