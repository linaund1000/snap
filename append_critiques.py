import os

base_dir = "/Users/emre/dev/ai/gp-oyun/Game_Architecture_Docs"

critiques = {
    "1_Base_Documentation/Docs.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Outdated / Misaligned.
Critique: The documentation describes a cybernetic feedback loop, but currently, System Feedback is poorly implemented for communication with the user. After all usage (taking photos, NPCs reading news), we need something robust to communicate the simulation's state to the user. Right now, it's just floating emojis and a hidden scoreboard.
""",
    "1_Base_Documentation/Current_architecture.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Badly Implemented.
Critique: Singleton abuse is rampant. `SocialGroupManager`, `HUDManager`, `GameManager`, `TimeManager`, and `NewspaperManager` are all Singletons creating hard dependencies. This makes testing impossible and tightly couples the UI to core simulation logic.
""",
    "1_Base_Documentation/Todo.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Disconnected Logic.
Critique: Needs a massive refactoring phase. `SitAction` is missing entirely despite being referenced in the day phases. "After all usage we need someting to do there" - the UI and system feedback require a major overhaul.
""",
    "2_Scenarios_Use_Cases/Scenarios_for_game.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Poorly Connected.
Critique: The gossip and hangout scenarios are partially hardcoded. `GroupHangoutAction` attempts to group NPCs but the UI/System feedback doesn't properly communicate this intent to the user, leading to confusing visual clustering without context.
""",
    "2_Scenarios_Use_Cases/Use_case_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Incomplete System Feedback.
Critique: The System Feedback loop is broken. The user takes a picture, but there is no mechanism to tell the user *why* an NPC reacted a certain way to the news. We need a way for the user to interrogate the system.
""",
    "2_Scenarios_Use_Cases/Usecases.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Bad Implementations.
Critique: Missing clear use cases for Player feedback. Right now viewing the social matrix is hidden behind a Tab key overlay that is visually cluttered and hard to parse. System feedback is for communication with the user, and this fails at it.
""",
    "2_Scenarios_Use_Cases/Use_case_stories.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Wrong Continuations.
Critique: The stories are too happy-path. When an action fails (e.g., photo has no subject, or group hangout fails to find members), the system silently fails instead of providing meaningful feedback to the player.
""",
    "3_Structural_Modeling/Class_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: God Object Antipattern.
Critique: `NPCController` is a massive god class (450+ lines) taking on too many responsibilities despite having sub-components. It handles movement, relationship modifications, and UI emoji spawning directly.
""",
    "3_Structural_Modeling/Object_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Fragmented State.
Critique: Object state is heavily fragmented. `RelationshipMatrix` holds opinions in strings, `NPCNeeds` holds floats, and `NPCController` holds an Enum that conflicts with Utility AI.
""",
    "3_Structural_Modeling/Component_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Tightly Coupled UI.
Critique: `UI Subsystem` is tightly coupled to core logic. For example, `JournalUI` directly pauses the game via `GameManager.Instance`, breaking encapsulation.
""",
    "3_Structural_Modeling/Entity_Relationship_ER_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Badly Implemented Data Structures.
Critique: Relationships are stored as simple string keys `npc_0_to_npc_1` in a Dictionary, making bidirectional matrix operations and lookups extremely slow and messy.
""",
    "4_State_Behavioral_Modeling/Activity_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Blocking Operations.
Critique: Activities lack clear interruption boundaries. A newspaper publish triggers a synchronous mass evaluation across all NPCs that causes frame drops.
""",
    "4_State_Behavioral_Modeling/Finite_state_machine_of_game.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Too Simplistic / Wrong Continuations.
Critique: Missing transition logic from Paused back to specific sub-states. The state machine is far too simplistic for a simulation game and fails to handle edge cases.
""",
    "4_State_Behavioral_Modeling/Finite_state_machine_of_objects.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Conflicting State Machines.
Critique: The FSM overlaps with Utility AI. Utility AI sets `_activeAction` but `NPCController` has its own `currentState` enum. They often drift out of sync, causing broken animations.
""",
    "4_State_Behavioral_Modeling/State_transition_table.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Dead Ends.
Critique: Dead ends everywhere. Example: `WalkingHome` state doesn't transition back to `Idle` cleanly in the morning, causing NPCs to get stuck in their houses.
""",
    "4_State_Behavioral_Modeling/Sub_state_transition_tables.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Hardcoded Logic.
Critique: Needs evaluation intervals are hardcoded in `NPCActionPlanner` instead of being event-driven or exposed to the designer.
""",
    "5_System_Feedback_HCI/Cybernetic_core_loop_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: System Feedback is Failing.
Critique: System feedback is meant for communication with the user, but it is currently bad. The player sees floating emojis but cannot interact or interrogate the system to understand *why* the matrix recalibrated. After all usage, we need something better here.
""",
    "5_System_Feedback_HCI/Sequence_diagrams.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Synchronous Chaining.
Critique: Synchronous chaining of events. `NewspaperManager` iterates all NPCs synchronously and directly modifies UI. This is bad for both performance and architectural decoupling.
""",
    "5_System_Feedback_HCI/HCI_state_models.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Disjointed UI States.
Critique: The UI states overlap improperly. The HUD overlay overlaps with the Photo Review modal, causing visual bugs and input conflicts (e.g. mouse lock breaking).
""",
    "5_System_Feedback_HCI/Stimulus_response_matrix.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Magic Numbers.
Critique: Magic numbers everywhere. Reaction deltas (like +10 or -30) are hardcoded directly into `AppraisalEngine` and `RelationshipMatrix` rather than being data-driven via ScriptableObjects.
""",
    "6_Deterministic_Logic_Constraints/Transfer_functions_State_space_models.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Non-deterministic Integration.
Critique: Euler integration relies on `Time.deltaTime` across multiple classes instead of a unified, fixed simulation tick. This causes simulation divergence depending on the user's framerate.
""",
    "6_Deterministic_Logic_Constraints/Timing_diagrams.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Non-deterministic Timers.
Critique: Staggered evaluation intervals in `NPCActionPlanner` use `Random.Range` on Start, which makes debugging impossible and causes non-deterministic simulation execution.
""",
    "6_Deterministic_Logic_Constraints/Boundary_condition_specifications.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Duplicated Logic.
Critique: Clamping logic for relationships (-100 to 100) and Needs (0 to 100) is duplicated across multiple scripts instead of being handled by a dedicated bounded data type.
""",
    "6_Deterministic_Logic_Constraints/Data_flow_diagrams_DFD.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Messy Data Flow.
Critique: Data flow is bidirectional and messy. UI reads from core logic, but core logic also directly invokes UI singletons (e.g. spawning emojis), breaking MVC patterns.
""",
    "7_Deployment_Infrastructure/Deployment_Network_topology_diagram.txt": """
[CURRENT STATUS & CRITIQUE]
Status: Main Thread Blocking.
Critique: No abstraction for saving/loading. `NewspaperManager` and `CameraController` directly encode and write PNG files on the main thread, causing severe game stutters during capture.
"""
}

for filepath, critique in critiques.items():
    full_path = os.path.join(base_dir, filepath)
    if os.path.exists(full_path):
        with open(full_path, "a", encoding="utf-8") as f:
            f.write("\\n" + critique.strip() + "\\n")
        print(f"Appended critique to {filepath}")
    else:
        print(f"File not found: {filepath}")

print("All critiques appended successfully!")
