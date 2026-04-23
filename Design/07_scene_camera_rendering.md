# GP-OYUN — Game Design Document
## File 07: Scene Layout, Camera & Rendering Pipeline

---

## 1. The One-Screen World

The entire game is designed to fit in a **single, fixed camera view** — no scroll, no pan, no zoom.

```
Aspect Ratio:  16:9
Resolution:    1920 × 1080 (reference)
Camera Type:   Orthographic
Projection:    Isometric 45° (X/Z ground plane, Y vertical)
Camera Height: Static — positioned above and at angle
```

The constraint of "everything on one screen" is a **design pillar**, not a limitation. Players must read and manage 8+ characters simultaneously — like watching a film through a fixed frame.

### ⚠️ MAP DESIGN RULES (League of Legends Style)

```
 VISIBILITY RULES:
 ────────────────────────────────────────────────────────
 1. Characters must ALWAYS be visible — no exceptions
 2. Trees are the ONLY objects that may partially block camera view
 3. ALL buildings, architecture, and structures must stay SHORT
    → Max building height: 3 units (below NPC head at camera angle)
 4. Markets, cafes, and public buildings are set slightly BACK
    from the main walkway areas — nothing blocks the main stage
 5. The "town square" centre must be COMPLETELY clear of tall objects
 6. Furniture and props never exceed 1.5 units height
 7. Background buildings (edges) can be taller but are behind camera clip
```

Think of it like a LoL map: the terrain is readable, the characters are clear, and the environment serves the gameplay — never obscures it.

---

## 2. Camera Setup

```
GameObject:      MainCamera
Position:        (0, 18, -12)         ← elevated and pulled back
Rotation:        (45°, 0°, 0°)        ← 45° downward tilt
Orthographic Size: 9.5
Culling Mask:    All layers except "HiddenUI"

No:
  - Camera shake (no trauma, no combat)
  - Field-of-view changes
  - Zoom in/out (everything must be readable at all times)
  - Camera follow (player walks into view naturally)
```

---

## 3. Scene Zone Layout (Grid Reference)

World is on a 32 × 18 unit grid. Everything is positioned within this space.

```
     0    4    8    12   16   20   24   28   32
  0  ┌────┬────┬────┬────┬────┬────┬────┬────┐ 0
     │BAKR│           PARK ZONE              │
  4  │    ├────┤     [FOUNTAIN]   [CHESS]    │ 4
     │    │    │  [BENCH A]    [BENCH B]     │
  8  ├────┤    ├────┬──────────────┬─────────┤ 8
     │ALLY│    │NEWS│  TOWN SQUARE │ BARBER  │
  12 │    │FLWR│BORD│              │ SHOP    │ 12
     │    │STLL│    │   [POST BOX] │         │
  16 ├────┴────┴────┴──────────────┴─────────┤ 16
     │  [CAFE A] [CAFE B]  [CAFE C] [NW OFF] │
  18 └────────────────────────────────────────┘ 18
```

### Zone Descriptions

| Zone | Grid Coords | Primary Purpose |
|---|---|---|
| PARK | (4,0)–(28,7) | Leila sketching, Hüseyin + Mustafa chess, benches |
| BAKERY | (0,0)–(4,8) | Agop's station, morning service |
| ALLEY | (0,8)–(4,12) | Ayşe's entry point, quiet zone |
| FLOWER STALL | (4,8)–(8,14) | Mihail's station |
| NEWSPAPER BOARD | (8,8)–(12,12) | Central emotional hub |
| TOWN SQUARE | (12,7)–(24,15) | Main crossing zone, pigeons, fountain |
| BARBER SHOP | (24,8)–(32,14) | Background NPC activity |
| CAFE ZONE | (0,14)–(24,18) | Tables A, B, C |
| NEWSPAPER OFFICE | (24,14)–(32,18) | Player end-of-day submission |

---

## 4. Rendering Layers (Unity Sorting Layers)

For correct 2.5D depth, objects use Y-position-based sorting.

```
Sorting Layers (back → front):
─────────────────────────────
  Background     — sky, far buildings (static, no sort)
  Ground         — cobblestones, grass, floor
  GroundDecals   — puddles, shadows on ground
  Props_Low      — flower beds, benches, low objects
  Props_Mid      — tables, fountain, counter
  Props_High     — lamp posts, tree trunk, board
  NPC_Body       — character meshes (Y-sorted per frame)
  Player_Body    — player mesh (always rendered above NPC)
  Props_Overlay  — objects that go in front of characters
  VFX            — particles, dust, paper flutter
  UI_World       — world-space UI (emotion indicators if needed)
  UI_Screen      — camera UI (photo frame, day counter)
```

### Y-Based Sorting (Critical for 2.5D)
```csharp
// Each NPC/prop renderer updates sortingOrder per frame:
renderer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 10);

// More negative Z = further from camera = lower sort order (drawn behind)
// More positive Z = closer to camera = higher sort order (drawn in front)
```

---

## 5. Lighting Design

```
Type:     Baked + Mixed lighting
Skybox:   Procedural — shifts warm to cool with DayPhase

Lights:
  DirectionalLight_Sun  — baked, angle softens with time-of-day shader
  PointLight_LampPosts  — realtime, activate at Evening phase
  PointLight_Bakery     — warm orange glow, always on during open hours
  AmbientOcclusion      — SSAO pass, subtle depth under props and NPCs

Emotional Lighting (subtle, never obvious):
  When TownMoodIndex < 0.3: scene desaturates slightly (post-process volume)
  When TownMoodIndex > 0.7: warmer colour grading, slight vignette reduction
```

---

## 6. Day / Night Visual Phases

```
Phase 1: MORNING
  Sky:          Cool amber horizon
  Scene:        Soft morning haze
  Lamp Posts:   OFF
  Board:        Paper posting animation plays at phase start

Phase 2: MIDDAY
  Sky:          Full warm white-blue
  Scene:        Sharpest shadows, peak contrast
  NPCs:         All routines running at peak activity

Phase 3: AFTERNOON
  Sky:          Golden hour creep
  Shadows:      Long, low-angle
  Emotional ripples from newspaper at maximum

Phase 4: EVENING
  Sky:          Dusk purple-orange
  Lamp Posts:   ON (warm point light flicker-on animation)
  Scene:        Reduced saturation
  Player:       Newspaper Office door glows (submission prompt)

Phase 5: NIGHT (Transition)
  Sky:          Dark — stars procedural
  NPCs:         Exit to edges or idle at designated spots
  Board:        Paper removed animation
  Scene:        Full desaturate → fade to black → fade in MORNING
  Duration:     5 seconds real time (skip/stylised transition)
```

---

## 7. NavMesh Layout

```
NavMesh Surface: baked on Ground layer only (XZ plane at Y=0)

NavMesh Obstacles (static):
  - Bakery interior walls
  - Chess table outer bounds
  - Fountain centre
  - Newspaper Board front face
  - Lamp posts

NavMesh Obstacles (dynamic):
  - Each bench when occupied (prevents NPCs standing on seated friends)
  - Cafe tables when two seated
  - Selin's pram (moving obstacle)

Agent Settings:
  Radius:   0.4 units
  Height:   1.8 units
  Speed:    NPC-specific (Hüseyin: 0.8, Leila: 1.2, Fatma: 1.6)
  Priority: Fatma=50, Player=1 (everyone yields to nobody but player)
```

---

## 8. Visual Identity Summary

```
Colour Palette:
  Ground:        #D4C5A9 warm stone
  Buildings:     #E8DCC8 plaster, #8B6914 timber
  Sky (Morning): #FFD4A3 → #87CEEB
  Sky (Dusk):    #FF8C42 → #6B3FA0
  Accent Joy:    #FFD700 gold
  Accent Anger:  #C0392B deep red
  Accent Sad:    #5B8DB8 slate blue
  Accent Fear:   #8E44AD muted purple

Typography (on Newspaper UI only):
  Headline:   Playfair Display Bold
  Caption:    EB Garamond Regular
  Date Line:  Courier Prime

Font Rule: No in-world text is readable. Street signs, shop signs, 
           newspaper text — all artistically blurred or in a 
           fictional alphabet. The world is language-agnostic.
```
