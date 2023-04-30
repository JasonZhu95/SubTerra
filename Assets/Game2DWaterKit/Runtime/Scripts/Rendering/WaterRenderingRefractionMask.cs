namespace Game2DWaterKit.Rendering
{
    using UnityEngine;

    [ExecuteInEditMode, RequireComponent(typeof(MeshRenderer), typeof(MeshFilter)), AddComponentMenu("")]
    public class WaterRenderingRefractionMask : MonoBehaviour
    {
        private static Mesh _mesh;

        private MeshRenderer _meshRenderer;
        private Material _material;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = _material = new Material(Shader.Find("Hidden/Game2DWaterKit-RefractionMask"));
            
            if (_mesh == null)
                GenerateMesh();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }

        private void GenerateMesh()
        {
            _mesh = new Mesh();

            const float size = 9999f; // an arbitrary large size

            var vertices = new Vector3[4];

            vertices[0] = new Vector3(-size, size);
            vertices[1] = new Vector3(size, size);
            vertices[2] = new Vector3(size, -size);
            vertices[3] = new Vector3(-size, -size);

            var triangles = new int[6];

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateBounds();
        }

        internal void SetActive(bool isActive)
        {
            _meshRenderer.enabled = isActive;
        }

        internal void SetupRenderingProperties(int renderQueue, int sortingLayerID, int sortingOrderInLayer)
        {
            _material.renderQueue = renderQueue;
            _meshRenderer.sortingLayerID = sortingLayerID;
            _meshRenderer.sortingOrder = sortingOrderInLayer;
        }
    }
}