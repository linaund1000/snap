using System.Collections.Generic;
using UnityEngine;

namespace GPOyun.NPC.UtilityAI
{
    public class NPCSenses : MonoBehaviour
    {
        private NPCController _controller;

        [Header("Vision Settings")]
        public float VisionRadius = 15f;
        [Range(0, 180)] public float VisionAngle = 120f; // 120 degree cone
        public LayerMask SightObstacleMask;
        public LayerMask NPCMask;

        public List<NPCController> VisibleNPCs { get; private set; } = new List<NPCController>();

        private float _scanTimer = 0f;
        private float _scanInterval = 0.5f; // Scan every half second

        public void Initialize(NPCController controller)
        {
            _controller = controller;
            
            // Try to find reasonable default masks if not set
            if (SightObstacleMask == 0) SightObstacleMask = LayerMask.GetMask("Default", "Building", "Obstacle");
            if (NPCMask == 0) NPCMask = LayerMask.GetMask("NPC", "Default");
        }

        private void Update()
        {
            if (_controller == null) return;

            _scanTimer += Time.deltaTime;
            if (_scanTimer >= _scanInterval)
            {
                _scanTimer = 0f;
                PerformVisionScan();
            }
        }

        private void PerformVisionScan()
        {
            VisibleNPCs.Clear();

            Collider[] hits = Physics.OverlapSphere(transform.position, VisionRadius, NPCMask);
            foreach (var hit in hits)
            {
                var otherNpc = hit.GetComponentInParent<NPCController>();
                if (otherNpc != null && otherNpc != _controller)
                {
                    Vector3 dirToTarget = (otherNpc.transform.position - transform.position).normalized;
                    dirToTarget.y = 0; // Flatten

                    // Check Vision Cone (Dot product)
                    if (Vector3.Angle(transform.forward, dirToTarget) < VisionAngle / 2f)
                    {
                        // Check Line of Sight (Raycast to ensure no walls)
                        float dist = Vector3.Distance(transform.position, otherNpc.transform.position);
                        Vector3 rayStart = transform.position + Vector3.up * 1.5f; // Eye level
                        Vector3 rayEnd = otherNpc.transform.position + Vector3.up * 1.5f;
                        Vector3 rayDir = (rayEnd - rayStart).normalized;

                        if (!Physics.Raycast(rayStart, rayDir, dist, SightObstacleMask))
                        {
                            VisibleNPCs.Add(otherNpc);
                        }
                    }
                }
            }
        }
        
        // Debug gizmos for Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, VisionRadius);

            Vector3 leftRay = Quaternion.Euler(0, -VisionAngle / 2f, 0) * transform.forward;
            Vector3 rightRay = Quaternion.Euler(0, VisionAngle / 2f, 0) * transform.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, leftRay * VisionRadius);
            Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, rightRay * VisionRadius);
        }
    }
}
