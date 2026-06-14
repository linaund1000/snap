# NPC Prefab Değiştirme Rehberi (Recipe)

Bu klasör, oyununuzdaki NPC'lerin Unity Prefab (Şablon) dosyalarını içerir. Artık NPC'ler kodla yoktan var edilmek yerine, bu klasördeki `.prefab` dosyaları sahneye çağrılarak (Instantiate) oluşturulur.

## Yeni Bir 3D Asset (Karakter) Eklerken Dikkat Edilmesi Gerekenler

Hazır aldığınız veya çizdirdiğiniz bir 3D karakteri bu prefablere entegre etmek için şu adımları izleyin:

### Adım 1: Prefab'i Açın
1. Unity'nin alt penceresindeki (Project) bu klasörde bulunan bir NPC Prefab'ine (örneğin `NPC_Leo.prefab`) çift tıklayarak Prefab Editörüne girin.

### Adım 2: Yeni 3D Modelinizi Ekleyin
1. Kendi 3D model dosyanızı (`.fbx`, `.obj` vb.) Prefab'in hiyerarşisinde **Ana Objenin (Root)** içine sürükleyip bırakın.
2. Ana obje (üzerinde `NPCController`, `PhotoSubject` gibi scriptlerin olduğu obje) yerinde kalmalıdır. Scriptleri **kesinlikle** silmeyin.

### Adım 3: Eski Yer Tutucuları (Placeholder) Silin Veya Gizleyin
1. Prefab içindeki eski `Body` (Kapsül) ve `Head` (Küre) objelerini seçip silebilirsiniz (Delete) veya Inspector'dan sol üstteki tiki kaldırarak gizleyebilirsiniz.
2. Artık onların yerine sizin eklediğiniz 3D model görünecektir.

### Kritik Kurallar (Bunlara Kesinlikle Dikkat Edin!)
- **Ana Obje (Root) Scriptleri:** `NPCController`, `ObstacleAvoidance`, `PhotoSubject` gibi tüm mantık (logic) scriptleri en üst objede durmalıdır. Yeni 3D modelinize bu scriptleri tekrar eklemeye çalışmayın, sadece 3D modeli görsel olarak alt obje (child) yapın.
- **NavMeshObstacle / Collider:** Yeni modelinizin boyutları farklıysa, ana objedeki `NavMeshObstacle` veya `CapsuleCollider` bileşenlerinin "Center" (Merkez) ve "Radius/Size" (Yarıçap/Boyut) değerlerini yeni modelinize tam oturacak şekilde Inspector'dan göz kararı ayarlayın. Aksi takdirde NPC'ler duvarların içinden geçebilir veya boşluğa takılabilir.
- **Animasyonlar (Animator):** Eğer yeni modelinizin yürüme/durma animasyonları varsa, `Animator` bileşeni sizin eklediğiniz 3D modelin üzerinde olacaktır. İleride animasyonları koddan tetiklemek isterseniz (örneğin `animator.SetTrigger("Walk")`), kodu bu yeni Animator'ı bulacak şekilde güncellememiz gerekecek.

Bu adımları izleyerek dilediğiniz kadar yeni NPC'yi kodları bozmadan oyuna dahil edebilirsiniz!
