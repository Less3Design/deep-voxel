using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    public class RenderView : MonoBehaviour
    {
        // TODO. Goal is is some structure that represents the rendered view of a single layer.
        // Or maybe it should be all the layers? Becuase layers need to cull each other so maybe they all need to be 
        // Maybe render view is a list of LayeredChunk's

        public WorldLayer world;

        [ContextMenu("InitRandom")]
        private void InitializeRandomChunks()
        {
            world = new WorldLayer();
            world.chunks = new Chunk[0];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int z = 0; z < 8; z++)
                    {
                        world.LoadChunk(new Chunk(new Vector3Int(x, y, z)).Randomize());
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            int i=0;
            foreach (Chunk c in world.chunks)
            {
                i++;
                Random.InitState(i);

                Gizmos.color = Random.ColorHSV(0f, 1f);
                int length = c.voxels.GetLength(0);
                Vector3Int scale = new Vector3Int(length, length, length);
                Gizmos.DrawWireCube(Vector3Int.Scale(c.id, scale), scale);
            }
        }
    }
}
