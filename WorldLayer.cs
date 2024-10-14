using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Less3.DeepVoxel
{
    //contains all the currently loaded chunks for a single layer of the world
    public struct WorldLayer
    {
        public Chunk[] chunks;

        public void LoadChunk(Chunk newChunk)
        {
            chunks = chunks.Append(newChunk).ToArray();
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
