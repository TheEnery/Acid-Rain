using UnityEngine;

namespace AcidRain.Terrain
{
    public interface IChunkFactory
    {
        public int Seed { get; }
        public GameObject Create(int x, int z);
        public Vector2Int WorldToChunkSystem(Vector3 position);
    }
}
