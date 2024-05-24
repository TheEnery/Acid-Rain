using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AcidRain.Terrain
{
    public class DiamondSquareChunkFactory : IChunkFactory
    {
        public const float ChunkSideLength = 45f;
        public const int VerticesOnSide = 17;

        public int Seed {  get; private set; }

        // since the triangles are independent of the coordinates of the vertices, I can reuse them
        private readonly int[] _cachedTrianglesArray;

        public DiamondSquareChunkFactory(int seed) 
        { 
            Seed = seed;
            _cachedTrianglesArray = CreateTrianglesArray();
        }

        public GameObject Create(int x, int z)
        {
            Vector3[] vertices = CreateVerticesArray(x, z);
            int[] triangles = _cachedTrianglesArray.ToArray();

            var chunk = new GameObject($"ChunkX{x}Z{z}");
            chunk.transform.localPosition = new Vector3(x * ChunkSideLength, 0, z * ChunkSideLength);

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };

            mesh.RecalculateNormals();

            AddComponents(chunk, mesh);

            return chunk;
        }

        private void AddComponents(GameObject chunk, Mesh mesh)
        {
            var meshFilter = chunk.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            var meshCollider = chunk.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private int[] CreateTrianglesArray()
        {
            var tringlesList = new List<int>();

            for (int i = 0; i < VerticesOnSide - 1; i++)
            {
                for (int j = 0; j < VerticesOnSide - 1; j++)
                {
                    int tlVerticeIndex = i * VerticesOnSide + j;
                    int trVerticeIndex = tlVerticeIndex + 1;
                    int blVerticeIndex = (i + 1) * VerticesOnSide + j;
                    int brVerticeIndex = blVerticeIndex + 1;

                    tringlesList.Add(tlVerticeIndex);
                    tringlesList.Add(trVerticeIndex);
                    tringlesList.Add(blVerticeIndex);

                    tringlesList.Add(blVerticeIndex);
                    tringlesList.Add(trVerticeIndex);
                    tringlesList.Add(brVerticeIndex);
                }
            }

            int[] triangles = tringlesList.ToArray();
            return triangles;
        }

        private Vector3[] CreateVerticesArray(int x, int z)
        {
            var matrix = new Vector3[VerticesOnSide, VerticesOnSide];
            var gap = ChunkSideLength / (VerticesOnSide - 1);

            for (int i = 0; i < VerticesOnSide; i++)
            {
                for (int j = 0; j < VerticesOnSide; j++)
                {
                    matrix[i, j].x = i * gap;
                    matrix[i, j].y = 0f;
                    matrix[i, j].z = j * gap;
                }
            }

            matrix[0, 0].y = GetGlobalY(x, z);
            matrix[VerticesOnSide - 1, VerticesOnSide - 1].y = GetGlobalY(x + 1, z + 1);
            matrix[0, VerticesOnSide - 1].y = GetGlobalY(x, z + 1);
            matrix[VerticesOnSide - 1, 0].y = GetGlobalY(x + 1, z);

            var chunkSeed = GetChunkSeed(x, z);
            var nChunkSeed = GetChunkSeed(x, z + 1);
            var sChunkSeed = GetChunkSeed(x, z - 1);
            var eChunkSeed = GetChunkSeed(x + 1, z);
            var wChunkSeed = GetChunkSeed(x - 1, z);

            var nRandom = new System.Random(GetBorderSeed(chunkSeed, nChunkSeed));
            var sRandom = new System.Random(GetBorderSeed(sChunkSeed, chunkSeed));
            var eRandom = new System.Random(GetBorderSeed(chunkSeed, eChunkSeed));
            var wRandom = new System.Random(GetBorderSeed(wChunkSeed, chunkSeed));
            var stepSize = VerticesOnSide - 1;
            var roughness = 0.1f;

            while (stepSize > 1)
            {
                int halfStep = stepSize / 2;
                float scale = roughness * stepSize;

                for (int i = 0; i < VerticesOnSide - 1; i += stepSize)
                {
                    matrix[i + halfStep, VerticesOnSide - 1].y = (matrix[i, VerticesOnSide - 1].y + matrix[i + stepSize, VerticesOnSide - 1].y) / 2 + ((float)nRandom.NextDouble() * 2 - 1) * scale;
                    matrix[i + halfStep, 0].y = (matrix[i, 0].y + matrix[i + stepSize, 0].y) / 2 + ((float)sRandom.NextDouble() * 2 - 1) * scale;
                    matrix[VerticesOnSide - 1, i + halfStep].y = (matrix[VerticesOnSide - 1, i].y + matrix[VerticesOnSide - 1, i + stepSize].y) / 2 + ((float)eRandom.NextDouble() * 2 - 1) * scale;
                    matrix[0, i + halfStep].y = (matrix[0, i].y + matrix[0, i + stepSize].y) / 2 + ((float)wRandom.NextDouble() * 2 - 1) * scale;
                }

                stepSize /= 2;
            }

            GenerateHeightMap(matrix, chunkSeed, 0.5f);

            Vector3[] vertices = matrix.Cast<Vector3>().ToArray();

            return vertices;
        }

        private Vector3[,] GenerateHeightMap(Vector3[,] map, int seed, float roughness)
        {
            var random = new System.Random(seed);
            var stepSize = VerticesOnSide - 1;

            while (stepSize > 1)
            {
                int halfStep = stepSize / 2;
                float scale = roughness * stepSize;

                // Diamond step
                for (int x = 0; x < VerticesOnSide - 1; x += stepSize)
                {
                    for (int y = 0; y < VerticesOnSide - 1; y += stepSize)
                    {
                        var mx = x + halfStep;
                        var my = y + halfStep;
                        if (mx == 0 || mx == VerticesOnSide - 1 || my == 0 || my == VerticesOnSide - 1) continue;
                        float avg = (map[x, y].y + map[x + stepSize, y].y + map[x, y + stepSize].y + map[x + stepSize, y + stepSize].y) / 4.0f;
                        map[mx, my].y = avg + (float)(random.NextDouble() * 2 - 1) * scale;
                    }
                }

                // Square step
                for (int x = 0; x < VerticesOnSide; x += halfStep)
                {
                    for (int y = (x + halfStep) % stepSize; y < VerticesOnSide; y += stepSize)
                    {
                        if (x == 0 || x == VerticesOnSide - 1 || y == 0 || y == VerticesOnSide - 1) continue;

                        float sum = 0;
                        int count = 0;

                        if (x >= halfStep) { sum += map[x - halfStep, y].y; count++; }
                        if (x + halfStep < VerticesOnSide) { sum += map[x + halfStep, y].y; count++; }
                        if (y >= halfStep) { sum += map[x, y - halfStep].y; count++; }
                        if (y + halfStep < VerticesOnSide) { sum += map[x, y + halfStep].y; count++; }

                        map[x, y].y = sum / count + (float)(random.NextDouble() * 2 - 1) * scale;
                    }
                }

                stepSize /= 2;
            }

            return map;
        }

        private float GetGlobalY(int x, int z)
        {
            const int chunksInGlobal = 10;
            var globalX = Mathf.FloorToInt((float)x / chunksInGlobal) * chunksInGlobal;
            var globalZ = Mathf.FloorToInt((float)z / chunksInGlobal) * chunksInGlobal;

            const int partOfGlobalInSeed = 10;
            var swGlobalY = GetChunkSeed(globalX, globalZ) % partOfGlobalInSeed;
            var neGlobalY = GetChunkSeed(globalX + chunksInGlobal, globalZ + chunksInGlobal) % partOfGlobalInSeed;
            var seGlobalY = GetChunkSeed(globalX + chunksInGlobal, globalZ) % partOfGlobalInSeed;
            var nwGlobalY = GetChunkSeed(globalX, globalZ + chunksInGlobal) % partOfGlobalInSeed;

            var nLerpY = Mathf.Lerp(neGlobalY, nwGlobalY, x);
            var sLerpY = Mathf.Lerp(seGlobalY, swGlobalY, x);
            var lerpY = Mathf.Lerp(nLerpY, sLerpY, z);
            var randomness = GetChunkSeed(x, z) % 10;

            return lerpY + randomness;
        }

        private int GetBorderSeed(int swChunkSeed, int neChunkSeed) => GetHashCode(Seed, swChunkSeed, neChunkSeed);
        private int GetChunkSeed(int x, int z) => GetHashCode(Seed, x, z);

        private int GetHashCode(int x, int y, int z)
        {
            const int prime1 = 73856093;
            const int prime2 = 19349663;
            const int prime3 = 83492791;

            int hash = 34255667;
            hash ^= (hash + x) * prime1;
            hash ^= (hash + y) * prime2;
            hash ^= (hash + z) * prime3;
            hash ^= (hash + x) * prime3;
            hash ^= (hash + y) * prime2;
            hash ^= (hash + z) * prime1;

            return hash;
        }

        public Vector2Int WorldToChunkSystem(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / ChunkSideLength),
                Mathf.FloorToInt(position.z / ChunkSideLength));
        }
    }
}
