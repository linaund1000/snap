using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GPOyun.UI
{
    /// <summary>
    /// Handles the initial splash screen for 'snap'.
    /// Displays the title and fades out when initialization is complete.
    /// </summary>
    public class SplashController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup splashCanvasGroup;

        public void Initialize(CanvasGroup cg, Text title)
        {
            splashCanvasGroup = cg;
        }

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float holdDuration = 1.0f;

        private void Start()
        {
            // Ensure the splash is visible at startup
            if (splashCanvasGroup != null)
            {
                splashCanvasGroup.alpha = 1f;
                splashCanvasGroup.blocksRaycasts = true;
            }

            if (GPOyun.Events.EventBus.Instance != null)
            {
                GPOyun.Events.EventBus.Instance.Subscribe<GPOyun.Events.CoreInitializedEvent>(OnCoreInitialized);
            }

            // Safety Fallback: If for some reason the event never fires, auto-fade after 5 seconds
            Invoke(nameof(StartFadeOut), 5f);
        }

        private void OnDestroy()
        {
            if (GPOyun.Events.EventBus.Instance != null)
            {
                GPOyun.Events.EventBus.Instance.Unsubscribe<GPOyun.Events.CoreInitializedEvent>(OnCoreInitialized);
            }
        }

        private void OnCoreInitialized(GPOyun.Events.CoreInitializedEvent ev)
        {
            StartFadeOut();
        }

        /// <summary>
        /// Triggered by the Bootstrap when systems are ready.
        /// </summary>
        public void StartFadeOut()
        {
            StartCoroutine(FadeSequence());
        }

        private IEnumerator FadeSequence()
        {
            yield return new WaitForSeconds(holdDuration);

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                if (splashCanvasGroup != null)
                {
                    splashCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
                }
                yield return null;
            }

            if (splashCanvasGroup != null)
            {
                splashCanvasGroup.alpha = 0f;
                splashCanvasGroup.blocksRaycasts = false;
            }
            
            Debug.Log("[Splash] Fade complete. World visible.");
            
            // Optionally Destroy to save memory
            Destroy(gameObject, 0.5f);
        }
    }
}
