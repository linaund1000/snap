# Neural Agentic Brain Architecture & Design Principles

Bu doküman, oyundaki NPC'lerin geleneksel FSM (Finite State Machine) yapısından kurtulup, kendi kendine karar verebilen, içsel dürtülerine ve dış dünyaya tepki veren **Agentic (Özerk) Neural Network** tabanlı bir beyne geçişinin tasarımını içerir.

## 1. Core Principles (Temel Prensipler)

1. **Continuous Need Decay (Sürekli Metabolizma):** NPC'nin ihtiyaçları (sosyalleşme, can sıkıntısı, enerji) zamanla sürekli değişir. NPC oyunun akışını beklemez, dürtüleri onu harekete geçirir.
2. **Event-Driven Sensory Input (Dışsal Reaktivite):** Etraftaki sesler, görsel değişiklikler veya patlayan emojiler nöral ağın "Input" katmanını sürekli uyarır.
3. **Agentic Prioritization (Özerk Karar Alma):** Bir dürtü çok yükseldiğinde (örn: aşırı sıkılma), NPC inisiyatif alarak rutinini bozar ve gidip birine "Merhaba" der. Oyuncunun veya oyun motorunun ona emir vermesine gerek yoktur.
4. **Fuzzy Thresholds & Personality:** Dürtülerin harekete geçme eşikleri sabit değildir. Kişilik matrisine (Personality) göre değişir. İçe dönük biri için "Sosyalleşme" eşiği çok yüksektir, dışa dönük biri için düşüktür.
5. **Mood Distortion (Ruh Hali Çarpıtması):** Beyin durumu (Cognitive State), algıyı değiştirir. Bunalmış bir NPC, normalde nötr yaklaştığı olaylara veya insanlara düşmanca tepkiler verebilir.

## 2. Ağ Mimarisi (Neural Topology)

### Katman 1: Sensörler ve İçgüdüler (Input Layer)
Beyne giren veriler ikiye ayrılır: İçsel (Instincts) ve Dışsal (Sensors).

**A. İçgüdüsel Nöronlar (Internal Drives - Metabolizma)**
Zamanla kendi kendine artan veya azalan biyolojik/psikolojik sayaçlar:
- `Drive_Boredom (Can Sıkıntısı):` Kimseyle etkileşime girilmediğinde artar.
- `Drive_Curiosity (Merak):` Aynı yerde çok uzun süre kalındığında artar.
- `Drive_Energy (Enerji):` Zamanla tükenir. 

**B. Dışsal Duyu Nöronları (External Sensors)**
- `Sensor_Visual:` Çevrede kimler var? (Dostlar, düşmanlar, oyuncu).
- `Sensor_Auditory:` Yakınlarda bir olay oldu mu? (Emoji dalgaları).
- `Sensor_WorldState:` Yağmur mu yağıyor? Akşam mı oldu?

---

### Katman 2: Bilişsel ve Kişilik Katmanı (Hidden Layer)
Girdilerin işlendiği, filtrelendiği ve NPC'ye özgü hale getirildiği yer.

- **Kişilik Filtresi (Personality Weights):** 
  - İçe dönük bir NPC'de `Drive_Boredom` çok yavaş artar.
- **Ruh Hali Çarpıtması (Mood Distortion):**
  - NPC'nin genel ruh hali, dışarıyı nasıl algıladığını değiştirir.
- **Yaratıcılık / Kaos Nöronu (The Whim Spark):**
  - Kusursuz mantığı bozan "Kıvılcım". Ara sıra rastgele ateşlenir ve mantıksız ama şaşırtıcı eylemlere neden olur.

---

### Katman 3: Motor ve Duygu Çıktıları (Output Layer)
En yüksek aktivasyona sahip nöronlar fiziksel eyleme dönüşür.

**A. Ajan Eylemleri (Agentic Actions):**
- `Action_Socialize (Hi de)` -> Sıkıntı içgüdüsü tavan yaptığında tetiklenir.
- `Action_Wander` -> Merak içgüdüsü tetiklediğinde çalışır.
- `Action_Rest` -> Enerji bittiğinde en yakın banka gider.

**B. Fiziksel Duygu Emisyonu (Systemic Emojis):**
- Emojiler UI değildir. Havaya fırlatılan bir duygu, etrafa fiziksel bir dalga (AoE) yayar ve diğer NPC'lerin beyinlerini doğrudan manipüle eder (Duygu Bulaşıcılığı).

---

## 3. FSM (State Machine) Eliminasyon Tablosu
Mevcut mimarideki `NPCState` Enum yapısı Neural Brain vizyonuna aykırıdır (Dual Brain Desynchronization). Bu yüzden eski state yapısı tamamen silinip, görevleri Neural Network çıktılarına (Actions) devredilecektir:

| Eski FSM State | Yeni Neural Action Çıktısı (Sorumlu Sınıf) | Değişim Detayı |
| --- | --- | --- |
| `NPCState.Reading` | `ReadNewsAction.cs` | Rotasyon ayarı ve `ProcessReadNews()` artık doğrudan bu aksiyonun `Execute()` metodunda çalışacak. |
| `NPCState.Fleeing` | `FleeAction.cs` / `FleePlayerAction.cs` | `_gestures.PlayFear()` fonksiyonu doğrudan bu eylem başlayınca tetiklenecek. |
| `NPCState.Sitting` | `ChillAloneAction.cs` | Slouching/Boyut ayarları bu aksiyona devredilecek. |
| `NPCState.Wandering` | `WanderAction.cs` | Durum bildirimi iptal edilecek, UtilityBrain eylemi yürütecek. |
| `NPCState.Idle` | (Yok - Zaten baz aktivasyon) | Action bitimlerinde Idle'a dönme kodu silinecek, beyin zaten boşta kalıp yeni eylem seçecek. |
| `NPCState.Hugging` | `SocializeAction.cs` vb. | `_gestures.PlayAffection()` ilgili sosyal eylem içine taşınacak. |
