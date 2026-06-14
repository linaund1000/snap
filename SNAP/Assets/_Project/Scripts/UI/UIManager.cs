using System.Collections.Generic;
using UnityEngine;
using GPOyun.Core;

namespace GPOyun.UI
{
    /// <summary>
    /// Centralized Finite State Machine for all UI Overlays.
    /// Manages Cursor lock states and GameManager pause states to prevent input conflicts.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Stack<GameObject> _activeMenus = new Stack<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            ServiceLocator.Register<UIManager>(this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                ServiceLocator.Unregister<UIManager>();
                Instance = null;
            }
        }

        private void Start()
        {
            ApplyMooreOutputs(GPOyun.Core.FSM.GlobalRouterState.Immersion);
        }

        /// <summary>
        /// Call this when any fullscreen or overlay UI opens.
        /// </summary>
        public void PushMenu(GameObject menu)
        {
            if (!_activeMenus.Contains(menu))
            {
                _activeMenus.Push(menu);
            }
        }

        /// <summary>
        /// Call this when the currently active overlay UI closes.
        /// </summary>
        public void PopMenu(GameObject menu)
        {
            if (_activeMenus.Count > 0 && _activeMenus.Peek() == menu)
            {
                _activeMenus.Pop();
            }
            else if (_activeMenus.Contains(menu))
            {
                // Edge case: A menu closed out of order. Rebuild stack.
                var temp = new List<GameObject>(_activeMenus);
                temp.Remove(menu);
                _activeMenus.Clear();
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    _activeMenus.Push(temp[i]);
                }
            }
        }

        public void CloseAllMenusFast()
        {
            var temp = new List<GameObject>(_activeMenus);
            _activeMenus.Clear();
            foreach (var m in temp)
            {
                if (m != null) m.SendMessage("Hide", SendMessageOptions.DontRequireReceiver);
            }
            ApplyMooreOutputs(GPOyun.Core.FSM.GlobalRouterState.Immersion);
        }

        public void ApplyMooreOutputs(GPOyun.Core.FSM.GlobalRouterState state)
        {
            switch (state)
            {
                case GPOyun.Core.FSM.GlobalRouterState.Immersion:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1.0f;
                    break;
                case GPOyun.Core.FSM.GlobalRouterState.Framing:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1.0f;
                    break;
                case GPOyun.Core.FSM.GlobalRouterState.UI_Active:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 1.0f; // Live Ticker / Real-time menus
                    break;
                case GPOyun.Core.FSM.GlobalRouterState.Paused:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 0.0f;
                    break;
            }
        }

        /// <summary>
        /// Use ESC to pop the top menu. Can be called by a global input handler or SettingsController.
        /// </summary>
        public bool HandleEscapeInput()
        {
            if (_activeMenus.Count > 0)
            {
                var topMenu = _activeMenus.Peek();
                // We rely on the menu itself to handle its closing animation/logic if possible.
                // If it's a simple toggle, we can disable it here.
                // For safety, we just let the active UI script handle ESC, but UIManager knows it's consumed.
                return true; // Input consumed
            }
            return false; // Not in UI Overlay
        }
        
        public bool IsAnyMenuOpen()
        {
            return _activeMenus.Count > 0;
        }
    }
}
