# How To: Running Smoke Tests & Verification

This guide explains how to verify that the game's core "Theatre" (the News loop) is working correctly using our automated and manual testing tools.

---

## 1. The Interactive Smoke Test (Best for Beginners)
This test simulates a full day of game logic directly in your Unity console.

### How to Run:
1.  **Open Unity** and ensure your Scene has an **EventBus**, **TimeManager**, and **NewspaperManager** (all created in previous steps).
2.  **Create an Empty GameObject** in your Hierarchy and name it `[TestRunner]`.
3.  **Drag the `GP_System_SmokeTest.cs`** script onto this object.
4.  **Assign an NPC**: Drag any NPC in your scene into the **"Test Npc"** slot on the script.
5.  **Enter Play Mode** (Click the Play icon at the top).
6.  **Right-click "GP System Smoke Test"** in the Inspector.
7.  Select **"Run Full Game Cycle Test"**.

### What to Look For:
- Check the **Console Window**. You should see the logs progressing:
    - `1. Phase -> Evening.`
    - `2. Player publishes a SCANDAL photo.`
    - `3. Phase -> Night -> Morning.`
    - `4. NPC Reacted to news after reading.`
- If you see these logs, your **Data Pipeline** is 100% healthy.

---

## 2. The Deterministic Logic Tests (Editor Mode)
These tests verify that the FSM logic (Internal Brains) is correct without needing to run the game.

### How to Run:
1.  In the top menu, go to **Window > General > Test Runner**.
2.  Switch to the **EditMode** tab.
3.  Click **Run All**.

### Why use this?
If you change a state transition rule (e.g., you want NPCs to react to news *twice*), these tests will tell you immediately if you broke a core rule before you even open your scene.

---

## 3. Manual Data Checking
If you want to manually verify data during the game:

1.  Select an NPC in the Hierarchy while the game is running.
2.  Look at the **NPC Controller** component.
3.  Our `Debug.Log` statements will tell you their internal states (e.g., `[NPC_1] Reacted to news`).
