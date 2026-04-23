# Codebase Header Scan (Scripts)
This document provides a snapshot of the first 5 lines of every `.cs` file in the project. Use this to quickly understand the **Imports**, **Namespaces**, and **Core Aim** of each system.

## Summary of External Packages Used
- **UnityEngine**: Core Unity runtime.
- **UnityEngine.UI**: Legacy/Standard UI components.
- **UnityEngine.InputSystem**: New Input System (Player/Camera control).
- **System / System.Collections**: Standard C# logic.
- **NUnit.Framework**: Automated Testing.

---

## 🏛️ UI System
| File | Header Preview |
|---|---|
| **SplashController.cs** | `using UnityEngine; using UnityEngine.UI; using System.Collections; namespace GPOyun.UI` |
| **EditorialUI.cs** | `using UnityEngine; using UnityEngine.UI; using GPOyun.Events; using GPOyun.Newspaper; using GPOyun.Core;` |
| **SettingsController.cs** | `using UnityEngine; using UnityEngine.UI; namespace GPOyun.UI {` |
| **HUDManager.cs** | `using UnityEngine; using UnityEngine.UI; using GPOyun.Events; namespace GPOyun.UI` |

---

## 🛠️ Core Systems
| File | Header Preview |
|---|---|
| **GPOyunBootstrap.cs** | `using UnityEngine; using GPOyun.Events; using GPOyun.Managers; using GPOyun.Newspaper; using GPOyun.Environment;` |
| **MovementBrain.cs** | `using UnityEngine; namespace GPOyun.Core { /// <summary> Higher-level coordinator...` |
| **CharacterMotor.cs** | `using UnityEngine; namespace GPOyun.Core { /// <summary> The deterministic physical driver...` |
| **ObstacleAvoidance.cs** | `using UnityEngine; namespace GPOyun.Core { /// <summary> Surgical proximity sensor...` |
| **GameManager.cs** | `using UnityEngine; using UnityEngine.Events; using GPOyun; using GPOyun.Managers; using GPOyun.Events;` |

---

## 🎭 NPC & Emotion Systems
| File | Header Preview |
|---|---|
| **NPCController.cs** | `using UnityEngine; using System.Collections; using GPOyun.Emotions; using GPOyun.Emotions.States; using GPOyun.NPC.States;` |
| **NPCStateMachine.cs** | `using UnityEngine; using GPOyun.NPC.States; namespace GPOyun.NPC {` |
| **EmotionStateMachine.cs**| `using UnityEngine; namespace GPOyun.Emotions { public class EmotionStateMachine` |
| **WalkingState.cs** | `using UnityEngine; using GPOyun.NPC; namespace GPOyun.NPC.States {` |

---

## 🌍 Environment & Managers
| File | Header Preview |
|---|---|
| **TimeManager.cs** | `using UnityEngine; using GPOyun.Events; namespace GPOyun.Managers {` |
| **AtmosphereManager.cs** | `using UnityEngine; using GPOyun.Events; using GPOyun.Managers; namespace GPOyun.Environment` |
| **TownSquareBuilder.cs** | `using UnityEngine; using GPOyun.Core; using GPOyun.NPC; using GPOyun;` |
| **NewspaperManager.cs** | `using System.Collections.Generic; using UnityEngine; using GPOyun.Events; using GPOyun.Managers;` |

---

## 📡 Events & Data
| File | Header Preview |
|---|---|
| **EventBus.cs** | `using System; using System.Collections.Generic; using UnityEngine; namespace GPOyun.Events` |
| **NPCPersonalityData.cs**| `using System; using System.Collections.Generic; using UnityEngine; using GPOyun.Events;` |

---

## 🧪 Tests & Diagnostics
| File | Header Preview |
|---|---|
| **DeterministicMovementTest.cs** | `using UnityEngine; using GPOyun.Core; namespace GPOyun.Testing {` |
| **GP_System_SmokeTest.cs** | `using UnityEngine; using GPOyun.Events; using GPOyun.NPC; using GPOyun.Newspaper;` |
| **DeterministicFSMTests.cs** | `using NUnit.Framework; using UnityEngine; using GPOyun; using GPOyun.Events; using GPOyun.NPC;` |
