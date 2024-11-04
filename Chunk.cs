using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    [System.Serializable]
    public class Chunk
    {
        public const int CHUNK_SIZE = 16;

        public WorldLayer world;
        public Voxel[,,] voxels = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

        public Vector3Int id;

        public Chunk(Vector3Int id)
        {
            this.id = id;
        }

        /// <summary>
        /// Returns the 6 cardinal neighbors of this chunk IF they exist
        /// </summary>
        public Chunk[] GetCardinalNeighbors()
        {
            if (world == null) return new Chunk[0];

            List<Chunk> neighbors = new List<Chunk>();
            if (world.TryGetChunkAtId(id + Vector3Int.up, out Chunk up))
                neighbors.Add(up);
            if (world.TryGetChunkAtId(id + Vector3Int.down, out Chunk down))
                neighbors.Add(down);
            if (world.TryGetChunkAtId(id + Vector3Int.left, out Chunk left))
                neighbors.Add(left);
            if (world.TryGetChunkAtId(id + Vector3Int.right, out Chunk right))
                neighbors.Add(right);
            if (world.TryGetChunkAtId(id + Vector3Int.forward, out Chunk forward))
                neighbors.Add(forward);
            if (world.TryGetChunkAtId(id + Vector3Int.back, out Chunk back))
                neighbors.Add(back);

            return neighbors.ToArray();
        }

        public RenderChunk Render()
        {
            RenderChunk render = new RenderChunk();

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        ProcessVoxelLocalOnly(this, render, x, y, z);
                    }
                }
            }

            void ProcessVoxelLocalOnly(Chunk chunk, RenderChunk renderChunk, int x, int y, int z)
            {
                if (x < 0 || x >= chunk.voxels.GetLength(0) ||
                    y < 0 || y >= chunk.voxels.GetLength(1) ||
                    z < 0 || z >= chunk.voxels.GetLength(2))
                {
                    return; // Skip if the array is not initialized or indices are out of bounds
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

        private bool IsFaceVisible(int x, int y, int z)
        {
            return IsVoxelHiddenInChunk(x, y, z) && IsVoxelHiddenInWorld(x, y, z);
        }

        public bool IsVoxelAir(Vector3Int voxel)
        {
            return voxels[voxel.x, voxel.y, voxel.z].id == 0;
        }

        /// <summary>
        /// Use to check if a face should be rendered against this voxel. Basically checking if the voxel is air 
        /// </summary>
        public bool IsVoxelHiddenInChunk(int x, int y, int z)
        {
            if (x < 0 || x >= CHUNK_SIZE || y < 0 || y >= CHUNK_SIZE || z < 0 || z >= CHUNK_SIZE)
                return true; // Face is at the boundary of the chunk
            return voxels[x, y, z].id == 0;
        }

        public bool IsVoxelHiddenInWorld(int x, int y, int z)
        {
            if (world.TryGetVoxelAt(GetGlobalVoxelPosition(x, y, z), out Voxel voxel))
            {
                return voxel.id == 0;
            }
            else
            {
                return true;
            }
        }

        public Vector3Int GetGlobalVoxelPosition(int x, int y, int z)
        {
            return new Vector3Int(
                x + id.x * CHUNK_SIZE,
                y + id.y * CHUNK_SIZE,
                z + id.z * CHUNK_SIZE);
        }

        public bool TryGetVoxelAtGlobalPosition(Vector3Int globalPos, out Voxel voxel)
        {
            Vector3Int pos = new Vector3Int(
                globalPos.x - id.x * CHUNK_SIZE,
                globalPos.y - id.y * CHUNK_SIZE,
                globalPos.z - id.z * CHUNK_SIZE);

            if (pos.x < 0 || pos.x >= CHUNK_SIZE ||
                pos.y < 0 || pos.y >= CHUNK_SIZE ||
                pos.z < 0 || pos.z >= CHUNK_SIZE)
            {
                voxel = new Voxel(0);
                return false;
            }
            else
            {
                voxel = voxels[pos.x, pos.y, pos.z];
                return true;
            }
        }
    }

    /// <summary>
    /// A chunk is proccessed and turned into a renderChunk. Basically raw geometry data.
    /// </summary>
    public class RenderChunk
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();

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
            float PerlinNoise3D(float x, float y, float z)
            {
                float xy = Mathf.PerlinNoise(x * 0.015f, y * 0.015f);
                float xz = Mathf.PerlinNoise(x * 0.015f, z * 0.015f);
                float yz = Mathf.PerlinNoise(y * 0.015f, z * 0.015f);
                float yx = Mathf.PerlinNoise(y * 0.015f, x * 0.015f);
                float zx = Mathf.PerlinNoise(z * 0.015f, x * 0.015f);
                float zy = Mathf.PerlinNoise(z * 0.015f, y * 0.015f);

                return (xy + xz + yz + yx + zx + zy) / 6f;
            }

            int length = chunk.voxels.GetLength(0);
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        if (PerlinNoise3D(
                            x + chunk.id.x * Chunk.CHUNK_SIZE,
                            y + chunk.id.y * Chunk.CHUNK_SIZE,
                            z + chunk.id.z * Chunk.CHUNK_SIZE) >= 0.5f)
                        {
                            chunk.voxels[x, y, z] = new Voxel(1);
                        }
                        else
                        {
                            chunk.voxels[x, y, z] = new Voxel(0);
                        }
                    }
                }
            }
            return chunk;
        }
    }
}
