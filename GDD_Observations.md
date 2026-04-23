# Design & GDD Observations: GP-OYUN
o

This document captures thoughts on the current design and technical implementation gaps for the Mediterranean 2.5D Pantomime project.

## 1. Aesthetic Considerations (Mediterranean Minimalist)
- **Visual Gaps**: The current "Zero-Asset" approach using Unity primitives is effective for rapid prototyping. However, to achieve a truly premium "Santorini" vibe, we should introduce:
    - **Rounded Edges**: Santorini architecture is famous for its soft, organic white curves. We could implement a shader-based rounding or use slightly more complex primitive compounds.
    - **Shadow Depth**: Minimalist designs rely on lighting. We should ensure high-quality soft shadows and perhaps a "Cel-Shaded" look that doesn't use heavy outlines, but rather flat color pools.
- **Color Palette**: The Cobalt/Stucco/Terracotta palette is excellent. We should stick to it strictly, avoiding any default Unity greens/grays.

## 2. Gameplay Loop (The "Gap")
- **Photo Utility**: Currently, the `CapturedTexture` in the `PhotoCapturedEvent` is `null`. The NPC reaction logic is also simplified. 
    - **Gap**: We need a way to "read" what is in the photo. 
    - **Solution**: A simple tag-based system (e.g., `PhotoSubject` component on NPCs/Objects) that the Camera detects via Raycast or SphereOverlap.
- **NPC Routine**: NPCs currently wander or sit. 
    - **Gap**: Social interaction. 
    - **Solution**: A "Gathering" state where NPCs clump together to react to the Board as a group, creating a "Theatre of Life" feel.

## 3. Technical Architecture (Compilation & Safety)
- **Dependency Issues**: The project relies heavily on `FindObjectOfType` and singleton access. This is fragile in Unity if objects aren't initialized in the exact right frame.
- **"Nuclear" Cold Start**: The `GPOyunBootstrap` is a great safety net, but it should ideally be part of a `SceneBootstrapper` that runs automatically on every scene load in the Editor to prevent the "Missing Managers" errors you encountered.

## 4. Documentation Strategy
- **Pantomime Logic**: Since the game has no text-based dialogue, the "GDD" should focus on **Animation State Tables**. Every emotion needs a corresponding procedural bob or rotation pattern (e.g., Happy = rapid bob, Sad = slight tilt and slow float).
