using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Less3.DeepVoxel
{
    //contains all the currently loaded chunks for a single layer of the world
    public class WorldLayer
    {
        public Chunk[] chunks;

        public void LoadChunk(Chunk newChunk)
        {
            chunks = chunks.Append(newChunk).ToArray();
            newChunk.world = this;
        }

        public void UnloadChunk(Chunk newChunk)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].id == newChunk.id)
                {
                    RemoveAt(ref chunks, i);
                }
            }
        }

        public Chunk GetChunkAtId(Vector3Int chunkPos)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].id == chunkPos)
                {
                    return chunks[i];
                }
            }
            return null;
        }

        public bool TryGetChunkAtId(Vector3Int chunkId, out Chunk chunk)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].id == chunkId)
                {
                    chunk = chunks[i];
                    return true;
                }
            }
            chunk = null;
            return false;
        }

        public bool TryGetVoxelAt(Vector3Int voxelPos, out Voxel voxel)
        {
            Vector3Int chunkId = new Vector3Int(
                voxelPos.x >= 0 ? voxelPos.x / Chunk.CHUNK_SIZE : (voxelPos.x / Chunk.CHUNK_SIZE) - 1,
                voxelPos.y >= 0 ? voxelPos.y / Chunk.CHUNK_SIZE : (voxelPos.y / Chunk.CHUNK_SIZE) - 1,
                voxelPos.z >= 0 ? voxelPos.z / Chunk.CHUNK_SIZE : (voxelPos.z / Chunk.CHUNK_SIZE) - 1);
            Vector3Int localPos = voxelPos - (chunkId * Chunk.CHUNK_SIZE);

            if (TryGetChunkAtId(chunkId, out Chunk chunk))
            {
                voxel = chunk.voxels[localPos.x, localPos.y, localPos.z];
                return true;
            }
            voxel = default;
            return false;
        }

        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }
    }
}
