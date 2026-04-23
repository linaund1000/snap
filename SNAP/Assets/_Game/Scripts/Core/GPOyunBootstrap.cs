using UnityEngine;
using GPOyun.Events;
using GPOyun.Managers;
using GPOyun.Newspaper;
using GPOyun.Environment;
using GPOyun.Player;
using GPOyun.CameraSystem;
using GPOyun.NPC;
using GPOyun.UI;
using GPOyun;

namespace GPOyun.Core
{
    /// <summary>
    /// The entry point and safety net for the GP-OYUN project.
    /// Ensures all managers exist and are initialized in the correct order.
    /// Also provides automation buttons for Unity Editor setup.
    /// </summary>
    public class GPOyunBootstrap : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private SplashController splashController;

        [Header("Manager Prefabs (Optional)")]
        [SerializeField] private GameObject eventBusPrefab;
        [SerializeField] private GameObject timeManagerPrefab;
        [SerializeField] private GameObject newspaperManagerPrefab;

        private void Awake()
        {
            #if UNITY_EDITOR
            // Automated Cold Start for first-time use
            if (GameObject.Find("[CORE]") == null) SetupHierarchy();
            var builder = FindAnyObjectByType<TownSquareBuilder>();
            if (builder != null && builder.transform.childCount == 0) builder.Build();
            #endif

            VerifyManagers();
        }

        private void Start()
        {
            if (EventBus.Instance != null)
            {
                Debug.Log("[Bootstrap] Publishing CoreInitializedEvent...");
                EventBus.Instance.Publish(new CoreInitializedEvent());
            }
        }

        public void VerifyManagers()
        {
            Debug.Log("[Bootstrap] Verifying core systems...");

            if (EventBus.Instance == null) CreateManager<EventBus>("EventBus", eventBusPrefab);
            if (TimeManager.Instance == null) CreateManager<TimeManager>("TimeManager", timeManagerPrefab);
            if (NewspaperManager.Instance == null) CreateManager<NewspaperManager>("NewspaperManager", newspaperManagerPrefab);

            Debug.Log("[Bootstrap] All systems online.");
        }

        private void CreateManager<T>(string name, GameObject prefab) where T : MonoBehaviour
        {
            if (prefab != null)
            {
                Instantiate(prefab);
            }
            else
            {
                GameObject go = new GameObject(name);
                go.AddComponent<T>();
                Debug.LogWarning($"[Bootstrap] {name} was missing! Created a default one.");
            }
        }

        #if UNITY_EDITOR
        [ContextMenu("Project Setup: Create Folders")]
        public void SetupFolders()
        {
            string[] folders = { "_Game", "_Game/Prefabs", "_Game/Materials", "_Game/Scenes", "_Game/Scripts" };
            foreach (string folder in folders)
            {
                string path = System.IO.Path.Combine(Application.dataPath, folder);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                    Debug.Log($"Created folder: {folder}");
                }
            }
            UnityEditor.AssetDatabase.Refresh();
        }

        [ContextMenu("Scene Setup: Create Hierarchy")]
        public void SetupHierarchy()
        {
            GameObject core = GameObject.Find("[CORE]") ?? new GameObject("[CORE]");
            GameObject ui = GameObject.Find("[UI]") ?? new GameObject("[UI]");

            if (FindAnyObjectByType<GPOyunBootstrap>() == null)
            {
                new GameObject("[BOOTSTRAP]").AddComponent<GPOyunBootstrap>();
            }
            
            CheckAndAdd<EventBus>(core, "EventBus");
            CheckAndAdd<TimeManager>(core, "TimeManager");
            CheckAndAdd<NewspaperManager>(core, "NewspaperManager");

            CheckAndAdd<SplashController>(ui, "SplashScreen");
            CheckAndAdd<SettingsController>(ui, "SettingsScreen");
            
            Debug.Log("[Bootstrap] Hierarchy setup complete.");
        }

        [ContextMenu("!!! NUCLEAR COLD START: FIX PROJECT !!!")]
        public void NuclearColdStart()
        {
            Debug.Log("[Bootstrap] Starting Nuclear Cold Start...");
            
            SetupFolders();
            SetupHierarchy();
            
            GameObject core = GameObject.Find("[CORE]");
            GameObject ui = GameObject.Find("[UI]");

            SetupWorldEnvironment(core);
            Camera cam = SetupLoLCamera();
            GameObject lightGo = SetupLighting();
            SetupAtmosphere(core, lightGo);
            SetupPlayer(core);
            GameManager gm = SetupGameManagers(core);
            SetupCameraSystems(cam);
            SetupUI(ui);

            if (gm != null)
            {
                gm.Initialize(
                    FindAnyObjectByType<NPCManager>(),
                    FindAnyObjectByType<NewspaperManager>(),
                    EventBus.Instance
                );
            }

            Debug.Log("[Bootstrap] ALL SYSTEMS ONLINE.");
        }

        private void SetupWorldEnvironment(GameObject parent)
        {
            var builder = FindAnyObjectByType<TownSquareBuilder>() ?? new GameObject("TownSquareBuilder").AddComponent<TownSquareBuilder>();
            builder.transform.SetParent(parent.transform);
            builder.Build();
        }

        private Camera SetupLoLCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }
            else
            {
                cam.tag = "MainCamera";
            }
            
            cam.transform.position = new Vector3(0, 25, -20);
            cam.transform.rotation = Quaternion.Euler(55, 0, 0);
            cam.fieldOfView = 60;
            return cam;
        }

        private GameObject SetupLighting()
        {
            GameObject lightGo = GameObject.Find("Directional Light");
            if (lightGo == null)
            {
                lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.0f;
                lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);
            }
            return lightGo;
        }

        private void SetupAtmosphere(GameObject parent, GameObject lightGo)
        {
            var atmosphere = FindAnyObjectByType<AtmosphereManager>() ?? new GameObject("AtmosphereManager").AddComponent<AtmosphereManager>();
            atmosphere.transform.SetParent(parent.transform);
            atmosphere.Initialize(lightGo.GetComponent<Light>());
        }

        private void SetupPlayer(GameObject parent)
        {
            GameObject player = GameObject.Find("Player_Mock");
            if (player == null)
            {
                GameObject playerGroup = new GameObject("Player_Mock");
                playerGroup.transform.position = new Vector3(0, 1.5f, -8f); // Moved further back

                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Body";
                body.transform.SetParent(playerGroup.transform);
                body.transform.localPosition = Vector3.zero;
                VisualUtils.ApplyAesthetic(body, VisualUtils.CobaltBlue);
                // Remove primitive collider to avoid fighting with main CharacterController
                DestroyImmediate(body.GetComponent<Collider>());

                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                head.name = "Head";
                head.transform.SetParent(playerGroup.transform);
                head.transform.localPosition = new Vector3(0, 1.2f, 0);
                head.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                VisualUtils.ApplyAesthetic(head, VisualUtils.CobaltBlue);
                DestroyImmediate(head.GetComponent<Collider>());

                var cc = playerGroup.AddComponent<CharacterController>();
                cc.height = 2f;
                cc.center = new Vector3(0, 1f, 0);
                cc.enabled = true; // Ensure active

                playerGroup.tag = "Player";
                playerGroup.AddComponent<PlayerController>();
                VisualUtils.ApplyPlayerVisuals(playerGroup);
            }
            else
            {
                player.tag = "Player";
                var cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = true;
            }
        }

        private GameManager SetupGameManagers(GameObject parent)
        {
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm == null)
            {
                GameObject gmGo = new GameObject("GameManager");
                gm = gmGo.AddComponent<GameManager>();
                gmGo.transform.SetParent(parent.transform);
            }
            return gm;
        }

        private void SetupCameraSystems(Camera cam)
        {
            if (cam.GetComponent<CameraController>() == null) cam.gameObject.AddComponent<CameraController>();
            if (cam.GetComponent<CameraFollow>() == null) cam.gameObject.AddComponent<CameraFollow>();
        }

        private void SetupUI(GameObject parent)
        {
            var hud = FindAnyObjectByType<HUDManager>() ?? new GameObject("HUDManager").AddComponent<HUDManager>();
            hud.transform.SetParent(parent.transform);
            VisualUtils.SetupMockHUD(hud);

            var splash = FindAnyObjectByType<SplashController>() ?? new GameObject("SplashController").AddComponent<SplashController>();
            splash.transform.SetParent(parent.transform);
            VisualUtils.SetupMockSplash(splash);
            
            var settings = FindAnyObjectByType<SettingsController>() ?? new GameObject("SettingsController").AddComponent<SettingsController>();
            settings.transform.SetParent(parent.transform);
            VisualUtils.SetupMockSettings(settings);

            var editorial = FindAnyObjectByType<EditorialUI>() ?? new GameObject("EditorialUI").AddComponent<EditorialUI>();
            editorial.transform.SetParent(parent.transform);
            
            splashController = splash;
        }

        private void CheckAndAdd<T>(GameObject parent, string name) where T : MonoBehaviour
        {
            if (UnityEngine.Object.FindObjectsByType<T>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None).Length == 0)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(parent.transform);
                go.AddComponent<T>();
            }
        }
        #endif
    }
}

