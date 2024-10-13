using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    [System.Serializable]
    public struct Chunk
    {
        public const int CHUNK_SIZE = 16;// length of a single axis of a chunk. Chunks are always cubes of this value
        public Voxel[,,] voxels;

        public Chunk(int size)
        {
            voxels = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
        }
    }
}
