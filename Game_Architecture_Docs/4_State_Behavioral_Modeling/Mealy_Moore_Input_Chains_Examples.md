# Mealy / Moore Machine Input Chain Examples

Bu belge, klavye ve fare hareketlerinin teorik makinelerde (HFSM) nasıl adım adım çözüldüğünü (First Principle Thinking) ispatlamak için hazırlanmıştır. Kullanıcının yapabileceği en kaotik kombinasyonların bile sistemde karşılığı vardır.

## Senaryo 1: Üç Kere J'ye Basmak (J -> J -> J)
*Beklenti: J (Matrix Aç) -> J (Feed Aç) -> J (Kapat).*

1. **Adım 1 (J Basıldı):**
   - **Current State:** `Immersion`
   - **Input:** `σ_J_Press`
   - **Mealy Output (InputRouter):** `Emit(Req_UnlockCursor, Req_BlockMove)`
   - **Transition:** `Immersion` -> `UI_Active`
   - **Sub-Machine (Journal):** `Closed` -> `Open_Matrix`
   - **Moore Output (UIManager):** Cursor = Unlocked, Time = Akar.

2. **Adım 2 (J Tekrar Basıldı):**
   - **Current State:** `UI_Active`
   - **Input:** `σ_J_Press`
   - **Sub-Machine (Journal):** `Open_Matrix` -> `Open_Feed` (Journal kendi içinde state değiştirir, Global state hala `UI_Active`).

3. **Adım 3 (J Tekrar Basıldı):**
   - **Current State:** `UI_Active`
   - **Input:** `σ_J_Press`
   - **Sub-Machine (Journal):** `Open_Feed` -> `Closed` (Journal kapanma sinyali üretir).
   - **Mealy Output (Journal):** `Emit(Journal_Closed)`
   - **Transition (Global):** `UI_Active` -> `Immersion`
   - **Moore Output (UIManager):** Cursor = Locked, Time = Akar. (Ekran kapanır).

---

## Senaryo 2: J Açıkken Esc'ye Basmak (J -> Esc / J -> J -> Esc)
*Beklenti: Menü ne kadar derinde olursa olsun Esc anında her şeyi kapatıp oyuna döndürmeli.*

1. **Adım 1 (J veya JJ yapıldı):** Sistem `UI_Active` durumundadır. Journal alt makinesi `Open_Matrix` veya `Open_Feed` durumundadır.
2. **Adım 2 (Esc Basıldı):**
   - **Input:** `σ_Esc_Press`
   - **Mealy Output (InputRouter):** `Emit(Global_ForceClose)`
   - **Sub-Machine Tepkileri:** `StateBus` üzerinden `Global_ForceClose` sinyalini alan TÜM alt makineler (Journal, Gallery, Board) anında `Closed` state'ine zorla geçirilir.
   - **Transition:** `UI_Active` -> `Immersion`
   - **Moore Output:** Fare kilitlenir, hareket serbest bırakılır. Ekran anında temizlenir.

---

## Senaryo 3: Hızlı Refleks (J -> C)
*Beklenti: Günlük okunurken acil bir durum oldu, vizör (C) anında açılmalı.*

1. **Adım 1 (J Basıldı):** Sistem `UI_Active` durumunda, Journal açık.
2. **Adım 2 (C Basılı Tutuldu):**
   - **Input:** `σ_Hold_C`
   - **Mealy Output (InputRouter):** `Emit(Global_ForceClose, Req_LockCursor, Req_HideHUD)`
   - **Sub-Machine Tepkileri:** Journal `Global_ForceClose` alıp anında kapanır. HUD kapanır. Cursor kilitlenir.
   - **Transition:** `UI_Active` -> `Framing`
   - **Sub-Machine (Camera):** `Closed` -> `Open_Framing`
   - **Moore Output:** Oyuncu hızı yarıya düşer, vizör ekrana gelir. (Ekran kapandı, capture açıldı).

---

## Twitch-Like Live Ticker Tasarımı (First Principles)
Oyun dünyasında dönen karmaşanın oyuncuya akması için sol alt köşede "Endless Scrolling" bir UI.
- **Tasarım Kuralları:** Yazılar küçük olmalı (10pt-12pt). Ekranda en fazla 5-6 satır durmalı. Eski yazılar 15-20 saniye sonra yavaşça silinmeli (Fade Out).
- **Veri Formatı:** `[Tarih/Saat] | [Emoji/Eylem] | [İsim 1] -> [İsim 2] | [Sonuç/İlişki]`
- **Örnek 1 (Küçük Olay):** `[08:15] 💬 Ivy gülümsedi -> Rex.`
- **Örnek 2 (Kritik Olay):** `[09:30] 💢 Mia öfkelendi -> Kaan. (İlişki: -15)`
- **Örnek 3 (Grup Olayı):** `[12:00] 🤝 Ivy, Rex ve Mia yeni bir grup kurdu.`

Bu Live Ticker, Journal'daki "Ledger" (defter) kısmından bağımsız çalışır. Sadece atmosfer ve "yaşayan dünya" (immersion) içindir. Diske kaydedilmez (Memory friendly).
