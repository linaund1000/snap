# GP-OYUN: Master Transition Tables

This document serves as the final technical authority for state transitions across all game systems.

---

## 1. Local Time & Day Cycle FSM
**Manager**: `TimeManager`
**Event**: `DayPhaseChangedEvent`

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `NIGHT` | Timer (5s) | `MORNING` | Reset `_hasReadTodayNews` for all NPCs. |
| `MORNING` | Timer (Config) | `MIDDAY` | Start NPC routines. |
| `MIDDAY` | Timer (Config) | `AFTERNOON` | Peak photo value window. |
| `AFTERNOON` | Timer (Config) | `EVENING` | Lighting shift; gossip propagation peak. |
| `EVENING` | `NewsPublished` OR Timer | `NIGHT` | Stop all movement; process daily reputation. |

---

## 2. NPC Master Behaviour FSM
**Manager**: `NPCStateMachine` (Physical Layer)

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `ANY` | `NPCHighAlertState` | `IDLE` / `FLEEING` | (Override) Reset current path if threat detected. |
| `IDLE` | `DayPhase == Morning` | `WALKING` | Wait for random stagger delay (0-15s). |
| `WALKING` | `ArrivalAtDestination` | `IDLE` | Default behavior. |
| `WALKING` | `ArrivalAtBoard` | `READING_NEWS` | Only if `_pendingNews != null`. |
| `READING_NEWS` | `Timer (3-8s)` | `IDLE` | Calls `ProcessReadNews()` on exit. |
| `IDLE` `WALKING` | `ProximityDetected` | `CONVERSING` | `random(0,1) < socialChance`. |
| `CONVERSING` | `Timer (4-6s)` | `IDLE` | Call `TryShareNews()` halfway. |

---

## 3. Emotion Axis FSM
**Manager**: `EmotionStateMachine` (Internal Layer)

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `ANY` | `ProcessReadNews` | `Reaction` | Target emotion determined by `PersonalityData`. |
| `IDLE` `WALKING` | `ProximityDetected` | `SOCIAL_INIT` | `random(0,1) < socialChance`. |
| `SOCIAL_INIT` | `Acknowledge` | `GOSSIP_EXCHANGE` | Shared rotation complete. |
| `GOSSIP_EXCHANGE`| `Success` | `SOCIAL_FAREWELL` | Call `TryShareNews()` halfway. |
| `SOCIAL_FAREWELL`| `Timer (1s)` | `IDLE` | Resume previous task. |

---

## 4. UI Master System FSM
**Manager**: `UIManager`

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `HUD_ONLY` | `Hold(C)` | `CAMERA_SYS` | Slow player movement speed. |
| `CAMERA_SYS` | `Release(C)` | `HUD_ONLY` | Restore movement speed. |
| `HUD_ONLY` | `OnOfficeEntry (Evening)` | `NEWSPAPER_ED` | Lock movement; open Editor. |
| `NEWSPAPER_ED`| `ConfirmPublish` | `HUD_ONLY` | Fires `NewsPublishedEvent`. |
| `HUD_ONLY` | `OnEscape` | `SETTINGS_MENU` | Pause game simulation. |

---

## 5. Sub-FSM: Camera & Newspaper
| Start State | Trigger | Target State | Guard |
|---|---|---|---|
| **[CAMERA]** `READY` | `CaptureInput` | `SHUTTER` | `Cooldown <= 0`. |
| **[CAMERA]** `SHUTTER`| `StoreComplete` | `COOLDOWN` | Save photo to Roll. |
| **[EDITOR]** `SELECT` | `PhotoSelected` | `CATEGORIZE` | Roll count > 0. |
| **[EDITOR]** `CATEGORIZED`| `Confirmed` | `HUD_ONLY` | Close UI; transition to Night. |

---

## 6. Sub-State Machine: Settings
**Manager**: `SettingsController`

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `GENERAL` | `TabClick(Audio)` | `AUDIO` | Load volume sliders. |
| `AUDIO` | `TabClick(Graphics)` | `GRAPHICS` | Load quality presets. |
| `ANY` | `OnApply` | `GENERAL` | Commit changes to `PlayerPrefs`. |

---

## 6. Sub-State Machine: Notifications
**Manager**: `NotificationQueue`

| Start State | Event / Trigger | Target State | Logic / Guard |
|---|---|---|---|
| `IDLE` | `NewNotification` | `FADE_IN` | Pop from FIFO Queue. |
| `FADE_IN` | `AnimComplete` | `DISPLAYING` | 2-3s duration. |
| `DISPLAYING` | `TimerEnd` | `FADE_OUT` | Move to next in queue. |
| `FADE_OUT` | `AnimComplete` | `IDLE` | Check if Queue is empty. |
