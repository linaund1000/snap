# System & UI Automata Theory (Moore & Mealy Machines)

Bu doküman, SNAP projesinin Kullanıcı Arayüzü (UI) ve Temel Oyun Sistemleri (System) arasındaki girdi/çıktı (Input/Output) yönetimini **Otomata Teorisi** üzerinden resmileştirir. Oyunun "State" karmasasına (bug'lar, kilitlenmeler, UI çakışmaları) kurban gitmemesi için sistem hem **Moore Makinesi** hem de **Mealy Makinesi** hibriti olarak tasarlanmıştır.

---

## 1. Matematiksel Model (The Automaton Model)

Sistem bir FSM tuple'ı olarak tanımlanır: **M = (Q, Σ, Λ, Ω, δ, λ, ω)**

- **Q (States):** Sistemin bulunabileceği sonlu durumlar kümesi.
- **Σ (Input Alphabet):** Kullanıcıdan veya sistemden gelen tetikleyiciler (Triggers).
- **Λ (Moore Outputs):** Sadece bulunulan State'e bağlı olan fiziksel çıktılar (Zamanın akması, farenin durumu).
- **Ω (Mealy Outputs):** State geçişi (Transition) sırasında anlık gerçekleşen çıktılar (Fotoğraf çekme, ses çalma).
- **δ (State Transition Function):** `δ: Q × Σ → Q` (Hangi durumda hangi girdi gelirse nereye geçilir?)
- **λ (Moore Output Function):** `λ: Q → Λ` (Şu anki duruma göre sistem fiziksel kuralları nedir?)
- **ω (Mealy Output Function):** `ω: Q × Σ → Ω` (Girdi alındığı an ne tepki verilir?)

---

## 2. States (Q) & Moore Output Function (λ: Q → Λ)

Moore makinesinin doğası gereği, sistem bir State'e oturduğu an çevresel faktörler (Zaman, Kamera, Input kilitleri) anında uygulanır.

| State (q ∈ Q) | λ: Time Scale | λ: Cursor State | λ: Player Move | λ: Viewfinder | λ: Active Canvas |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `q_IMMERSION` | 1.0 (Akar) | Locked (Gizli) | Serbest | Gizli | Sadece HUD |
| `q_FRAMING` | 1.0 (Akar) | Locked (Gizli) | Yarı Hız | Açık | HUD + Crosshair |
| `q_UI_JOURNAL` | 1.0 (Akar) | Unlocked (Açık)| Bloklu | Gizli | JournalUI |
| `q_UI_GALLERY` | 1.0 (Akar) | Unlocked (Açık)| Bloklu | Gizli | PhotoGalleryUI |
| `q_UI_BOARD` | 1.0 (Akar) | Unlocked (Açık)| Bloklu | Gizli | NewspaperBoardUI |
| `q_PAUSED` | **0.0 (Durur)** | Unlocked (Açık)| Bloklu | Gizli | SettingsUI |

---

## 3. Input Alphabet (Σ)

Kullanıcının sisteme gönderebileceği anlık sinyaller:
- `σ_Hold_C` / `σ_Release_C` (Kamerayı aç/kapat)
- `σ_Press_J` (Journal)
- `σ_Press_G` (Gallery)
- `σ_Press_B` (Board)
- `σ_Press_Esc` (Settings / Geri)
- `σ_Click` (Aksiyon/Fotoğraf)

---

## 4. State Transition (δ) & Mealy Output (ω) Functions

Bu tablo, sistemin hangi durumda hangi sinyali aldığında hem nereye geçeceğini (δ) hem de anlık olarak ne tür bir eylem (ω) üreteceğini belirler. Mealy makinesi özellikleri burada parlar (Örn: `q_FRAMING` durumunda `σ_Click` gelmesi state'i değiştirmez ama anlık fotoğraf çeker).

### 4.1. Core System & Camera (Mealy & Moore Hibrit)
| Current State (q) | Input (σ) | Next State δ(q, σ) | Mealy Output ω(q, σ) (Anlık Aksiyon) |
| :--- | :--- | :--- | :--- |
| `q_IMMERSION` | `σ_Hold_C` | `q_FRAMING` | Kamera lensi açılma sesi çal (SFX). |
| `q_FRAMING` | `σ_Release_C`| `q_IMMERSION` | Lens kapanma sesi çal (SFX). |
| `q_FRAMING` | `σ_Click` | `q_FRAMING` | **[Mealy Aktif]** Flaş patlat, Fotoğrafı Diske Kaydet. State değişmez. |
| `q_FRAMING` | `σ_Press_J` | `q_FRAMING` | **[Mealy Aktif]** "Hata/Kilitli" sesi (Buzzer) çal. Geçiş REDDEDİLDİ. |

### 4.2. UI Cross-Override & Interrupts
Bir UI açıkken başka bir Input geldiğinde sistemin çökmemesi için tasarlanmış geçişler.

| Current State (q) | Input (σ) | Next State δ(q, σ) | Mealy Output ω(q, σ) (Anlık Aksiyon) |
| :--- | :--- | :--- | :--- |
| `q_UI_*` (Tümü)| `σ_Hold_C` | `q_FRAMING` | **Override:** Hızlı refleks! Tüm menüleri Dispose et, direkt kameraya geç. |
| `q_UI_JOURNAL`| `σ_Press_G` | `q_UI_GALLERY` | **Cross-Fade:** Journal'ı gizle, Gallery'yi öne al. |
| `q_UI_*` (Tümü)| `σ_Press_Esc`| `q_IMMERSION` | Menü kapanış sesi (SFX). |

### 4.3. Mutlak Kesinti (The Absolute Interrupt)
| Current State (q) | Input (σ) | Next State δ(q, σ) | Mealy Output ω(q, σ) (Anlık Aksiyon) |
| :--- | :--- | :--- | :--- |
| Any `q` | `σ_Press_Esc`| `q_PAUSED` | Oyunu dondur. UI dışı tüm input listener'ları uyut. |
| `q_PAUSED` | Any (C, J, G)| `q_PAUSED` | Hiçbir Input kabul edilmez. Tamamen izole. |

---

## 5. Sistem Referans Haritası (Where to Implement)

Bu Automata Teorisi, oyun motorundaki şu dosyalarda fiziksel koda dönüşmelidir:

1. **`GlobalInputListener.cs`**: Bu sınıf, **δ (State Transition Function)** görevini üstlenmelidir. Eski `if-else` mantığı silinmeli, yukarıdaki State (Q) kontrol edilerek sadece izin verilen Σ girdileri işlenmelidir.
2. **`UIManager.cs`**: Bu sınıf, **λ (Moore Output Function)** görevini üstlenmelidir. Sistem yeni bir duruma geçtiğinde TimeScale, CursorLock ve aktif Canvas yönetimini sadece bu sınıf yapmalıdır.
3. **UseCases (Örn: `PhotoCaptureUseCase`)**: Bu sınıflar **ω (Mealy Output Function)** görevini üstlenmelidir. State değişmeden anlık aksiyon gerektiren durumları tetikler.
