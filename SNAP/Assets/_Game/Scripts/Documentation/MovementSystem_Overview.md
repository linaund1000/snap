---
# GP-OYUN: Movement Pipeline Documentation
This document outlines the entire 'Input → Animation' pipeline, explaining how characters move in this Mediterranean Social Simulation.

## 1. The Physical Driver: `CharacterMotor`
Located at: `Assets/_Game/Scripts/Core/CharacterMotor.cs`

The **Motor** is the lowest level. It doesn't know *why* it is moving; it only knows *how* to apply physics.
- **Acceleration/Deceleration**: Implements Miyazaki-influenced "weighty" movement.
- **Gravity**: Keeps characters grounded or lets them fall from heights.
- **Grounding**: Uses `Physics.Raycast` to detect the floor (Layer 6).
- **Steering**: Accepts a `Vector3` input direction and turns smoothly towards it.

## 2. The Navigator: `MovementBrain`
Located at: `Assets/_Game/Scripts/Core/MovementBrain.cs`

The **Brain** adds a layer of intelligence to the motor.
- **Obstacle Avoidance**: It uses the `ObstacleAvoidance` component to detect walls (Layer 7) and "nudges" the input direction to prevent collisions.
- **Safe Movement**: It ensures that if a character is told to walk into a wall, they glide along it instead of getting stuck.

## 3. Input Sources
### 3.1 Player Input (`PlayerController`)
- **Source**: `UnityEngine.InputSystem.Keyboard`.
- **Logic**: Directly maps WASD keys to an isometric Vector3.
- **Process**: `Keyboard -> PlayerController -> MovementBrain -> CharacterMotor`.

### 3.2 NPC Behaviour (`NPCStateMachine`)
- **Source**: AI States (e.g., `NPCWalkingState`).
- **Logic**: Calculates a vector towards a target (Wander, Bench, or News Board).
- **Process**: `State -> NPCController -> MovementBrain -> CharacterMotor`.

## 4. Visual Layer: `NPCController` (Bobbing)
The vertical "Spirit-like" movement is strictly visual!
- **Logic**: `Mathf.Sin(Time.time)` is applied to the local position of the **Body** and **Head** game objects.
- **Separation**: This ensures that while characters look like they are floating/bobbing, their **CharacterController** remains firmly on the ground for deterministic physics and collision.

## 5. Summary Flow
1. **INTENT**: Player presses [W] OR AI decides to walk to a bench.
2. **DIRECTION**: An "Input Direction" is calculated and passed to the **Brain**.
3. **AVOIDANCE**: The **Brain** rayscans for obstacles and modifies the direction.
4. **VELOCITY**: The **Motor** takes the final direction and smoothly accelerates/decelerates.
5. **PHYSICS**: The Motor applies Gravity and calls `CharacterController.Move()`.
6. **VISUAL**: The `NPCController` adds the "Ghostly Bob" purely for aesthetic feel.
---
