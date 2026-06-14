# Global UI Input Moore Machine & State Transition Matrix

Oyunun karmaşık kullanıcı arayüzünü (UX) hatasız yönetmek için sistem, klasik bir **Moore Makinesi** olarak tasarlanmıştır. 
Moore makinesinde çıktılar (Oyunun durması, farenin kilitlenmesi, hangi ekranın açık olduğu) **yalnızca bulunulan State (Durum)** tarafından belirlenir. Bu, geçişler (transition) sırasında yaşanan state uyuşmazlıklarını ve bug'ları sıfıra indirir. (Örn: Menü kapanır ama mouse kilitli kalır bug'ı yaşanmaz).

## 1. System States & Moore Output Function (λ: Q → Λ)

Sistem bir State'e girdiğinde, aşağıdaki "Output" matrisi anında uygulanır. Her State, oyunun fiziksel ve görsel kurallarını dikte eder.

| State (Q) | Output (Λ): Time Scale | Output (Λ): Mouse State | Output (Λ): Player Movement | Output (Λ): Active UI Canvas |
| :--- | :--- | :--- | :--- | :--- |
| **`IMMERSION_FREE`** | 1.0 (Normal) | **Locked** (Invisible) | Allowed (100% Speed) | HUD (Target Inspector) |
| **`IMMERSION_FRAMING`** | 1.0 (Normal) | **Locked** (Invisible) | Allowed (50% Speed) | HUD + Viewfinder Crosshair |
| **`UI_JOURNAL_MATRIX`**| 1.0 (Real-time)| **Unlocked** (Visible)| **Blocked** | JournalUI (Matrix Panel) |
| **`UI_JOURNAL_FEED`** | 1.0 (Real-time)| **Unlocked** (Visible)| **Blocked** | JournalUI (Feed Panel) |
| **`UI_JOURNAL_FOCUS`**| 1.0 (Real-time)| **Unlocked** (Visible)| **Blocked** | JournalUI (Focus Panel) |
| **`UI_GALLERY`** | 1.0 (Real-time)| **Unlocked** (Visible)| **Blocked** | PhotoGalleryUI |
| **`UI_BOARD`** | 1.0 (Real-time)| **Unlocked** (Visible)| **Blocked** | NewspaperBoardUI |
| **`UI_SETTINGS`** | **0.0 (Paused)** | **Unlocked** (Visible)| **Blocked** | Settings/Pause Menu |

*Not: Menüler açıkken oyun akmaya devam eder (Time=1.0). Bu, "Live Ticker" ve yaşayan dünya felsefesinin kalbidir. Sadece `UI_SETTINGS` (Pause Menu) oyunu dondurur.*

---

## 2. Input Triggers (Σ)

- `Σ_C_Hold`: Kamera tuşuna basılı tutma
- `Σ_C_Release`: Kamera tuşunu bırakma
- `Σ_J_Press`: Journal tuşu
- `Σ_G_Press`: Gallery tuşu
- `Σ_B_Press`: Board tuşu
- `Σ_Esc_Press`: Escape tuşu
- `Σ_LClick`: Sol Tık

---

## 3. The State Transition Function Matrix (δ: Q × Σ → Q)

Moore makinesinin çekirdeği: Hangi durumdayken hangi tuşa basılırsa nereye geçilir? 
Aşağıdaki matris, tespit edilen **eksikleri ve mantık hatalarını (flaws)** kapatacak şekilde tasarlanmıştır.

### 3.1. Kamera ve Vizör Geçişleri (Framing)
*Kamera açıldığında diğer her şey durmalı veya bloklanmalıdır.*

| Current State (Q) | Input Trigger (Σ) | Next State (Q') | UX Nedeni & Karar |
| :--- | :--- | :--- | :--- |
| `IMMERSION_FREE` | `Σ_C_Hold` | `IMMERSION_FRAMING` | Doğal vizör açılışı. |
| `IMMERSION_FRAMING` | `Σ_C_Release` | `IMMERSION_FREE` | Doğal vizör kapanışı. |
| `IMMERSION_FRAMING` | `Σ_LClick` | `IMMERSION_FRAMING` | Fotoğraf çekilir (State değişmez, arka planda UseCase çalışır). |
| `IMMERSION_FRAMING` | `Σ_J_Press` / `Σ_G_Press` / `Σ_B_Press` | **`IMMERSION_FRAMING` (BLOCKED)** | Vizörden bakarken günlük AÇILAMAZ. Input tamamen yoksayılır. |
| `IMMERSION_FRAMING` | `Σ_Esc_Press` | `IMMERSION_FREE` | Esc'ye basmak vizörü zorla kapattırıp ana oyuna döndürür. |

### 3.2. Menü İçi Override (Sıvı/Akıcı UX)
*Menüler açıldığında, oyuncunun menüler arası geçişi pürüzsüz olmalıdır.*

| Current State (Q) | Input Trigger (Σ) | Next State (Q') | UX Nedeni & Karar |
| :--- | :--- | :--- | :--- |
| `UI_*` (Tümü hariç Settings)| `Σ_C_Hold` | `IMMERSION_FRAMING` | **OVERRIDE:** Oyuncu menüdeyken ilginç bir an görürse, C'ye basılı tutarak menüleri yırtıp direkt vizöre geçer. |
| `UI_JOURNAL_*` | `Σ_G_Press` | `UI_GALLERY` | **CROSS-OVERRIDE:** Journal açıkken G'ye basılırsa, Journal gizlenir Gallery açılır. |
| `UI_GALLERY` | `Σ_J_Press` | `UI_JOURNAL_MATRIX`| **CROSS-OVERRIDE:** Galeri açıkken J'ye basılırsa Galeri kapanır Journal açılır. |
| `UI_BOARD` | `Σ_J_Press` | `UI_JOURNAL_MATRIX`| **CROSS-OVERRIDE:** Board açıkken J'ye basılırsa Journal açılır. |
| `UI_*` (Herhangi Menü)| `Σ_Esc_Press` | `IMMERSION_FREE` | Esc her zaman menüleri kapatıp oyuna (Immersion) döndürür. |

### 3.3. Journal Kendi İç FSM Döngüsü (J -> J -> J)
*Sadece Journal'ın iç sekmelerindeki döngü.*

| Current State (Q) | Input Trigger (Σ) | Condition | Next State (Q') |
| :--- | :--- | :--- | :--- |
| `IMMERSION_FREE` | `Σ_J_Press` | No Target | `UI_JOURNAL_MATRIX` |
| `IMMERSION_FREE` | `Σ_J_Press` | Has Target | `UI_JOURNAL_FOCUS` |
| `UI_JOURNAL_MATRIX`| `Σ_J_Press` | None | `UI_JOURNAL_FEED` |
| `UI_JOURNAL_FEED` | `Σ_J_Press` | None | `IMMERSION_FREE` (Kapanır) |
| `UI_JOURNAL_FOCUS` | `Σ_J_Press` | None | `IMMERSION_FREE` (Kapanır) |

### 3.4. Settings / Pause State (Mutlak Durdurma)
*Settings (Ayarlar) menüsündeyken yaşanan mantık hatalarının düzeltilmesi.*

| Current State (Q) | Input Trigger (Σ) | Next State (Q') | UX Nedeni & Karar |
| :--- | :--- | :--- | :--- |
| `IMMERSION_FREE` | `Σ_Esc_Press` | `UI_SETTINGS` | Oyunu durdurur ve Ayarlar menüsünü açar. |
| `UI_SETTINGS` | `Σ_Esc_Press` | `IMMERSION_FREE` | Ayarlardan çıkar, oyunu devam ettirir. |
| `UI_SETTINGS` | **Tüm Diğer Tuşlar** (`C`, `J`, `G`, `B`) | **`UI_SETTINGS` (BLOCKED)** | **CRITICAL FIX:** Oyun PAUSE durumundayken hiçbir menü veya kamera kısayolu çalışmamalıdır. Input yoksayılır. |

---

## 4. Endless Scrolling vs Deductive Ledger (UX Separation)

To prevent cognitive overload and memory bloat, we separate **Live Observation** from **Deductive Memory**.

1. **The Emoji / Live Ticker (Observation Layer):**
   - **Data:** Every single wave, angry face, handshake, and micro-interaction.
   - **UX Type:** Ephemeral, endless scrolling UI (e.g., bottom-left corner ticker).
   - **Memory Profile:** Holds max 50 strings. Items older than 30 seconds fade out and are deleted from memory. It is purely for "game feel".

2. **The Deductive Journal (Ledger Layer):**
   - **Data:** `MemoryEvent` structs logged by the `NPCMemoryStream`. Major turning points: Hostile Confrontations, Group Formations, Deep Relationships.
   - **UX Type:** Accessed via `J`. Persistent, filtered data for solving the "Social Network Matrix".
   - **Memory Profile:** Serialized to disk via `NPCSaveManager`.
