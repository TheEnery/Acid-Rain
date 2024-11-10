using UnityEngine;

namespace AcidRain.Terrain
{
    public interface IChunkMeshFactory
    {
        public float ChunkSideLength { get; }
        public int Seed { get; }
        public Mesh Create(int x, int z);
        public Vector2Int WorldToChunkSystem(Vector3 position);
    }
}
