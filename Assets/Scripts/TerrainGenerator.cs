using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AcidRain
{
    public class TerrainGenerator : MonoBehaviour
    {
        private void Start()
        {
            Mesh mesh = new Mesh();

            const int size = 16;
            Vector3[,] matrix = new Vector3[size, size];

            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++) {
                    matrix[i, j].x = i * 4;
                    matrix[i, j].y = Random.value * 3;
                    matrix[i, j].z = j * 4;
                }
            }

            Vector3[] vertices = matrix.Cast<Vector3>().ToArray();

            var tringlesList = new List<int>();

            for (int i = 0; i < size - 1; i++) {
                for (int j = 0; j < size - 1; j++) {
                    tringlesList.Add(i * size + j);
                    tringlesList.Add(i * size + j + 1);
                    tringlesList.Add((i + 1) * size + j);

                    tringlesList.Add((i + 1) * size + j);
                    tringlesList.Add(i * size + j + 1);
                    tringlesList.Add((i + 1) * size + j + 1);
                }
            }

            int[] triangles = tringlesList.ToArray();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
    }
}
