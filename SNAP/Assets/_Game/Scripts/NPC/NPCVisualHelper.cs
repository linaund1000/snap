using UnityEngine;

namespace GPOyun.NPC
{
    /// <summary>
    /// Helper to assign a unique color to an NPC capsule based on their ID.
    /// This makes characters distinguishable in the proto-town.
    /// </summary>
    public class NPCVisualHelper : MonoBehaviour
    {
        [SerializeField] private NPCController controller;
        [SerializeField] private Renderer capsuleRenderer;

        private void Start()
        {
            if (controller == null) controller = GetComponent<NPCController>();
            if (capsuleRenderer == null) capsuleRenderer = GetComponentInChildren<Renderer>();

            ApplyVisuals();
        }

        public void ApplyVisuals()
        {
            if (capsuleRenderer == null || controller == null) return;

            // Generate a color based on NpcId to keep it consistent
            float hue = (controller.NpcId * 0.13f) % 1.0f;
            Color npcColor = Color.HSVToRGB(hue, 0.7f, 0.8f);

            capsuleRenderer.material = new Material(Shader.Find("Standard"));
            capsuleRenderer.material.color = npcColor;
            
            Debug.Log($"[NPC_{controller.NpcId}] Visual color assigned.");
        }
    }
}
