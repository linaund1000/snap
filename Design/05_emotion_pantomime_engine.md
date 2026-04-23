# GP-OYUN — Game Design Document
## File 05: Emotion System & Pantomime Engine

---

## 1. The Pantomime Principle

> **There are no words in this world. Every feeling is architecture.**

The entire game's communication layer is built on the **Pantomime Engine** — a system that translates internal emotional state data into physically visible, universally legible character performances.

The challenge: make an audience of one (the player) understand exactly what each of 8+ characters is experiencing — simultaneously — without a single word or sound effect that implies speech.

### ⚠️ CRITICAL DESIGN PRIORITY: BODY > FACE

```
 PRIORITY ORDER (highest to lowest):
 ─────────────────────────────────────
 1. Body posture & movement     ← THIS IS THE GAME
 2. Arm/hand gestures           ← Core readability
 3. Head movement (nod, shake)  ← Clear at distance
 4. Emoji-style floating icons  ← Support layer
 5. Clothing/physics response   ← Subtle reinforcement
 6. Facial blend shapes         ← LOWEST priority (barely visible at 45° iso)
```

**Why?** At isometric camera distance (45°, ortho size 9.5), facial micro-expressions are nearly invisible. Players read CHARACTER through HOW THEY MOVE, not how their eyebrows twitch. Detailed facial rigs are expensive to create and invisible to the player.

**Rule:** If a gesture can communicate the emotion, the face doesn't need to. Face is a bonus, never the primary channel.

---

## 2. The Emotion Model

We use a **6-axis emotion model** derived from Paul Ekman's basic emotions, adapted for gameplay.

```
EMOTION AXES (0.0 = none, 1.0 = peak)
─────────────────────────────────────
  Joy         0.0 ───────────────── 1.0
  Sadness     0.0 ───────────────── 1.0
  Anger       0.0 ───────────────── 1.0
  Fear        0.0 ───────────────── 1.0
  Surprise    0.0 ───────────────── 1.0
  Disgust     0.0 ───────────────── 1.0
```

At any moment, each NPC has a **composite emotional state** — not just one emotion but a weighted blend:

```
Example: Leila reads Disaster news
  Joy:      0.00
  Sadness:  0.65
  Fear:     0.40
  Surprise: 0.20
  Disgust:  0.00
  Anger:    0.05
```

This composite drives both the **face** and the **body** independently.

---

## 3. Facial Expression System

### 3.1 Blend Shape Architecture (Per Character)
Each character FBX has the following blend shape regions:

```
UPPER FACE
  BS_Brow_Raise_L / R       — eyebrow surprise/fear/query
  BS_Brow_Furrow_L / R      — anger/concentration
  BS_Brow_Sad_L / R         — sadness innerBrow lift

EYE REGION
  BS_Eye_Wide_L / R         — surprise/fear
  BS_Eye_Squint_L / R       — joy/disgust
  BS_Eye_Lid_Lower_L / R    — sadness lid droop

NOSE
  BS_Nose_Wrinkle           — disgust

MOUTH
  BS_Mouth_Smile_L / R      — joy
  BS_Mouth_Frown_L / R      — sadness
  BS_Mouth_Open             — surprise/fear
  BS_Mouth_Press            — anger suppression
  BS_Lip_Bite               — anxiety
  BS_Jaw_Drop               — shock
```

### 3.2 Blend Shape Driver (Runtime)
```csharp
// EmotionToFaceMapper drives blend shapes from float[] EmotionVector
void UpdateFace(float[] emotions)
{
    // Map each axis to blend shape weights
    face.SetBlendShape("BS_Brow_Raise",   emotions[Surprise] * 0.8f + emotions[Fear] * 0.6f);
    face.SetBlendShape("BS_Brow_Furrow",  emotions[Anger]);
    face.SetBlendShape("BS_Brow_Sad",     emotions[Sadness] * 0.9f);
    face.SetBlendShape("BS_Mouth_Smile",  emotions[Joy]);
    face.SetBlendShape("BS_Mouth_Frown",  emotions[Sadness] * 0.7f);
    face.SetBlendShape("BS_Eye_Wide",     emotions[Fear] * 0.85f + emotions[Surprise] * 0.7f);
    face.SetBlendShape("BS_Jaw_Drop",     emotions[Surprise] * 0.9f);
    face.SetBlendShape("BS_Nose_Wrinkle", emotions[Disgust]);
    // etc.
}
```

### 3.3 Face Transition Rules
- Blending speed: fast emotions (Surprise) snap in ≤ 0.3s
- Slow emotions (Sadness) interpolate over 1.5–2.5s
- No teleporting between poles — always smooth Lerp
- After peak: gradual return to Neutral over 3–10s depending on personality.Neuroticism

---

## 4. Body Language System

The body layer is separate from the face — an NPC can have a happy face but tense body (layered performance).

### 4.1 Animator Parameters (per NPC)

```
FLOAT parameters (0-1):
  EmotionJoy
  EmotionSadness
  EmotionAnger
  EmotionFear

TRIGGER parameters:
  Emotion_Neutral
  Emotion_Shock          — one-shot flinch
  Emotion_CryBrief       — brief moment, returns to base
  Emotion_Stomp          — anger expression
  Emotion_Jump           — joy burst

BOOL parameters:
  IsReading              — at newspaper board
  IsSeated
  IsWalking
```

### 4.2 Animation Layers

```
Layer 0: Base Locomotion    — Idle, Walk, Run variants
Layer 1: Upper Body Gesture — additive, arms/hands
Layer 2: Head & Neck        — head turn, nodding, tilting
Layer 3: Face Rig           — blend shape driver mask
```

Layers 1–3 use **Additive blending** so a walking NPC can simultaneously gesture with arms and express with face.

---

## 5. Gesture Library

The core gestural vocabulary of the game — readable at 2.5D distance.

| Gesture Name | Trigger | Body Description |
|---|---|---|
| `Nod_Slow` | Approval / Agreement | One slow forward head dip |
| `Nod_Fast` | Enthusiastic yes | Three rapid bobs |
| `HeadShake` | Disagreement | Side-to-side, 2–3 times |
| `Shrug` | Confusion / Don't know | Shoulders up, palms out |
| `ArmsCross` | Anger / Defensiveness | Arms fold, chin down |
| `HandsToMouth` | Shock / Horror | Both hands fly to mouth |
| `ClaspHands` | Relief / Hope | Hands pressed together |
| `PointAt` | Drawing attention | Extended index finger, eyeline match |
| `WaveGoodbye` | Farewell | Full arm sweep |
| `Stomp` | Rage | Foot down, arms tense |
| `EmbraceAir` | Happiness / Opening | Arms open wide |
| `LookAround` | Anxiety / Surveillance | Repeated head checks L/R |
| `BowHead` | Grief | Head drops, shoulders curl in |
| `Spin_Small` | Delight burst | Quarter-turn on spot |
| `SketchAir` | Leila-only: inspiration | Mimes drawing strokes in air |
| `FeedPigeons` | Hüseyin-only: calm | Slow hand arc, scatter |
| `ThumbsUp` | Acknowledgment | Clear thumb gesture |
| `Clap` | Celebration | Visible, bouncy handclap |

---

## 6. NPC-to-NPC Silent Communication

When two NPCs are within 1.5 tiles and facing each other, a **Conversation Event** triggers.

### 6.1 Silent Conversation Flow
```
1. ProximityDetector fires: bool TwoNPCsFacing
2. Both NPCs lock NavMeshAgent: stop walking
3. Initiator plays Greeting gesture
4. Receiver plays Response gesture (auto-selected from RelationshipData)
5. 1–3 "exchanges" play (Gesture → Counter-Gesture)
6. Conversation ends: both play Farewell gesture
7. NavMesh resumes
```

### 6.2 Gesture Response Rules
```
Initiator: HAPPY_GREETING  →  Receiver (Friendly):  HAPPY_GREETING
                           →  Receiver (Neutral):   NOD_SLOW
                           →  Receiver (Hostile):   ARMS_CROSS + LOOK_AWAY

Initiator: SCANDAL_MIME    →  Receiver (Neurotic):  HANDS_TO_MOUTH
(re-enacting news)         →  Receiver (Stable):    HEAD_SHAKE + SHRUG
                           →  Receiver (Agreeable): CLASPHAND + WORRIED

Initiator: COMFORT_GESTURE →  Receiver (Sad):       BOW_HEAD → SMALL_NOD
                           →  Receiver (Angry):      ARMS_CROSS (rebuffs)
```

---

## 7. Emotion Decay & Baseline Return

All emotional states have **half-lives** — they naturally decay toward an NPC's personality baseline.

```csharp
void Update()
{
    // Every emotion decays toward resting value
    currentJoy     = Mathf.Lerp(currentJoy,     personality.RestingJoy,     decayRate * Time.deltaTime);
    currentAnger   = Mathf.Lerp(currentAnger,   personality.RestingAnger,   decayRate * Time.deltaTime);
    // ... etc.

    // New events inject a value pulse
    // void ReactToNews(float injectedEmotion, float axis) {
    //     currentEmotion[axis] = Mathf.Clamp01(currentEmotion[axis] + injectedEmotion);
    // }
}
```

| Personality Trait | Decay Behaviour |
|---|---|
| High Neuroticism | Slow decay — emotions linger |
| Low Neuroticism | Fast decay — returns to neutral quickly |
| High Agreeableness | Anger decays 30% faster |
| Low Agreeableness | Joy decays 30% faster |

---

## 8. The "Peak Moment" System

A **Peak Moment** is when an NPC reaches ≥ 0.85 on any single emotion axis. This is the ideal photo moment.

```
Peak Moment conditions:
  → Face is at maximum expressiveness
  → Body language is at its most readable
  → A soft visual indicator appears (subtle rim light or particle — invisible to NPCs)
  → Lasts 2–4 seconds before decay begins
```

The camera UI frame glows faintly when pointing at a NPC in Peak Moment state. The player must decide: capture this vulnerable moment, or respect it.
