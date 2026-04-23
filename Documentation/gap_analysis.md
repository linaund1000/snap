# GP-OYUN: Gap Analysis & Path to Readiness

After auditing the **Master GDD**, **FSM Specifications**, and **Newspaper System** docs against the current source code, here is the official situation report.

## 1. Ready vs. Not Ready (Audit)

| System | status | Priority | Description |
| :--- | :--- | :---: | :--- |
| **Foundation** | **READY** | - | EventBus, Basic NPC Registry, Phase-based TimeManager are functional. |
| **Namespace Architecture** | **READY** | - | Refactored into GPOyun.NPC, Core, Events, etc. (Stable). |
| **Internal Emotions** | **READY** | - | FSM for Anger, Joy, Fear, Surprise, Sadness is implemented. |
| **Physical Behaviour** | **NOT READY** | HIGH | Only Idle, Walking, Reading implemented. Missing: **Sitting, Working, Conversing, HighAlert, Fleeing**. |
| **Editorial System** | **NOT READY** | HIGH | No "Roll" (Photo selection) or UI for choosing headline slots. |
| **Environmental Logic** | **NOT READY** | MED | Interactive objects (Benches, Cafe, Chess) lack FSMs/Logic. |
| **Social Persistence** | **NOT READY** | MED | NPCs don't have "NewsMemory" (last 7 days) or "Reputation" tracking. |
| **Pantomime Engine** | **MOCK** | LOW | Currently using 3D primitives with simple bobbing. Needs gesture-replacement system. |

---

## 2. The Path Forward (Roadmap)

### Phase 1: Completing the Interaction Loop (Next Steps)
- [ ] **Editorial Brain**: Build `NewspaperRoll.cs` to store captured photos.
- [ ] **Editorial UI**: Create the drag-and-drop UI mockup for Slot 1, 2, 3 publishing.
- [ ] **Memory System**: Add `NewsMemory` to `NPCController` and track reactions over 7 days.

### Phase 2: World Enrichment (The "Living" Square)
- [ ] **Bench Logic**: Implement `BenchFSM.cs` so NPCs can sit and dwell by the fountain.
- [ ] **Gossip Logic**: Implement the "Information Hub" (Fatma) so news spreads physically.
- [ ] **Selin & The Pram**: Add the "High Alert" collision system for chaos tracking.

### Phase 3: Finishing the NPC Brain
- [ ] **Physical States**: Implement `NPCSittingState`, `NPCWorkingState`, and `NPCConversingState`.
- [ ] **Priority Override**: Ensure `HIGH_ALERT` (collision) overrides `ROUTINE` (walking).

### Phase 4: Polish & "Exile" Condition
- [ ] **Reputation Engine**: Build the `ReputationManager` to track if the player is being exploitative.
- [ ] **Game Over Loop**: Implement the "Exile" sequence (Night phase end with 0 reputation).

---

## 3. Comparison Breakdown: Current Code vs. Specs

### Master FSM
- **Specs**: 11 Physical states.
- **Code**: 3 Physical states.
- **Gap**: Missing the "Social" states (Conversing, Sitting together).

### News Logic
- **Specs**: Category weight depends on OCEAN personality traits (e.g. High Neuroticism → Shocked).
- **Code**: Uses a simplified switch case in `NPCController`.
- **Gap**: Needs integration with `NPCPersonalityData` ScriptableObjects for precise reactions.

### Day/Night Cycle
- **Specs**: Midday (Peak Activity), Evening (Lamp Posts ON), Night (Reset props).
- **Code**: Basic state change.
- **Gap**: Lacks the actual "Visual transition" effects besides light color.
