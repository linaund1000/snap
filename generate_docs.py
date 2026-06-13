import os

base_dir = "/Users/emre/dev/ai/gp-oyun/Game_Architecture_Docs"

files_content = {
    "1_Base_Documentation/Docs.txt": """
GAME REQUIREMENTS & OVERVIEW
============================
The project is a Photography & Social Simulation Game (GPOyun) set in a Mediterranean town square (Santorini style).
It focuses on the cybernetic feedback loop between the player's photography (journalism) and the autonomous social dynamics of the NPCs.
The core engine is built in Unity using C# and heavily utilizes the new Input System.
""",
    "1_Base_Documentation/Current_architecture.txt": """
CURRENT ARCHITECTURE
====================
The architecture relies on a centralized Singleton Manager pattern for system orchestration, avoiding excessive EventBus abstraction in favor of direct method calls.
Core Managers:
  - GameManager: Orchestrates overall game states (Loading, Playing, Paused, GameOver).
  - TimeManager: Manages time-of-day phases (Morning, Midday, Afternoon, Evening, Night) and triggers phase-specific system events.
  - NewspaperManager: Collects captured photos, creates news editions, and distributes them to NPCs.
  - NPCManager: A registry for all active NPCs.
  - RelationshipMatrix: A centralized bidirectional memory matrix storing relational integers [-100 to 100] and textual opinions between all NPC pairs.
NPCs utilize a Utility AI architecture combined with Neural Subsystems (SensoryMatrix, AppraisalEngine, MemoryStream).
""",
    "1_Base_Documentation/Todo.txt": """
TODO LIST
=========
- Expand Utility AI actions (e.g., complete Sitting/Eating actions, pathfinding refinement).
- Deepen the Newspaper procedural headline generation based on Photo Composition.
- Add more granular reaction animations to PantomimeGestures.
- Optimize NavMesh obstacle avoidance for dense NPC crowds.
- Implement persistent JSON Save/Load for the RelationshipMatrix and NPC Memory Streams.
""",
    "2_Scenarios_Use_Cases/Scenarios_for_game.txt": """
SCENARIOS FOR GAME
==================
Scenario A: The Gossip Catalyst
1. Player photographs NPC A arguing with NPC B.
2. Photo is published as a "Scandal".
3. Next morning, NPC C reads the paper, lowers opinion of NPC A, and triggers an Angry emoji.
4. NPC A's relationship with the Player drops significantly.

Scenario B: The Hero Shot
1. Player photographs NPC D doing something positive (Local News).
2. Published as Hero News.
3. NPCs read the news, raising their relationship with NPC D.
4. NPC D gains a shiny Star emoji overlay and their relationship with the Player boosts +30.
""",
    "2_Scenarios_Use_Cases/Use_case_diagram.txt": """
USE CASE DIAGRAM (Textual)
==========================
Actors:
- Player (Journalist)
- System Time (Environment)
- NPC (Citizen)

Use Cases:
[Player] --> (Aim Camera Viewfinder)
[Player] --> (Capture Photo) -> Includes: (Evaluate Composition Score)
[Player] --> (Review Photo) -> Extends: (Store Photo in Pool)
[Player] --> (View Relationship Gossip Overlay)

[System Time] --> (Advance Day Phase)
[System Time] --> (Publish Morning Newspaper) -> Triggers: (Distribute News to NPCs)

[NPC] --> (Read Newspaper)
[NPC] --> (Evaluate News Content) -> Modifies: (Relationship Matrix)
[NPC] --> (Socialize / Gossip)
[NPC] --> (React to Camera Flash) -> Triggers: (Pose or Flee)
""",
    "2_Scenarios_Use_Cases/Usecases.txt": """
USE CASES LIST
==============
UC-01: Take Picture
UC-02: Score Photo Composition
UC-03: Publish Daily Newspaper
UC-04: NPC Read News
UC-05: Update Social Relationships
UC-06: NPC Gossip Exchange
UC-07: NPC React to Stimulus (Camera/Flash)
UC-08: Time Progression
""",
    "2_Scenarios_Use_Cases/Use_case_stories.txt": """
USE CASE STORIES
================
Story: Take Picture
As a Player, I hold the 'C' key to aim the viewfinder. I point the camera at an NPC. I press Spacebar to capture. The CameraController calculates a composition score based on the subject's screen space, creates a RenderTexture snapshot, saves it to disk as a PNG, and passes the metadata to the NewspaperManager pool.

Story: NPC Read News
As an NPC, during the Morning phase, my Utility AI increases my 'HasPendingNews' flag. My ActionPlanner selects 'ReadNewsAction'. I walk to the Newspaper Board. I read the front page story. My Appraisal Engine evaluates if the story is about me or someone else, checking my Agreeableness and Neuroticism traits, and shifts my internal relationship scores accordingly.
""",
    "3_Structural_Modeling/Class_diagram.txt": """
CLASS DIAGRAM (Textual)
=======================
Managers:
  GameManager (Singleton)
  TimeManager (Singleton)
  NewspaperManager (Singleton)
  NPCManager (Singleton)
  RelationshipMatrix (Singleton)

Player Components:
  PlayerController
  CameraController
  ViewfinderManager

NPC Components:
  NPCController
  NPCLocomotion
  PantomimeGestures
  NPCSensoryMatrix
  NPCAppraisalEngine
  NPCMemoryStream
  NPCNeeds
  NPCActionPlanner

Use Cases:
  TakePictureUseCase
  ComposeEditorialUseCase
""",
    "3_Structural_Modeling/Object_diagram.txt": """
OBJECT DIAGRAM (Textual)
========================
Instance: GameManager_Ins (GameManager)
  State: Playing

Instance: RelationshipMatrix_Ins (RelationshipMatrix)
  Relations: [npc_0_to_npc_1: 45], [npc_1_to_npc_0: 45]

Instance: NPC_0 (NPCController)
  ID: 0, Name: "Leo"
  Emotion: Neutral
  Components: Planner_0, Memory_0, Needs_0 (Energy: 80, Boredom: 20)

Instance: NewspaperManager_Ins (NewspaperManager)
  History: [Day 1: "Local Hero", Day 2: "Scandal!"]
  TodayPhotos: [PhotoData_A, PhotoData_B]
""",
    "3_Structural_Modeling/Component_diagram.txt": """
COMPONENT DIAGRAM (Textual)
===========================
[Game Client Component]
  |-- [UI Subsystem] (HUDManager, PhotoReviewUI, JournalUI)
  |-- [Camera Subsystem] (CameraController, PhotoScorer)
  |-- [Simulation Core] (GameManager, TimeManager)
  |-- [Social Engine] (RelationshipMatrix, NewspaperManager)
  |-- [AI Agents Component] (NPCController, UtilityAI Actions)
  |-- [File System Interface] (PNG Exporter, PhotoData Persistence)
""",
    "3_Structural_Modeling/Entity_Relationship_ER_diagram.txt": """
ENTITY-RELATIONSHIP (ER) DIAGRAM (Textual)
==========================================
Entity: PhotoData
  - CapturedTexture (Texture2D)
  - WorldPosition (Vector3)
  - PrimarySubject (PhotoSubject)
  - CompositionScore (Int)
  - FilePath (String)

Entity: NewsStory
  - Headline (String)
  - Category (NewsCategory Enum)
  - SourcePhoto (PhotoData)

Entity: Relationship Edge
  - FromID (Int)
  - ToID (Int)
  - Score (Int: -100 to 100)
  - Opinions (List of Strings)

Relationships:
- NewsPublishedData HAS-A FrontPage (NewsStory)
- NewsStory HAS-A SourcePhoto (PhotoData)
- RelationshipMatrix MANAGES Relationship Edges
""",
    "4_State_Behavioral_Modeling/Activity_diagram.txt": """
ACTIVITY DIAGRAM (Textual)
==========================
Activity: Morning News Publishing Loop
1. TimeManager transitions to DayPhase.Morning.
2. TimeManager calls NewspaperManager.OnMorningArrived().
3. NewspaperManager selects Best Photo from Pool.
4. NewspaperManager creates NewsStory and calls RelationshipMatrix.ProcessPublishingEvent().
5. RelationshipMatrix loops over all NPCs, calculating Relationship Deltas based on Personality.
6. RelationshipMatrix updates edge values and triggers NPC Emoji reactions.
7. NewspaperManager signals NPCManager -> NPCs get 'PendingNews' flag.
8. NPCs evaluate Utility AI and pathfind to NewspaperBoard.
""",
    "4_State_Behavioral_Modeling/Finite_state_machine_of_game.txt": """
FINITE STATE MACHINE OF GAME
============================
State: Loading
  -> On Initialized -> Playing

State: Playing
  -> On Pause Menu Opened -> Paused
  -> On End Condition Met -> GameOver

State: Paused
  -> On Resume -> Playing

State: GameOver
  -> On Restart -> Loading
""",
    "4_State_Behavioral_Modeling/Finite_state_machine_of_objects.txt": """
FINITE STATE MACHINE OF OBJECTS (NPCController)
===============================================
States:
- Idle
- Wandering
- WalkingToBoard
- Reading
- Reacting
- WalkingHome
- Sitting
- Hugging
- Fleeing
- ChillingInGroup
- Socializing
- Traveling

Transitions:
- Idle -> Wandering (Utility AI WanderAction triggers)
- Wandering -> Reacting (SensoryMatrix detects Camera Flash)
- Reacting -> Idle (Pose Coroutine completes)
- Idle -> WalkingToBoard (HasPendingNews flag triggers ReadNewsAction)
- WalkingToBoard -> Reading (Arrival at Board)
- Reading -> Reacting (Appraisal Engine processes News Story)
""",
    "4_State_Behavioral_Modeling/State_transition_table.txt": """
STATE TRANSITION TABLE (NPC)
============================
Current State     | Condition / Trigger                 | Next State
-------------------------------------------------------------------------
Idle              | Utility AI selects WanderAction     | Wandering
Wandering         | Arrive at Destination               | Idle
Any               | See Camera Flash (Neurotic > 0.6)   | Fleeing / Reacting
Any               | Morning Phase Begins                | WalkingToBoard
WalkingToBoard    | Arrive at Board Node                | Reading
Reading           | Processing News Coroutine ends      | Reacting
Reacting          | Pose Timer Expires                  | WalkingHome / Idle
""",
    "4_State_Behavioral_Modeling/Sub_state_transition_tables.txt": """
SUB-STATE TRANSITION TABLES (NPC Needs & AI)
============================================
Utility AI Evaluation Sub-state:
Current Focus      | Condition                      | Next Focus
-------------------------------------------------------------------------
Evaluate Boredom   | Boredom > 70 & Energy > 50     | Execute WanderAction
Evaluate Social    | SocialDesire > 80              | Execute GossipAction
Evaluate Energy    | Energy < 20                    | Execute GoHomeAction
Evaluate News      | PendingNews == True            | Execute ReadNewsAction
""",
    "5_System_Feedback_HCI/Cybernetic_core_loop_diagram.txt": """
CYBERNETIC CORE LOOP DIAGRAM (Textual)
======================================
[ PLAYER OBSERVATION ]
      |
      v
[ PLAYER ACTION ] (Capture Photo) -> Alters Environment State (Photo Pool)
      |
      v
[ SYSTEM PROCESS ] (Publish Newspaper) -> Aggregates Photos, Creates Headlines
      |
      v
[ MATRIX RECALIBRATION ] (RelationshipMatrix) -> Shifts Social Network based on News
      |
      v
[ NPC REACTION ] (Visual & Behavioral) -> NPCs spawn Emojis, Change Paths, Gossip
      |
      v
[ PLAYER OBSERVATION ] -> Sees consequences of journalism -> Repeats Loop
""",
    "5_System_Feedback_HCI/Sequence_diagrams.txt": """
SEQUENCE DIAGRAMS (Textual)
===========================
Sequence: NPC Reads News About Themselves
Player -> CameraController: TakePicture(Target=NPC_1)
CameraController -> NewspaperManager: StorePhoto()
TimeManager -> NewspaperManager: OnMorningArrived()
NewspaperManager -> NPC_1: ReceiveNews(Data)
NPC_1 -> UtilityAI: Increase Priority of ReadNewsAction
NPC_1 -> Board: Walk To Board
NPC_1 -> AppraisalEngine: Evaluate(NewsData)
AppraisalEngine -> NPC_1: Emotion = Angry, RelWithPlayer = -40
NPC_1 -> HUDManager: SpawnEmoji(🤬)
""",
    "5_System_Feedback_HCI/HCI_state_models.txt": """
HCI STATE MODELS (Player Interface)
===================================
Mode: Exploration (Default)
- Input: WASD to Move, Mouse Look
- Output: Standard HUD, Compass

Mode: Viewfinder Aiming
- Input: Hold 'C'
- Output: Viewfinder UI overlay, Composition Reticle, Rule-of-Thirds Grid, Depth of Field blur outside focus.

Mode: Photo Review Modal
- Input: Triggered upon Capture
- Output: Displays captured Texture2D, Composition Score, Target Name. Options: [Keep] or [Discard].

Mode: Gossip Overlay
- Input: Hold 'Tab'
- Output: Displays Relationship network lines and opinions above NPC heads.
""",
    "5_System_Feedback_HCI/Stimulus_response_matrix.txt": """
STIMULUS-RESPONSE MATRIX (Sensory & Appraisal)
==============================================
Stimulus Type             | Personality Modifiers       | Response Outcome
--------------------------------------------------------------------------------------
Camera Flash              | Neuroticism > 0.6           | Angry, Flee, Rel -15
Camera Flash              | Extraversion > 0.7          | Happy, Pose, Rel +10
Player Aimed Camera       | Has 'CameraShy' Memory      | Fearful, Flee, Rel -30
Local Hero News           | Target == Self              | Happy, Pose ⭐, Rel +30
Scandal News              | Target == Self              | Angry, Cry 💔, Rel -40
Scandal News              | Target == Other, Agreeable  | Sad, Sympathy, Rel -5
Scandal News              | Target == Other, Disagree.  | Happy, Schadenfreude, Rel -20
""",
    "6_Deterministic_Logic_Constraints/Transfer_functions_State_space_models.txt": """
TRANSFER FUNCTIONS & STATE-SPACE MODELS
=======================================
Dynamic Need Decay Function (Euler Integration):
Need(t+1) = Clamp(Need(t) + (DecayRate * DeltaTime), 0, 100)
Example:
Boredom(t+1) = Clamp(Boredom(t) + (1.5 * dt), 0, 100)

Relationship Delta Transfer Function:
Relationship(A,B,t+1) = Clamp(Relationship(A,B,t) + DeltaEvent, -100, 100)
Where DeltaEvent from News (Agreeable Personality):
DeltaEvent = Round( 25 * ReactionIntensity * (1 + Agreeableness) )

Utility Function for Actions:
Utility_ReadNews = (HasPendingNews ? 100 : 0) * (IsNightTime ? 0.1 : 1.0)
Utility_Wander = Boredom * 0.8 + (Energy > 50 ? 20 : 0)
""",
    "6_Deterministic_Logic_Constraints/Timing_diagrams.txt": """
TIMING DIAGRAMS (Textual)
=========================
System Clock (TimeManager)
0s --- 120s --- 240s --- 360s --- 480s --- 600s
 |      |        |        |        |        |
Morn   Midday   Aft.Nn   Eve.     Night    Morn (Cycle resets)
 |                                          |
 +- Newspaper Published                     +- Newspaper Published

Utility AI Evaluation Tick
NPC_0: 0.0s -> 1.0s -> 2.0s
NPC_1: 0.3s -> 1.3s -> 2.3s
NPC_2: 0.7s -> 1.7s -> 2.7s
(Staggered intervals prevent CPU spikes)
""",
    "6_Deterministic_Logic_Constraints/Boundary_condition_specifications.txt": """
BOUNDARY CONDITION SPECIFICATIONS
=================================
1. Matrix Bounds:
   All relationship scores are rigidly clamped between -100 (Absolute Rival) and +100 (Best Friend).
2. Needs Bounds:
   All physiological/psychological needs (Boredom, Energy, Social) are clamped between 0 and 100.
3. Time Bounds:
   24-hour cycle logic wraps modulo 24. Day phases are modulo 5.
4. Photo Pool Limits:
   A maximum number of photos in the Newspaper pool is enforced to prevent Memory Leaks if the player spams the shutter.
5. Physics Constraints:
   NPCs use NavMesh Obstacle Carving to avoid walking through dynamic environment props (Benches, Fountains) and each other.
""",
    "6_Deterministic_Logic_Constraints/Data_flow_diagrams_DFD.txt": """
DATA FLOW DIAGRAM (DFD)
=======================
[Player Input (WASD/Mouse)] -> (PlayerController) -> [Camera Raycast]
                                                        |
                                                        v
[Subject Metadata] <-- (PhotoSubject component) <--- (CameraController)
                                                        |
                                                        v
[Texture2D Array] + [Metadata] ---> (TakePictureUseCase) --> [File I/O: write PNG]
                                                        |
                                                        v
(NewspaperManager Pool) <-------------------------------+
        |
        v
[NewsPublishedData] ---> (RelationshipMatrix) ---> [Global Relation Dictionary Updates]
        |
        v
(NPC AppraisalEngine) ---> [Emotion Update] ---> (PantomimeGestures & Emojis)
""",
    "7_Deployment_Infrastructure/Deployment_Network_topology_diagram.txt": """
DEPLOYMENT & NETWORK TOPOLOGY DIAGRAM (Textual)
===============================================
Topology: Standalone Local Architecture (Zero Network Dependency)

[ Client Machine (Windows/Mac) ]
  |
  |-- [ Executable Environment (Unity Player) ]
  |      |-- Memory: GameManager, RelationshipMatrix, Active Scene
  |
  |-- [ Local OS File System ]
         |-- /Application.dataPath/_Game/CapturedPhotos/
         |     |-- uuid_20260613_1700_Local.png
         |     |-- uuid_20260613_1705_Scandal.png
         |
         |-- /Application.persistentDataPath/SaveFiles/
               |-- SocialState.json
               |-- TimeState.json

Data relies fully on local disk storage. No cloud persistence, no multiplayer networking.
"""
}

for filepath, content in files_content.items():
    full_path = os.path.join(base_dir, filepath)
    os.makedirs(os.path.dirname(full_path), exist_ok=True)
    with open(full_path, "w", encoding="utf-8") as f:
        f.write(content.strip() + "\\n")

print("Generated successfully!")
