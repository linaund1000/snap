using UnityEngine;
using UnityEditor;
using GPOyun.NPC;
using GPOyun.Core;
using GPOyun.Environment;

namespace GPOyun.EditorScripts
{
    public class NPCPrefabGenerator : EditorWindow
    {
        [MenuItem("GPOyun/Generate NPC Prefabs")]
        public static void GeneratePrefabs()
        {
            string folderPath = "Assets/_Project/Resources/NPCs";

            // Klasörün var olduğundan emin ol
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/_Project/Resources", "NPCs");
            }

            Color[] presetColors = new Color[] {
                VisualUtils.Terracotta,
                VisualUtils.CobaltBlue,
                VisualUtils.PineGreen,
                VisualUtils.WoodBrown,
                VisualUtils.StuccoWhite,
                new Color(1f, 0.55f, 0.35f), // Premium Peach
                new Color(1f, 0.82f, 0.15f), // Sunshine Yellow
                new Color(0.85f, 0.45f, 0.95f), // Soft Lavender
                new Color(0.15f, 0.75f, 0.75f), // Cozy Teal
                new Color(1f, 0.35f, 0.35f)  // Crimson Coral
            };

            string[] names = new string[] {
                "Leo", "Zoe", "Max", "Mia", "Eli", "Ava", "Kai", "Ivy", "Rex", "Sol"
            };

            for (int id = 0; id < 10; id++)
            {
                string npcName = names[id % names.Length];
                
                // Geçici objeyi oluştur
                GameObject npcGroup = new GameObject($"NPC_{id}_{npcName}");
                
                Color col = presetColors[id % presetColors.Length];

                // Gövde (Placeholder)
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Body";
                body.transform.SetParent(npcGroup.transform);
                body.transform.localPosition = Vector3.zero;
                VisualUtils.ApplyAesthetic(body, col);

                // Kafa (Placeholder)
                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.name = "Head";
                head.transform.SetParent(npcGroup.transform);
                head.transform.localPosition = new Vector3(0, 1.2f, 0);
                head.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                VisualUtils.ApplyAesthetic(head, col);
                
                // Gerekli Scriptleri Ekle
                NPCController controller = npcGroup.AddComponent<NPCController>();
                controller.NpcId = id;
                controller.NpcName = npcName;
                // boardPosition prefab içinde null kalacak, spawn anında set edilmeli.
                
                npcGroup.AddComponent<NPCVisualHelper>();
                npcGroup.AddComponent<ObstacleAvoidance>();
                
                var photoSub = npcGroup.AddComponent<PhotoSubject>();
                photoSub.SubjectName = npcName;
                photoSub.PrimaryCategory = GPOyun.Newspaper.NewsCategory.Local;

                // Prefab olarak kaydet
                string prefabPath = $"{folderPath}/NPC_{id}_{npcName}.prefab";
                PrefabUtility.SaveAsPrefabAsset(npcGroup, prefabPath);

                // Geçici objeyi sahneden sil
                DestroyImmediate(npcGroup);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[GPOyun] 10 Adet NPC Prefab'i başarıyla '{folderPath}' klasörüne oluşturuldu!");
        }
    }
}
