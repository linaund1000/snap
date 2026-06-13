# Game Architecture Issues & Refactoring Roadmap

PRIORITY | ISSUE_NAME | DESCRIPTION | REFERENCED_FILES | ACTION_PLAN
---|---|---|---|---
High | Singleton_Coupling | Core managers tightly coupled; testing impossible. | GameManager.cs, TimeManager.cs | Remove Instance accessors. Implement IoC Container.
High | Synchronous_IO_Stutter | Capturing photos blocks main thread, freezing game. | TakePictureUseCase.cs, CameraController.cs | [DONE] Move Texture2D.EncodeToPNG to async Task.Run.
High | Matrix_Garbage_Collection | String concatenation in relationship matrix causes memory spikes. | RelationshipMatrix.cs | [DONE] Replace string keys with struct NPCPair.
High | Dual_Brain_Desynchronization | Enum state and Utility AI drift out of sync. | NPCController.cs, NPCActionPlanner.cs | Replace Enum FSM with GOAP or Behavior Tree.
High | Missing_System_Feedback | Player cannot interrogate NPC state without clunky UI. | HUDManager.cs, JournalUI.cs | Implement Target Inspector UI via raycast.
High | Unseeded_RNG_Logic | Random.Range used in Utility AI, destroying determinism. | NPCActionPlanner.cs, NewspaperManager.cs | Instantiate global PRNG with fixed master seed.
Med | Non_Deterministic_Timing | Integration relies on Time.deltaTime in Update loops. | TimeManager.cs, NPCNeeds.cs | Move simulation logic to FixedUpdate.
Med | Blocking_UI_Coupling | Opening journal directly pauses the simulation loop. | JournalUI.cs, GameManager.cs | Implement global EventBus (OnJournalOpened).
Med | Hardcoded_Magic_Numbers | Emotional deltas (+10, -30) are hardcoded. | AppraisalEngine.cs | Extract to SimulationConfig.asset ScriptableObject.
Med | Unbounded_Memory_Streams | NPC logs append infinitely to List, causing memory leak. | NPCMemoryStream.cs | Convert List<string> to fixed-size Ring Buffer.
Med | Missing_Composition_Feedback | Player doesn't know why a photo scored poorly during aiming. | ViewfinderManager.cs, PhotoScorer.cs | Add UI reticle glowing based on InterestLevel frustum intersection.
Low | Legacy_OnGUI_Usage | Emoji reactions use obsolete Unity 4 UI methods. | HUDManager.cs | Replace OnGUI with TextMeshPro Canvas objects.
Low | Dead_End_Transitions | NPCs trapped in WalkingHome state when morning arrives. | NPCController.cs, MoveAction.cs | Flush active queue on TimeManager.OnMorningArrived.
Low | Procedural_Gen_Stagnation | Town built via code primitives, blocking level designers. | TownSquareBuilder.cs | Replace GameObject.CreatePrimitive with Prefab Instantiation.
Low | Missing_Audio_Cues | Systemic feedback lacks spatial audio confirmation. | None (Missing System) | Create AudioManager listening to EventBus.

## What is Missing (Architectural Gaps)
1. **Audio Subsystem:** Completely missing. Audio is fragmented PlayOneShot calls.
2. **Serialization Layer:** No ISaveable interfaces. Simulation resets every time the game is closed.
3. **Event Bus / PubSub System:** Missing entirely. Systems communicate via direct Singleton method calls.
4. **Editorial UI:** Missing ComposeEditorialUseCase; player forced to publish everything they shoot.
5. **Dynamic Population Handler:** Assumes exactly 10 NPCs. Missing logic for death, birth, or departure.
