# GP-OYUN — Game Design Document
## File 09: Finite State Machine Specifications

---

## 1. Design Philosophy

> **Every living thing in this world has a state. Every state has rules. Every rule is testable.**

The FSM architecture is the most critical technical component of GP-OYUN. Without strict, well-tested state machines, NPC behaviour becomes unpredictable, the newspaper loses its cause-effect chain, and the game loses its soul.

All FSMs are defined as **ScriptableObject-based state definitions** in `/Scripts/FSM/` — decoupled from MonoBehaviour, independently runnable, and unit-testable.

---

## 2. NPC Master State Machine

### 2.1 State Definitions

```
┌──────────────────────────────────────────────────────────────────┐
│                     NPC MASTER FSM                                │
│                                                                  │
│  ┌──────────┐     ┌──────────┐     ┌─────────────┐              │
│  │   IDLE   │ ←──→│ WALKING  │ ←──→│  APPROACHING│              │
│  └────┬─────┘     └────┬─────┘     └──────┬──────┘              │
│       │                │                   │                     │
│       ▼                ▼                   ▼                     │
│  ┌──────────┐     ┌──────────┐     ┌─────────────┐              │
│  │ SITTING  │     │ WORKING  │     │  CONVERSING  │              │
│  └────┬─────┘     └──────────┘     └─────────────┘              │
│       │                                                          │
│       ▼                                                          │
│  ┌──────────────┐     ┌───────────┐     ┌──────────┐            │
│  │ READING_NEWS │ ──→ │ REACTING  │ ──→ │ RECOVERY │            │
│  └──────────────┘     └───────────┘     └──────────┘            │
│                                                                  │
│  ┌──────────────┐     ┌───────────┐                              │
│  │  HIGH_ALERT  │     │  FLEEING  │     ← Override states        │
│  └──────────────┘     └───────────┘                              │
└──────────────────────────────────────────────────────────────────┘
```

### 2.2 State Descriptions

| State | Entry Condition | Behaviour | Exit Condition |
|---|---|---|---|
| **IDLE** | Default / no scheduled task | Play IdleStanding or IdleSitting animation. Run ambient look gestures | Routine timer triggers next task |
| **WALKING** | Routine or event requires relocation | NavMeshAgent active, walk animation, head can still turn | Arrival at destination |
| **APPROACHING** | Another NPC or player in proximity | Walk toward target at reduced speed | Within 1.5 tiles of target |
| **SITTING** | At bench, cafe, chess table | Sit animation, can still gesture upper-body | Routine timer or emotion spike |
| **WORKING** | At work station (bakery, flower stall) | Character-specific work animations | End of work shift |
| **READING_NEWS** | At newspaper board, news is posted | Scan-left/right animation, head-tilt | Read timer expires (3–8s) |
| **REACTING** | After reading news | Emotion burst, gesture play | Emotion decays below threshold |
| **RECOVERY** | After reaction completes | Slow return to baseline, passive animation | Baseline reached, new routine starts |
| **CONVERSING** | Two NPCs facing within 1.5 tiles | Gesture exchange sequence | 1–3 exchanges complete |
| **HIGH_ALERT** | Collision event (e.g., pram), extreme emotion | Freeze + alert gesture, heightened awareness | 10s timeout or source removed |
| **FLEEING** | Player reputation < 0.2 + player proximity | Run animation, flee away from player | Player exits detection radius |

### 2.3 Priority Hierarchy

```
Priority 4 (HIGHEST):  Scripted Event  → Story beat overrides everything
Priority 3:            HIGH_ALERT      → Safety/threat override
Priority 2:            EmotionReaction → Reading news, reacting, conversing
Priority 1 (LOWEST):   Routine         → Schedule-based daily tasks
```

When a higher-priority state demands transition, the current state is **interrupted** — the NPC saves its routine position and resumes after the higher-priority state resolves.

---

## 3. Day/Night Cycle FSM

```
                    ┌─────────┐
                    │ MORNING │ ← Day starts here
                    │  06:00  │
                    └────┬────┘
                         │ After 15 min real-time (configurable)
                         ▼
                    ┌─────────┐
                    │ MIDDAY  │
                    │  10:00  │
                    └────┬────┘
                         │ After 15 min
                         ▼
                    ┌───────────┐
                    │ AFTERNOON │
                    │   14:00   │
                    └─────┬─────┘
                          │ After 10 min
                          ▼
                    ┌─────────┐
                    │ EVENING │
                    │  18:00  │
                    └────┬────┘
                         │ Player publishes or timeout 10 min
                         ▼
                    ┌─────────┐
                    │  NIGHT  │ → 5s real-time transition
                    │  22:00  │
                    └────┬────┘
                         │ Reset all systems, advance day counter
                         └──→ MORNING (next day)
```

### Phase Triggers

| Phase Transition | Systems Notified |
|---|---|
| → MORNING | Board posts news, NPCs start routines, weather set |
| → MIDDAY | Peak NPC activity flag, photo value bonus |
| → AFTERNOON | Peak social interaction, internal emotion ripple max |
| → EVENING | Office opens, NPCs wind down, lamp posts ON |
| → NIGHT | Board cleared, props reset, emotions processed, day++ |

---

## 4. Newspaper Board FSM

```
              ┌─────────┐
              │  EMPTY  │ ← Initial state + post-Night reset
              └────┬────┘
                   │ Morning phase + player published last evening
                   ▼
          ┌─────────────────┐
          │  POSTING (anim) │ → Paper unfurl animation (2s)
          └────────┬────────┘
                   │ Animation complete
                   ▼
              ┌──────────┐
              │  POSTED  │ → NPCs can read
              └────┬─────┘
                   │ Night phase
                   ▼
          ┌────────────────┐
          │ CLEARING (anim)│ → Paper removed animation (1s)
          └────────┬───────┘
                   │ Animation complete
                   └──→ EMPTY
```

### Board Interaction Rules
- Only transitions to POSTED if `NewsPublishedEvent` was fired previous evening
- If player didn't publish → Board stays EMPTY all day → NPCs have NO news reaction
- NPCs approach board in priority order: Fatma (always first), then random

---

## 5. Bench Object FSM

```
         ┌───────────┐
         │   EMPTY   │
         └─────┬─────┘
               │ NPC enters bench trigger zone
               ▼
      ┌─────────────────┐
      │  OCCUPIED_ONE   │ → NPC plays sit animation
      └────────┬────────┘
               │ Second NPC enters + relationship > 0.6
               ▼
      ┌─────────────────┐
      │  OCCUPIED_TWO   │ → Both seated
      └────────┬────────┘
               │ After 10s idle
               ▼
      ┌────────────────────┐
      │    CONVERSATION    │ → Silent conversation event
      └────────┬───────────┘
               │ Conversation complete or NPC routine calls
               ▼
         Back to OCCUPIED_ONE or EMPTY
```

---

## 6. Cafe Table FSM

```
         ┌───────────┐
         │   EMPTY   │
         └─────┬─────┘
               │ NPC with CafeTime routine arrives
               ▼
       ┌─────────────────┐
       │  SEATED_ONE     │
       └────────┬────────┘
               │ Second NPC arrives
               ▼
       ┌─────────────────┐
       │  SEATED_TWO     │ → Conversation triggers
       └────────┬────────┘
               │ If angry NPC arrives (Anger ≥ 0.8)
               ▼
       ┌─────────────────┐
       │   CONFRONTATION │ → Angry NPC gestures, seated NPCs react
       └────────┬────────┘
               │ After 5s: angry NPC leaves
               │ Rare: TABLE_FLIP animation
               ▼
         Back to SEATED or EMPTY
```

---

## 7. Chess Game FSM

```
     ┌───────────────┐
     │   WAITING     │ ← Hüseyin at table, Mustafa not yet arrived
     └──────┬────────┘
            │ Mustafa enters chess trigger
            ▼
     ┌───────────────┐
     │   PLAYING     │ → Both seated, move gestures every 30–60s
     └──────┬────────┘
            │ After 8–12 exchanges
            ▼
     ┌───────────────┐
     │   GAME_END    │ → Winner: happy gesture, Loser: brief disgust → laugh
     └──────┬────────┘
            │ After result animation (5s)
            ▼
     ┌───────────────┐
     │   COOLDOWN    │ → Both idle, reflect, may start new game or leave
     └───────────────┘
```

---

## 8. Pram (Selin) FSM

```
     ┌──────────────┐
     │    NORMAL    │ ← Default, Selin pushes pram through square
     └──────┬───────┘
            │ Physics prop enters pram collider
            ▼
     ┌──────────────┐
     │  COLLISION   │ → Selin: Fear +0.8, Anger +0.4
     └──────┬───────┘ → Nearby NPCs: Surprise +0.6
            │ 3s duration
            ▼
     ┌──────────────┐
     │  HIGH_ALERT  │ → Selin: ProtectivePose, scans for source
     └──────┬───────┘
            │ 10s duration, no new collision
            ▼
     ┌──────────────┐
     │   RECOVERY   │ → Selin: slow exhale, resumes pushing
     └──────┬───────┘
            │ 5s
            └──→ NORMAL
```

---


## 10. Player Reputation FSM

```
     ┌──────────────┐         ┌──────────────┐
     │   UNKNOWN    │ ──────→ │   TRUSTED    │
     │  (Days 1-3)  │         │  (Rep > 0.6) │
     └──────┬───────┘         └──────────────┘
            │                         ↑
            │                         │ Compassionate coverage
            ▼                         │
     ┌──────────────┐         ┌──────────────┐
     │   NEUTRAL    │ ←─────→ │   CAUTIOUS   │
     │ (Rep 0.3-0.6)│         │ (Rep 0.2-0.3)│
     └──────────────┘         └──────┬───────┘
                                     │ Continued exploitation
                                     ▼
                              ┌──────────────┐
                              │   DISTRUSTED │ → NPCs flee from camera
                              │  (Rep < 0.2) │
                              └──────┬───────┘
                                     │ Rep < 0.1 for 3 days
                                     ▼
                              ┌──────────────┐
                              │    EXILED    │ → Game Over
                              └──────────────┘
```

---

## 11. Town Mood Index FSM

```
     ┌──────────────────┐
     │    THRIVING      │ ← Mood > 0.7 (warm colours, open NPCs)
     └────────┬─────────┘
              │ Mood drops below 0.7
              ▼
     ┌──────────────────┐
     │     STABLE       │ ← Mood 0.4–0.7 (default state)
     └────────┬─────────┘
              │ Mood drops below 0.4
              ▼
     ┌──────────────────┐
     │    ANXIOUS       │ ← Mood 0.2–0.4 (NPCs cluster, routines break)
     └────────┬─────────┘
              │ Mood drops below 0.2
              ▼
     ┌──────────────────┐
     │     CRISIS       │ ← Mood < 0.2 (NPCs leave, scene desaturates)
     └─────────────────┘
```

---

## 12. Implementation Notes

### File Structure
```
/Scripts/FSM/
  ├── States/
  │    ├── NPCState.cs              — abstract base class
  │    ├── NPCIdleState.cs
  │    ├── NPCWalkingState.cs
  │    ├── NPCSittingState.cs
  │    ├── NPCReadingNewsState.cs
  │    ├── NPCReactingState.cs
  │    ├── NPCConversingState.cs
  │    ├── NPCHighAlertState.cs
  │    └── NPCFleeingState.cs
  ├── Objects/
  │    ├── BoardFSM.cs
  │    ├── BenchFSM.cs
  │    ├── CafeTableFSM.cs
  │    ├── ChessGameFSM.cs
  │    └── PramFSM.cs
  ├── Systems/
  │    ├── DayCycleFSM.cs
  │    ├── GossipFSM.cs
  │    ├── ReputationFSM.cs
  │    └── TownMoodFSM.cs
  └── Tests/
       ├── NPCStateTests.cs
       ├── DayCycleTests.cs
       └── BoardFSMTests.cs
```

### Testing Strategy
- All FSMs must be testable **without Unity** (pure C# logic, no MonoBehaviour)
- Mock EventBus for unit tests
- Each FSM must have:
  - Valid state transition test
  - Invalid state transition test (verify rejection)
  - Edge case: concurrent event handling
  - Performance test: 8 NPC FSMs running 1000 ticks
