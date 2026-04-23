# Unity Scene Setup Guide — Gp-Oyun

This guide explains how to wire your GameObjects in the Unity Editor to match our C# logic.

---

## 1. The Core Infrastructure

The "Managers" are the brain of the game. They must be present in every scene.

1.  **Create a Bootstrap**:
    - Build a new GameObject named `[BOOTSTRAP]`.
    - Attach the `GPOyunBootstrap.cs` script.
    - Right-click the script in the Inspector and select **"Scene Setup: Create Hierarchy"**. This will automatically create the `[CORE]` group for you.
2.  **Verify Managers**:
    - Under `[CORE]`, you should now see **EventBus**, **TimeManager**, and **NewspaperManager**.

---

## 2. Setting Up the Town (Proto-Town)

1.  Create an empty GameObject named `[ENVIRONMENT]`.
2.  Attach the **`TownSquareBuilder.cs`** script.
3.  Right-click and select **"Build Proto-Town Square"**.
4.  **CRITICAL**: Go to **Window > AI > Navigation (Obsolete)** and click **Bake**. NPCs cannot move until the NavMesh is baked on the ground plane.

---

## 3. The NPC Prefab

1.  Create a **Capsule** in your scene.
2.  Attach the **`NPCController.cs`** and **`NPCVisualHelper.cs`** scripts.
3.  Add a **NavMeshAgent** component.
4.  Add an **Animator** component.
5.  **Tagging**: Ensure the Newspaper Board object has the Tag `NewspaperBoard`.
6.  **Drag to Project**: Drag this Capsule into your `_Game/Prefabs` folder to save it.

---

## 4. The Player & Camera

1.  Create a **Cube** (The Player).
2.  Attach the **`PlayerController.cs`** script.
3.  Attach a **CharacterController** component.
4.  **The Camera**:
    - Ensure your **Main Camera** is a child of the Player OR positioned to view the whole square (2.5D style).
    - In `PlayerController`, the script expects `Camera.main` to exist.

---

## 5. Atmosphere (Lighting)

1.  Attach the **`AtmosphereManager.cs`** script to any object.
2.  Drag your **Directional Light** into the "Main Directional Light" slot in the Inspector.

---

## 6. How to Test
1.  Click **Play**.
2.  Wait for the **TimeManager** to advance the phase.
3.  Watch the Console for `[NPC_X] Reacted to news`.
4.  If NPCs don't move, check your **NavMesh Bake**.
