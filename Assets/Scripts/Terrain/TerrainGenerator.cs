using UnityEngine;

namespace AcidRain.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private int _seed = 1;
        [SerializeField] private IChunkFactory _chunkBuilder;

        private void Start()
        {
            _chunkBuilder = new DiamondSquareChunkFactory(_seed);

            var chunksInRadius = 4;
            for(int x = -chunksInRadius; x <= chunksInRadius; x++)
            {
                for (int z = -chunksInRadius; z <= chunksInRadius; z++)
                {
                    _chunkBuilder.Create(x, z).transform.SetParent(transform, false);
                }
            }
        }
    }
}
