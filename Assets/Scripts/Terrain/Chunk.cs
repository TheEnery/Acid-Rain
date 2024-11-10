using UnityEngine;

namespace AcidRain.Terrain
{
    public class Chunk : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        public bool Visible
        {
            get => _meshRenderer.enabled;
            set { _meshRenderer.enabled = value; }
        }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public static Chunk Create(Mesh mesh, Vector3 position, Transform parent)
        {
            var gameObject = new GameObject($"ChunkX{position.x}Z{position.z}");
            gameObject.transform.localPosition = position;
            gameObject.transform.SetParent(parent, false);

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            var meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            var chunk = gameObject.AddComponent<Chunk>();
            return chunk;
        }
    } 
}