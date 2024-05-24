using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AcidRain.Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        private IChunkFactory _chunkFactory;
        private Dictionary<Vector2Int, GameObject> _chunks = new();
        private Vector2Int _currentCenter;
        [SerializeField] private int _drawingDistance = 4;
        [SerializeField] private int _loadingDistance = 6;
        private GameObject _player;
        [SerializeField] private int _seed = 1;

        private void Start()
        {
            _chunkFactory = new DiamondSquareChunkFactory(_seed);

            _player = GameObject.Find("Body(Clone)");

            LoadChunks();

            StartCoroutine(CheckChunkLoading());
        }

        private GameObject CreateChunk(int x, int z)
        {
            var chunk = _chunkFactory.Create(x, z);
            chunk.transform.SetParent(transform, false);
            return chunk;
        }

        private IEnumerator CheckChunkLoading()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);

                var newCenter = _chunkFactory.WorldToChunkSystem(_player.transform.position);
                
                if (newCenter != _currentCenter)
                {
                    _currentCenter = newCenter;

                    _chunks
                        .Where(kvChunk => (kvChunk.Key - _currentCenter).magnitude > _loadingDistance)
                        .Select(c => c.Key)
                        .ToList()
                        .ForEach(p =>
                        {
                            var chunk = _chunks.GetValueOrDefault(p);
                            _chunks.Remove(p);
                            Destroy(chunk);
                        });

                    _chunks
                        .ToList()
                        .ForEach(kvChunk =>
                        {
                            if ((kvChunk.Key - _currentCenter).magnitude > _drawingDistance)
                            {
                                kvChunk.Value.GetComponent<MeshRenderer>().enabled = false;
                            }
                        });
                    
                    
                    LoadChunks();
                }
            }
        }

        private void LoadChunks()
        {
            for (int i = -_drawingDistance; i <= _drawingDistance; i++)
            {
                int border = (int)Math.Sqrt(_drawingDistance * _drawingDistance - i * i + 1);
                for (int j = -border; j <= border; j++)
                {
                    var position = new Vector2Int(_currentCenter.x + i, _currentCenter.y + j);

                    if (_chunks.TryGetValue(position, out GameObject chunk))
                    {
                        chunk.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        chunk = CreateChunk(position.x, position.y);
                        _chunks.Add(position, chunk);
                    }
                }
            }
        }
    }
}
