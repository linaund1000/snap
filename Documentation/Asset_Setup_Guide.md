# Technical Guide: NPC & Asset Setup (MVP)

This guide explains how to use your existing `.obj` files with the GP-OYUN MVP code foundation.

## 1. Importing OBJ Files
1. Drag your `.obj` files into the `Assets/_Game/Art/Models/` folder in Unity.
2. In the **Import Settings** (Inspector), ensure:
   - **Scale Factor**: Typically 1.0, but adjust if your models appear too small/large.
   - **Extract Materials**: Click "Extract Materials" to a local folder so you can edit colors.

## 2. Setting Up an NPC Prefab
Since we don't have rigging yet, we will treat the model as a "statue" that slides/moves via the NavMesh.

1. Create a new **Empty GameObject** in your scene and name it after your NPC (e.g., `NPC_Agop`).
2. Drag your imported `.obj` model as a **child** of this GameObject.
3. Add the following components to the parent `NPC_Agop`:
   - **NavMesh Agent**: Handles movement logic.
   - **Animator**: Required by the controller (even if empty for now).
   - **NPC Controller**: The script I just updated. 
4. **Configuration in Inspector**:
   - **NPC ID**: Assign a unique number (1-8).
   - **Personality**: Create and assign an `NPCPersonalityData` ScriptableObject.
   - **Target Position**: Create an Empty GameObject called `NewspaperBoard_Target` in your scene and assign it here. This is where the NPC will walk in the Morning.

## 3. NavMesh Setup
1. Define your ground (e.g., a Plane or Cube) and set it to **Static**.
2. Go to `Window > AI > Navigation`.
3. In the **Bake** tab, click **Bake**. Your NPCs can now move!

## 4. Simple "Fake" Animations (Optional)
Because your meshes are unrigged, they will "slide" across the floor. To make them feel more alive:
- Create a simple **Animation Controller** with a "Speed" parameter.
- Add an animation that subtly **bobs** the child mesh up and down or scales it slightly when moving.
- The `NPCController` automatically updates the `Speed` parameter based on movement.

## 5. Camera Setup
1. Select your `MainCamera`.
2. Set **Projection** to `Orthographic`.
3. Set **Position** to `(0, 18, -12)` and **Rotation** to `(45, 0, 0)` as per the design docs.
4. Set **Size** to `9.5`.

---
**Next Step**: Once you have your NPC moving to the board, you can test the **Capture** mechanic by pressing `Space` while walking around as the player!
