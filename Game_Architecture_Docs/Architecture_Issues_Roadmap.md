# Game Architecture Issues & Refactoring Roadmap

Bu belge, oyunun mimari sorunlarını, UX (Kullanıcı Deneyimi) eksiklerini ve gelecekteki refactor (yeniden yapılandırma) planlarını barındırır. Yeni FSM ve Live Feed tasarımı sonrası güncellenmiştir.

## Aktif Görev Listesi (Suana Kadar Yapilmayanlar & Yeni Eklenenler)

PRIORITY | ISSUE_NAME | DESCRIPTION | REFERENCED_FILES | ACTION_PLAN
---|---|---|---|---
🚨 **CRITICAL** | Global_UI_Input_FSM | Girdiler (Input) hala spagetti if-else bloklarıyla çalışıyor. Kamera açılırken menülerin üst üste binmesi gibi UX hatalarına açık. | `GlobalInputListener.cs`, `UIManager.cs` | Yeni tasarlanan `Global_UI_State_Transition_Matrix.md` anayasasına göre UIManager ve GlobalInputListener'ı "State Machine" yapısına geçir. `Override` ve `Block` mantıklarını kodla.
🚨 **CRITICAL** | Endless_Live_Ticker_UI | `GlobalEventLogger` verileri sadece Journal açılınca gözüküyor. Emojiler ve küçük olayların Twitch Chat gibi kenarda akması lazım. | `HUDManager.cs`, `GlobalEventLogger.cs` | Ekranın sol alt köşesine kaybolan/akan (fading scroll) bir metin kutusu ekle. Dünya yaşıyor hissini canlı tut.
High | Viewfinder_Composition_Feedback | Oyuncu kamerayla bir NPC'ye bakarken fotoğrafın "iyi/ilginç" olup olmayacağını vizörden (crosshair) anlayamıyor. | `PhotoScorer.cs`, `ViewfinderManager.cs` | Vizörün rengini (Örn: Yeşile dönme) hedefin InterestLevel skoruna göre anlık değiştir (Raycast veya Frustum intersection).
High | Unbounded_Memory_Streams | NPC logları `List<MemoryEvent>` içine sonsuza kadar yazılıyor. 3 günlük simülasyon sonunda RAM sızıntısı (Memory Leak) yapabilir. | `NPCMemoryStream.cs`, `GlobalEventLogger.cs` | Listeleri Ring Buffer mantığına çevir. Max 50/100 eleman tutulsun. Eski olaylar silinsin.
High | Missing_Audio_Cues | Oyunda sistemik geri bildirimlerin (Fotoğraf çekme, emoji atma, ilişki kırılması) hiç sesi yok. | Yeni Sistem | `EventBus` veya Observer üzerinden çalışacak bir `AudioManager` yarat ve uzamsal (spatial) sesleri bağla.
Med | Editorial_Agency_Validation | Gazete çıkarma ekranı var (`EditorialUI`) ama oyuncu bu fotoğrafları çöpe atabiliyor mu? Kategori seçebiliyor mu? Bağlantıları zayıf. | `EditorialUI.cs`, `NewspaperManager.cs` | Fotoğrafların manuel kategorize edilmesi (Scandal, Heartwarming) sisteminin test edilmesi ve eksiklerin giderilmesi.
Low | Procedural_Gen_Stagnation | Kasaba kodla üretilen küplerden ibaret (`TownSquareBuilder.cs`). Seviye tasarımcıları sahnede çalışamıyor. | `TownSquareBuilder.cs` | Primitive objeler yerine gerçek Prefab (Ev, Ağaç, Bank) Instantiation sistemine geç.

---

## 🛑 Daha Önce Tamamlanan Kritik İşler (Geçmiş Zaferler)
- `[DONE]` **Dual_Brain_Desynchronization:** Enum state'ler yıkıldı, tam teşekküllü UtilityAI ve NPCBrain sistemi kuruldu.
- `[DONE]` **Missing_System_Feedback:** Hedefe bakıldığında bilgileri gösteren `TargetInspectorUI` yazıldı.
- `[DONE]` **Matrix_Garbage_Collection:** RelationshipMatrix string birleştirmelerinden kurtarılıp performanslı Struct'lara çevrildi.
- `[DONE]` **Synchronous_IO_Stutter:** Fotoğraf çekimi (EncodeToPNG) asenkron Task'a taşındı, anlık takılmalar önlendi.
- `[DONE]` **Predictable_Utility:** Yapay zekaya "Whim" (Kaos) çarpanı eklendi, robotik hareketler kırıldı.
- `[DONE]` **Static_Emojis:** Emojiler görsel olmaktan çıkıp (AoE SphereCast ile) fiziksel etki bırakan bir "Büyü/Etki"ye (`ProcessEmojiStimulus`) dönüştürüldü.
- `[DONE]` **Multi-State_Journal:** Journal UI artık tüm ekranı kör etmeyen, state machine (Matrix -> Feed -> Focus) ile çalışan akıcı bir FSM tabanlı sisteme geçirildi.
