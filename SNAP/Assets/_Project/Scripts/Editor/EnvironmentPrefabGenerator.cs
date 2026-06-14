using UnityEngine;
using UnityEditor;
using GPOyun.Core;
using GPOyun.NPC;
using GPOyun.Environment;

namespace GPOyun.EditorScripts
{
    public class EnvironmentPrefabGenerator : EditorWindow
    {
        [MenuItem("GPOyun/Generate Environment Prefabs")]
        public static void GeneratePrefabs()
        {
            string folderPath = "Assets/_Project/Resources/Environment";

            if (!AssetDatabase.IsValidFolder("Assets/_Project/Resources"))
                AssetDatabase.CreateFolder("Assets/_Project", "Resources");
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/_Project/Resources", "Environment");

            // 1. Houses
            CreateHousePrefab(folderPath, "House_Bakery", new Vector3(8, 6, 8), VisualUtils.StuccoWhite, VisualUtils.Terracotta);
            CreateHousePrefab(folderPath, "House_Cafe", new Vector3(7, 5, 7), VisualUtils.StuccoWhite, VisualUtils.CobaltBlue);
            CreateHousePrefab(folderPath, "House_Residence_A", new Vector3(6, 7, 6), VisualUtils.StuccoWhite, VisualUtils.Terracotta);
            CreateHousePrefab(folderPath, "House_Residence_B", new Vector3(5, 5, 9), VisualUtils.StuccoWhite, VisualUtils.CobaltBlue);

            // 2. Clock Tower
            CreateClockTowerPrefab(folderPath);

            // 3. Fountain
            CreateFountainPrefab(folderPath);

            // 4. Newspaper Board
            CreateNewspaperBoardPrefab(folderPath);

            // 5. Flower Pot
            CreateFlowerPotPrefab(folderPath);

            // 6. Tree
            CreateTreePrefab(folderPath);

            // 7. Bench
            CreateBenchPrefab(folderPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[GPOyun] Çevre Prefab'leri başarıyla '{folderPath}' klasörüne oluşturuldu!");
        }

        private static GameObject CreatePrim(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            return obj;
        }

        private static void CreateHousePrefab(string folder, string name, Vector3 size, Color wallCol, Color roofCol)
        {
            GameObject container = new GameObject(name);
            container.layer = 7; // COLLISION LAYER

            GameObject walls = CreatePrim(PrimitiveType.Cube, "Walls", new Vector3(0, size.y / 2f, 0), size, container.transform);
            VisualUtils.ApplyAesthetic(walls, wallCol);

            GameObject roof = CreatePrim(PrimitiveType.Cube, "Roof", new Vector3(0, size.y + 0.1f, 0), new Vector3(size.x + 0.5f, 0.4f, size.z + 0.5f), container.transform);
            VisualUtils.ApplyAesthetic(roof, roofCol);

            GameObject door = CreatePrim(PrimitiveType.Cube, "Door", new Vector3(0, 1.25f, size.z / 2f + 0.05f), new Vector3(1.5f, 2.5f, 0.2f), container.transform);
            VisualUtils.ApplyAesthetic(door, VisualUtils.CobaltBlue);

            CreateWindow(container.transform, new Vector3(size.x/3f, size.y*0.6f, size.z/2f + 0.05f));
            CreateWindow(container.transform, new Vector3(-size.x/3f, size.y*0.6f, size.z/2f + 0.05f));

            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
            nmo.carving = true;
            nmo.size = size;
            nmo.center = new Vector3(0, size.y / 2f, 0);

            SaveAndDestroy(container, $"{folder}/{name}.prefab");
        }

        private static void CreateWindow(Transform parent, Vector3 pos)
        {
            GameObject window = CreatePrim(PrimitiveType.Cube, "Window", pos, new Vector3(1f, 1f, 0.1f), parent);
            VisualUtils.ApplyAesthetic(window, VisualUtils.FountainBlue, 0.8f);
        }

        private static void CreateClockTowerPrefab(string folder)
        {
            GameObject container = new GameObject("ClockTower");
            
            GameObject tower = CreatePrim(PrimitiveType.Cube, "TowerBase", new Vector3(0, 8f, 0), new Vector3(4f, 16, 4f), container.transform);
            VisualUtils.ApplyAesthetic(tower, VisualUtils.StuccoWhite);
            
            GameObject towerTop = CreatePrim(PrimitiveType.Cube, "TowerCap", new Vector3(0, 16.5f, 0), new Vector3(4.5f, 1, 4.5f), container.transform);
            VisualUtils.ApplyAesthetic(towerTop, VisualUtils.CobaltBlue);
            
            var subject = container.AddComponent<PhotoSubject>();
            subject.PrimaryCategory = GPOyun.Newspaper.NewsCategory.Global;
            
            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
            nmo.carving = true;
            nmo.size = new Vector3(4f, 16, 4f);
            nmo.center = new Vector3(0, 8f, 0);

            SaveAndDestroy(container, $"{folder}/ClockTower.prefab");
        }

        private static void CreateFountainPrefab(string folder)
        {
            GameObject container = new GameObject("Fountain");

            GameObject fBase = CreatePrim(PrimitiveType.Cylinder, "Fountain_Base", Vector3.zero, new Vector3(5, 0.4f, 5), container.transform);
            VisualUtils.ApplyAesthetic(fBase, VisualUtils.SlateGrey);
            
            GameObject water = CreatePrim(PrimitiveType.Cylinder, "Water", new Vector3(0, 0.3f, 0), new Vector3(4.5f, 0.1f, 4.5f), container.transform);
            VisualUtils.ApplyAesthetic(water, VisualUtils.FountainBlue, 0.9f);

            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Capsule;
            nmo.carving = true;
            nmo.radius = 2.5f;
            nmo.height = 1f;

            SaveAndDestroy(container, $"{folder}/Fountain.prefab");
        }

        private static void CreateNewspaperBoardPrefab(string folder)
        {
            GameObject container = new GameObject("NewspaperBoard");

            GameObject board = CreatePrim(PrimitiveType.Cube, "Board", new Vector3(0, 1.5f, 0), new Vector3(0.3f, 3f, 5f), container.transform);
            VisualUtils.ApplyAesthetic(board, VisualUtils.WoodBrown);

            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
            nmo.carving = true;
            nmo.size = new Vector3(0.3f, 3f, 5f);
            nmo.center = new Vector3(0, 1.5f, 0);

            SaveAndDestroy(container, $"{folder}/NewspaperBoard.prefab");
        }

        private static void CreateFlowerPotPrefab(string folder)
        {
            GameObject container = new GameObject("FlowerPot");

            GameObject pot = CreatePrim(PrimitiveType.Cylinder, "Pot", Vector3.zero, new Vector3(0.6f, 0.4f, 0.6f), container.transform);
            VisualUtils.ApplyAesthetic(pot, VisualUtils.Terracotta);
            
            GameObject plant = CreatePrim(PrimitiveType.Sphere, "Plant", Vector3.up * 0.4f, new Vector3(0.5f, 0.5f, 0.5f), container.transform);
            VisualUtils.ApplyAesthetic(plant, Color.green);

            SaveAndDestroy(container, $"{folder}/FlowerPot.prefab");
        }

        private static void CreateTreePrefab(string folder)
        {
            GameObject container = new GameObject("CypressTree");
            container.layer = 7; // COLLISION LAYER

            GameObject trunk = CreatePrim(PrimitiveType.Cylinder, "Trunk", new Vector3(0, -2.5f, 0), new Vector3(0.5f, 1.5f, 0.5f), container.transform);
            VisualUtils.ApplyAesthetic(trunk, VisualUtils.WoodBrown);

            GameObject foliage = CreatePrim(PrimitiveType.Cylinder, "Foliage", Vector3.zero, new Vector3(1.2f, 3f, 1.2f), container.transform);
            VisualUtils.ApplyAesthetic(foliage, VisualUtils.PineGreen);

            GameObject foliageTop = CreatePrim(PrimitiveType.Sphere, "FoliageTop", new Vector3(0, 3f, 0), new Vector3(1.0f, 1.0f, 1.0f), container.transform);
            VisualUtils.ApplyAesthetic(foliageTop, VisualUtils.PineGreen);
            
            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Capsule;
            nmo.carving = true;
            nmo.radius = 0.6f;
            nmo.height = 5f;
            nmo.center = Vector3.zero;

            SaveAndDestroy(container, $"{folder}/CypressTree.prefab");
        }

        private static void CreateBenchPrefab(string folder)
        {
            GameObject container = new GameObject("Bench");
            container.AddComponent<BenchObject>();

            GameObject seat = CreatePrim(PrimitiveType.Cube, "Seat", new Vector3(0, 0.2f, 0), new Vector3(3f, 0.2f, 0.8f), container.transform);
            VisualUtils.ApplyAesthetic(seat, VisualUtils.WoodBrown);

            GameObject back = CreatePrim(PrimitiveType.Cube, "Back", new Vector3(0, 0.6f, 0.4f), new Vector3(3f, 0.8f, 0.1f), container.transform);
            VisualUtils.ApplyAesthetic(back, VisualUtils.WoodBrown);
            
            var nmo = container.AddComponent<UnityEngine.AI.NavMeshObstacle>();
            nmo.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
            nmo.carving = true;
            nmo.size = new Vector3(3f, 1f, 1f);
            nmo.center = new Vector3(0, 0.5f, 0);

            SaveAndDestroy(container, $"{folder}/Bench.prefab");
        }

        private static void SaveAndDestroy(GameObject obj, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, path);
            DestroyImmediate(obj);
        }
    }
}
