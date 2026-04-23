# GP-OYUN — Game Design Document
## File 06: Collision, Triggers & Event Listeners

---

## 1. Architecture Overview

All game events flow through a **two-layer system**:

```
Layer A: Physics/Spatial Events — Unity Collider triggers (OnTriggerEnter / Stay / Exit)
Layer B: Logic Events           — EventBus (C# events, no MonoBehaviour coupling)

Physical event → fires EventBus message → any listener processes it
```

This means no system talks directly to another. The EventBus is the single communication backbone.

---

## 2. Trigger Zone Registry

Every trigger zone in the world is a named, catalogued component.

### 2.1 `TRG_NewspaperBoard`

```
GameObject:   NewspaperBoard
Collider:     BoxCollider — 2 units wide, 1 unit deep, 1.5 units tall (IsTrigger: true)
Layer:        TriggerZones (Layer 5)

OnTriggerEnter (NPC tag):
  → Fire: NewsReadEvent { NpcId, BoardState }

OnTriggerEnter (Player tag):
  → Show UI prompt: "Inspect paper" (not implemented in v1)

OnTriggerStay (NPC):
  → NPC plays ReadingIdle animation while inside collider

OnTriggerExit (NPC):
  → NPC returns to walking routine
```

### 2.2 `TRG_BakeryZone`

```
Collider: BoxCollider, entrance zone of bakery
Radius:   3 × 2 units

OnTriggerEnter (NPC):
  → If NPC has no current meal → Queue: walkToBakery routine
  → If it is Agop's morning shift → Fire: ServiceEvent { NPCId }

OnTriggerEnter (Player):
  → No mechanical effect in v1; Agop plays greeting gesture
```

### 2.3 `TRG_BenchZone_A` / `TRG_BenchZone_B`

```
Collider: SphereCollider, radius 1.2

OnTriggerEnter (NPC):
  → If bench occupied by another NPC → check Relationship.Sentiment
       > 0.6 → sit beside them (triggers TwoNPCSeated event → conversation)
       < 0.3 → sad NPC turns away, finds another spot
  → If bench empty → NPC sits, enters IdleSitting state

TwoNPCSeated event:
  → After 10s idle: trigger a silent conversation sequence (see Emotion doc §6)
```

### 2.4 `TRG_FountainArea`

```
Collider: CylinderCollider, radius 3

OnTriggerEnter (Leila NPC):
  → Immediately stop walking, pull out sketchbook
  → After 5s: face fountain, play SketchAir gesture loop

OnTriggerEnter (Any NPC + EmotionJoy ≥ 0.7):
  → NPC pauses, looks at fountain water, plays Peaceful idle variant

OnTriggerEnter (Player):
  → Camera captures fountain → adds "Atmospheric" tag to photo
```

### 2.5 `TRG_ChessTable`

```
Collider: BoxCollider, 2.5 × 2.5 units

OnTriggerEnter (Hüseyin NPC):
  → If Mustafa is already at table → Hüseyin sits → ChessGameStart event
  → Else → Hüseyin waits (standing idle with watch-check gesture)

ChessGameStart event:
  → Both NPCs sit, ChessGame coroutine begins
  → Every 30–60s: one NPC plays a move gesture (hand move above board)
  → Other NPC reacts: Nod (good move) or HeadShake (surprised by move)
  → Winner determined by random weighted by mood: happier NPC wins more often
  → Loser: brief Disgust → then laugh gesture → back to neutral
```

### 2.6 `TRG_CafeTable_[A/B/C]`

```
Each table: BoxCollider, 1.5 × 1.5 units, capacity 2

OnTriggerEnter (NPC + routine = "CafeTime"):
  → Assign seat, sit animation
  → If second NPC arrives → TwoNPCSeated → conversation begins

OnTriggerEnter (NPC + EmotionAnger ≥ 0.8):
  → NPC approaches but does NOT sit
  → Stands, gestures (pointing, frustrated arms)
  → Other seated NPCs: LookAround + HandsToMouth
  → After 5s: angry NPC walks away → NPCs at table: exhale, shrug
```

### 2.7 `TRG_PrambZone` (Selin's Pram)

```
Collider: SphereCollider on Pram object, radius 0.8 (IsTrigger: true)

OnTriggerEnter (Physics prop — e.g., knocked chess piece):
  → Fire: PramCollisionEvent
  → Selin: face snaps to pram, HighAlert emotion burst (Fear: +0.8, Anger: +0.4)
  → Nearby NPCs: Startle (Surprise: +0.6, brief LookAround)
  → Offending NPC (if caused by them): ExaggeratedApology gesture

OnTriggerEnter (Player):
  → Selin looks at player, plays ProtectivePose (crosses in front of pram)
  → No mechanics — pure readable body language
```

### 2.8 `TRG_GossipRadius` (Fatma)

```
Type:     Dynamic — attached to Fatma as she moves
Collider: SphereCollider, radius 2.0, updates with Fatma's position

OnTriggerEnter (Any NPC):
  → If Fatma.CarriedEmotion != Neutral:
       Fire: GossipPropagationEvent {
           TargetNPC,
           EmotionCategory = Fatma.CarriedEmotion,
           Intensity = Fatma.CarriedIntensity * 0.4  // reduced version
       }
  → Target NPC plays "Heard something" head-turn gesture
  → Target NPC receives 40% of Fatma's current emotional charge
```

### 2.9 `TRG_PostBox`

```
Collider: SphereCollider, radius 1.0

OnTriggerEnter (Ayşe NPC — during PostBox routine):
  → Play: CheckMail gesture
  → After 3s: reveal one of 3 possible "letter states" via Animator trigger:
       GoodNews  → Joy (+0.5) + eyes-wide gesture
       BadNews   → Sadness (+0.7) + head-bow
       NoMail    → Shrug + HeadShake
  → Letter state randomised daily, weighted by town mood index
```

### 2.10 `TRG_MihailAura` (Flower Vendor proximity effect)

```
Collider: SphereCollider on Mihail, radius 2.5

While any NPC is inside:
  → Per-second: apply mood modifier
       If Mihail.Joy ≥ 0.5 → target NPC Joy += 0.01 * dt (gentle push)
       If Mihail.Sadness ≥ 0.5 → target NPC Sadness += 0.015 * dt

Design intent: Mihail is a passive mood field. Time near him nudges neighbours.
```

---

## 3. EventBus Event Type Reference

All events published during collision/trigger states:

```csharp
// Core Published Events
NewsReadEvent            { int NpcId, BoardState board }
ServiceEvent             { int NpcId, Offering offer }
TwoNPCSeatedEvent        { int NpcA, int NpcB, Vector3 TablePosition }
ChessGameStartEvent      { int ElderA, int ElderB }
ChessGameEndEvent        { int WinnerId, int LoserId }
PramCollisionEvent       { int PropId, Vector3 CollisionPoint }
GossipPropagationEvent   { int TargetNpcId, EmotionType Emotion, float Intensity }
PhotoCapturedEvent       { Texture2D Photo, PhotoTags Tags, Vector3 WorldPosition }
NewsPublishedEvent       { Texture2D[] Photos, NewsCategory[] Categories }
NPCEmotionChangedEvent   { int NpcId, EmotionType Emotion, float Intensity }
DayPhaseChangedEvent     { DayPhase Phase }  // Morning, Midday, Afternoon, Evening, Night
```

---

## 4. Listener Architecture Per System

```
GameManager         → DayPhaseChangedEvent
NewspaperManager    → NewsPublishedEvent     → board state update
NPCController (×8)  → NewsPublishedEvent     → individual emotion reaction
                    → GossipPropagationEvent → secondary emotion push
CameraSystem        → PhotoCapturedEvent     → store in session roll
SoundManager        → NPCEmotionChangedEvent → play ambient non-verbal audio
TownMoodTracker     → NPCEmotionChangedEvent → update Town Mood Index
```

---

## 5. Collision Layer Matrix

| | Ground | Walls | Props | NPC Bodies | Player | Triggers |
|---|---|---|---|---|---|---|
| **Ground** | — | — | ✓ | ✓ | ✓ | — |
| **Walls** | — | — | ✓ | ✓ | ✓ | — |
| **Props** | ✓ | ✓ | ✓ | — | — | — |
| **NPC Bodies** | ✓ | ✓ | — | — | ✓ | ✓ |
| **Player** | ✓ | ✓ | — | ✓ | — | ✓ |
| **Triggers** | — | — | — | ✓ | ✓ | — |

> ✓ = layers collide / detect  
> Props don't collide with NPC bodies (NPCs pass through props unless scripted)  
> Props do collide with Ground and Walls (physics-based)
