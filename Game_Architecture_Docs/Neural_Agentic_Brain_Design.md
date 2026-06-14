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

---

## 4. Dinamik UX ve Sistem Geri Bildirimi (Dynamic UX & System Feedback)
Oyuncu ve NPC'ler arasındaki etkileşim sadece mekanik bir düzeyde kalmamalı, karşılıklı bir diyaloğa (insani ilişki) dönüşmelidir. Bunun için şu dinamik UX prensipleri uygulanacaktır:

### 4.1. Fotoğraf Yorumlama (Photo Captioning & NPC Influence)
- **Mekanik:** Oyuncu bir fotoğraf çektiğinde veya gazeteyi basmadan önce o fotoğrafın altına bir **"String Yorum (Caption)"** yazabilir.
- **Etki (Feedback):** Yazılan bu yorum sadece UI üzerinde kalmaz; gazete yayınlandığında veya anlık olarak NPC'lerin `Input Layer`'ına bir duygu değişkeni olarak pompalanır. NPC'ler haberin içeriğinden çok, oyuncunun eklediği yorumun "tonuna" (Agresif, komik, kışkırtıcı) göre tepki verir. Oyuncu kendi kelimeleriyle dünyayı manipüle etmiş olur.

### 4.2. Göz Kontağı ve İlgiyi Çalma (Physical Attention Grabbing)
- **Mekanik:** Oyuncunun varlığı pasif bir gözlemci olmamalıdır. Eğer bir NPC oyuncuya çok sinirlenmişse veya ona kritik bir şey anlatmak istiyorsa, oyuncunun odağını **fiziksel olarak üzerine çekebilmelidir.**
- **Etki (Feedback):** NPC oyuncunun yanına geldiğinde kamerayı (Player Camera) yumuşak ama zorlayıcı bir interpolasyonla kendine çevirir. "Bana bak, seninle konuşuyorum!" hissi yaratılır. Sesli diyalog olmasa bile, vücut dili ve UI/Camera Force ile aşırı güçlü bir etkileşim kurulur.

### 4.3. Expresif Sistem Geri Bildirimleri (Expressive UI & System Cues)
- Sadece emojiler değil, ekran titremeleri (Screen Shake), UI çerçevesinin kızarması (Anger Vignette) veya anlık işitsel efektler (kulak çınlaması) sayesinde oyun dünyası (System), NPC'nin iç dünyasını oyuncuya organik bir şekilde yansıtır. Geri bildirimler her zaman iki yönlü olmalıdır.
