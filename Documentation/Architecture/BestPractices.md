# GP-OYUN: Architectural Principles & Best Practices

To build a game that is robust, easy to debug, and "Elon-style" efficient, we must follow these core principles. This is the **"Right Way"** to scale our Mediterranean town square.

---

## 1. PREVENT: Spaghetti Code (Direct References)
**The Problem**: A script that says `Npc.newspaperManager.Publish()` creates a "hard link." If you delete the NewspaperManager, the NPC script breaks.

*   **DON'T**: Have objects "talk" to each other directly through variables.
*   **DO**: Use the **EventBus**. 
    *   *Example*: The UI sends a `NewsPublishedEvent`. Any NPC that is currently `READING` will "hear" it automatically without knowing the UI exists.

---

## 2. PREVENT: Logic in the Inspector
**The Problem**: Setting complex logic values (like NPC routine times) manually on every instance in every scene.

*   **DON'T**: Put the "Brain" data in the scene objects.
*   **DO**: Use **ScriptableObjects** (like `NPCPersonalityData`). 
    *   *Benefit*: You can update Agop's personality once in the Project folder, and every Agop in every scene updates instantly.

---

## 3. PREVENT: The "God Script"
**The Problem**: A `PlayerController` that handles movement, camera, UI, newspaper logic, and sound.

*   **DON'T**: Build a giant script that does everything.
*   **DO**: Follow the **Single Responsibility Principle**.
    *   `PlayerController` handles WASD.
    *   `CameraManager` handles the Viewfinder.
    *   `AtmosphereManager` handles lighting.

---

## 4. PREVENT: Frame-Rate Dependent Logic
**The Problem**: `if (health < 0)` checked in every `Update()`.

*   **DON'T**: Check states 60 times a second if they only change once.
*   **DO**: Use **State Changes** and **Events**.
    *   Logic should happen exactly *once* when the event fires (e.g. `OnNewsPublished`).
    *   Use the **FSM `Tick()`** for things that *must* happen continuously (like walking towards a target).

---

## 5. THE PHILOSOPHY: "Invisible Interface, Visible Content"
**The Rule**: The player should never be thinking about the "Menu." They should be thinking about "The Town."

*   **GOOD WAY**: The UI buttons only appear when you are in a specific zone (like the Office).
*   **GOOD WAY**: The Camera Shutter doesn't have a "loading bar"—it has a visual flash and a sound.
*   **GOOD WAY**: NPC reactions aren't speech bubbles—they are body language animations.

---

## 6. PREVENT: Manual Scene Mess
**The Problem**: Manually dragging 10 objects into a scene to make it work.

*   **DO**: Use the **Bootstrap Pattern**. 
    *   Our `GPOyunBootstrap` creates the core systems automatically. If you delete the EventBus by mistake, the Bootstrap recreates it. This makes your project "Indestructible."
