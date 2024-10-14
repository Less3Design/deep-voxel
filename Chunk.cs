using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    [System.Serializable]
    public class Chunk
    {
        public const int CHUNK_SIZE = 32;
        public Voxel[,,] voxels = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

        public Vector3Int id;

        public Chunk(Vector3Int id)
        {
            this.id = id;
        }

        public RenderChunk Render(Chunk[] neighbors)
        {
            RenderChunk render = new RenderChunk();

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        ProcessVoxel(this, render, x, y, z);
                    }
                }
            }

            void ProcessVoxel(Chunk chunk, RenderChunk renderChunk, int x, int y, int z)
            {
                // Check if the voxels array is initialized and the indices are within bounds
                if (x < 0 || x >= chunk.voxels.GetLength(0) ||
                    y < 0 || y >= chunk.voxels.GetLength(1) || z < 0 || z >= chunk.voxels.GetLength(2))
                {
                    return; // Skip processing if the array is not initialized or indices are out of bounds
                }
                Voxel voxel = chunk.voxels[x, y, z];
                if (voxel.id != 0)
                {
                    // Check each face of the voxel for visibility
                    bool[] facesVisible = new bool[6];

                    // Check visibility for each face
                    facesVisible[0] = chunk.IsFaceVisible(x, y + 1, z); // Top
                    facesVisible[1] = chunk.IsFaceVisible(x, y - 1, z); // Bottom
                    facesVisible[2] = chunk.IsFaceVisible(x - 1, y, z); // Left
                    facesVisible[3] = chunk.IsFaceVisible(x + 1, y, z); // Right
                    facesVisible[4] = chunk.IsFaceVisible(x, y, z + 1); // Front
                    facesVisible[5] = chunk.IsFaceVisible(x, y, z - 1); // Back

                    for (int i = 0; i < facesVisible.Length; i++)
                    {
                        if (facesVisible[i])
                            renderChunk.AddFaceData(x, y, z, i);
                    }
                }
            }

            return render;
        }

        public bool IsVoxelAir(Vector3Int voxel)
        {
            return voxels[voxel.x, voxel.y, voxel.z].id == 0;
        }

        /// <summary>
        /// Use to check if a face should be rendered against this voxel. Basically checking if the voxel is air 
        /// </summary>
        public bool IsFaceVisible(int x, int y, int z)
        {
            if (x < 0 || x >= CHUNK_SIZE || y < 0 || y >= CHUNK_SIZE || z < 0 || z >= CHUNK_SIZE)
                return true; // Face is at the boundary of the chunk
            return voxels[x, y, z].id == 0;
        }
    }

    /// <summary>
    /// A chunk is proccessed and turned into a renderChunk. Basically raw geometry data.
    /// </summary>
    public class RenderChunk
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;

        public void AddFaceData(int x, int y, int z, int faceIndex)
        {
            // Based on faceIndex, determine vertices and triangles
            // Add vertices and triangles for the visible face
            // Calculate and add corresponding UVs

            if (faceIndex == 0) // Top Face
            {
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
            }

            if (faceIndex == 1) // Bottom Face
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));
            }

            if (faceIndex == 2) // Left Face
            {
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x, y + 1, z));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(0, 1));
            }

            if (faceIndex == 3) // Right Face
            {
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));
            }

            if (faceIndex == 4) // Front Face
            {
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 1));
            }

            if (faceIndex == 5) // Back Face
            {
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

            }

            int vertCount = vertices.Count;

            // First triangle
            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 3);
            triangles.Add(vertCount - 2);

            // Second triangle
            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 2);
            triangles.Add(vertCount - 1);
        }
    }

    public static class ChunkUtil
    {
        public static Chunk Randomize(this Chunk chunk)
        {
            int length = chunk.voxels.GetLength(0);
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        chunk.voxels[x, y, z] = new Voxel((ushort)Random.Range(0, 2));
                    }
                }
            }
            return chunk;
        }
    }
}
