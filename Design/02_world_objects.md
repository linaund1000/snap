# GP-OYUN — Game Design Document
## File 02: World Objects, Environment & Props

---

## 1. World Layout — The Town Square

The entire game takes place in a **single contiguous scene**. Think of it as a theatre stage — the camera never moves, but the stage is alive.

```
┌─────────────────────────────────────────────────────────────┐
│  [BAKERY]  [PARK BENCH] [FOUNTAIN]  [PARK BENCH] [LIBRARY]  │
│                                                              │
│  [FLOWERS]  [PIGEONS]              [CHESS TABLE] [OLD TREE] │
│                                                              │
│  [ALLEY]   [NEWSPAPER BOARD]    [POST BOX]  [NEWSPAPER OFF] │
│                                                              │
│  [CAFE]    [FLOWER STALL]  [TOWN SQUARE]   [BARBER SHOP]    │
│                                                              │
│  [STEPS]   [BENCH]     [COBBLESTONE PATH]   [CLOCK TOWER]   │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Static Environment Objects

These have no logic. They are purely visual/aesthetic but anchored to the grid for pathfinding purposes.

| Object | Position Zone | Purpose |
|---|---|---|
| Cobblestone Ground | All | Base tile layer |
| Old Stone Fountain | Centre | Landmark, NPC gathering point |
| Clock Tower | Top-right | Readable time indicator |
| Old Oak Tree | Park zone | Shade, NPC idle spot |
| Flower Beds | Park perimeter | Seasonal colour change |
| Brick Walls | Edges | Scene boundaries |
| Street Lamp Posts | Grid paths | Atmospheric lighting |
| Window Boxes | Building facades | Detail, seasonal state |
| Pigeons | Park zone | Ambient life — scatter on approach |

---

## 3. Interactive Environment Objects

Objects the player and NPCs can meaningfully interact with.

### 3.1 Newspaper Board
```
Tag:     NewspaperBoard
Type:    Trigger Zone (all NPCs)
State:   Empty | HasNews
Visual:  Wooden board with pinned paper + photos when HasNews
Logic:
  - Publishes at start of each morning
  - All NPCs navigate to it within 60s of morning start
  - Triggers NewsPublishedEvent for each NPC in proximity
  - Each NPC's reaction plays when they are within 1 tile of board
```

### 3.2 Park Bench (×2)
```
Tag:     ParkBench
Type:    Seat point (NPCs occupy, player can sit)
Capacity: 1-2 per bench
Logic:
  - NPC with Sad / Contemplative state will navigate here
  - Two NPCs on same bench may trigger a silent dialogue interaction
  - Good photo opportunity zone
```

### 3.3 Bakery Counter
```
Tag:     BakeryInterior
Type:    Service Point
Logic:
  - NPCs visit once per day (morning routine)
  - Baker NPC stands behind counter all morning
  - Unlocks "Free Sample" gesture event during high-joy states
```

### 3.4 Cafe Tables (×3)
```
Tag:     CafeTable
Type:    Social Seat
Capacity: 2 per table (groups)
Logic:
  - NPCs pair-up based on relationship data
  - Sitting triggers Idle_Drinking animation + gesture exchanges
  - Conflict news can cause table-flip behaviour (animation event)
```

### 3.5 Chess Table
```
Tag:     ChessTable
Type:    Two-person interaction
Logic:
  - Occupied by Elder NPC pair each afternoon
  - Captures a 2-NPC emotional exchange (good photo opportunity)
  - Outcome of "game" (gesture winner) shifts both NPCs' mood
```

### 3.6 Newspaper Office (Player)
```
Tag:     NewspaperOffice
Type:    Player Interaction Zone, End-of-Day Trigger
Logic:
  - Player enters to open Photo Selection UI
  - Drag-drop up to 3 photos to publish slots
  - Assign category tag per photo
  - Confirm publishes → triggers night cycle
```

### 3.7 Post Box
```
Tag:     PostBox
Type:    Story Beat Trigger
Logic:
  - Certain NPCs visit here to "receive" off-screen events
  - Post visit triggers EmotionEvent with category from letter content
  - Rare events only — surprise mechanic
```

### 3.8 Chess Pieces / Props
```
Tag:     PickupProp
Type:    Physics prop (Rigidbody)
Logic:
  - Angry NPCs can knock props over (physics event)
  - Knocked props stay displaced until "night reset"
  - Displaced prop in player's photo adds "Chaos" bonus tag
```

---

## 4. Collision Layer Map

```
Layer 0: Ground         — walkable, no collisions
Layer 1: Walls/Bounds   — NavMesh boundary, physics blocker
Layer 2: Props          — triggers for NPC: proximity events
Layer 3: NPC Bodies     — used for overlap detection between NPCs
Layer 4: Player         — unique layer: triggers different events than NPC
Layer 5: Trigger Zones  — invisible, fires events (Board area, Cafe, Bench)
```

---

## 5. Ambient World Objects (Non-interactive but animated)

| Object | Animation | Purpose |
|---|---|---|
| Pigeons | Flock, scatter on approach | Life, mood indicator |
| Wind Mill (background) | Slow spin | Atmospheric |
| Laundry Line | Cloth sim in breeze | Depth layer |
| Shop Sign | Slow swing | Wind ambiance |
| Newspaper Pages | Flutter and drift (Night only) | Transition FX |
| Rain Puddles | Ripple shader | Weather state |

---

## 6. Prefab Naming Convention

```
PRF_NPC_[Name]           — NPC character prefabs
PRF_ENV_[Object]         — Static environment
PRF_PROP_[Object]        — Interactive props
PRF_UI_[Element]         — UI prefabs
PRF_SFX_[Source]         — Audio source objects
PRF_VFX_[Effect]         — Particle/shader effects
```

---

## 7. NavMesh Notes

- NavMesh baked on Layer 0 (Ground)
- All bench/table/board zones have **NavMesh Obstacle** during occupation
- NPCs use **NavMeshAgent** with priorities: Routine > EmotionReaction > Idle
- Player has no NavMeshAgent — free WASD movement with physics collision
