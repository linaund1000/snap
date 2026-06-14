using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GPOyun.EditorScripts
{
    public class OBJExporter : ScriptableObject
    {
        [MenuItem("GameObject/GPOyun/Export Selected to OBJ", false, 10)]
        static void ExportSelectedToOBJ()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogWarning("[GPOyun] Lütfen dışa aktarmak için bir obje seçin.");
                return;
            }

            string exportFolder = "Assets/_Project/Models/Exported";
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Models"))
                AssetDatabase.CreateFolder("Assets/_Project", "Models");
            if (!AssetDatabase.IsValidFolder(exportFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Models", "Exported");

            foreach (GameObject selected in Selection.gameObjects)
            {
                ExportGameObject(selected, exportFolder);
            }
        }

        static void ExportGameObject(GameObject obj, string folder)
        {
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0)
            {
                Debug.LogWarning($"[GPOyun] {obj.name} içinde hiç 3D model (MeshFilter) bulunamadı.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"# GPOyun OBJ Exporter - {obj.name}");

            int vertexOffset = 0;
            int normalOffset = 0;
            int uvOffset = 0;

            foreach (MeshFilter mf in meshFilters)
            {
                Mesh mesh = mf.sharedMesh;
                if (mesh == null) continue;

                Renderer renderer = mf.GetComponent<Renderer>();
                string matName = renderer != null && renderer.sharedMaterial != null ? renderer.sharedMaterial.name : "Default";

                sb.AppendLine($"\ng {mf.name}_{matName}");

                // Dönüşümleri (Transform) ayarla
                Matrix4x4 localToWorld = mf.transform.localToWorldMatrix;
                // Eğer ana obje sıfır noktasında değilse, objenin kendi merkezine göre export almak daha iyidir
                Matrix4x4 objToWorld = obj.transform.worldToLocalMatrix * localToWorld;

                // Vertices
                foreach (Vector3 v in mesh.vertices)
                {
                    Vector3 wv = objToWorld.MultiplyPoint3x4(v);
                    // OBJ format expects X Y Z but Unity uses Left-Handed Y-Up. We invert X.
                    sb.AppendLine($"v {-wv.x} {wv.y} {wv.z}");
                }

                // Normals
                foreach (Vector3 n in mesh.normals)
                {
                    Vector3 wn = objToWorld.MultiplyVector(n);
                    sb.AppendLine($"vn {-wn.x} {wn.y} {wn.z}");
                }

                // UVs
                foreach (Vector2 uv in mesh.uv)
                {
                    sb.AppendLine($"vt {uv.x} {uv.y}");
                }

                // Triangles (Faces)
                for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                {
                    int[] triangles = mesh.GetTriangles(submesh);
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        // OBJ is 1-indexed. We invert triangle order to fix normal direction due to inverted X.
                        int i1 = triangles[i] + 1 + vertexOffset;
                        int i2 = triangles[i + 2] + 1 + vertexOffset;
                        int i3 = triangles[i + 1] + 1 + vertexOffset;

                        sb.AppendLine(HasNormalsAndUVs(mesh) 
                            ? $"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}"
                            : $"f {i1} {i2} {i3}");
                    }
                }

                vertexOffset += mesh.vertices.Length;
                normalOffset += mesh.normals.Length;
                uvOffset += mesh.uv.Length;
            }

            string filePath = $"{folder}/{obj.name}.obj";
            File.WriteAllText(filePath, sb.ToString());
            
            AssetDatabase.Refresh();
            Debug.Log($"[GPOyun] {obj.name} başarıyla OBJ olarak kaydedildi: {filePath}");
        }

        static bool HasNormalsAndUVs(Mesh mesh)
        {
            return mesh.normals != null && mesh.normals.Length > 0 && mesh.uv != null && mesh.uv.Length > 0;
        }
    }
}
