using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    public class ChunkPreview : MonoBehaviour
    {
        public Chunk chunk = new Chunk(Chunk.CHUNK_SIZE);

        [ContextMenu("NewChunk")]
        public void NewRandomChunk()
        {
            chunk = new Chunk(Chunk.CHUNK_SIZE);

            for (int i = 0; i < Chunk.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < Chunk.CHUNK_SIZE; j++)
                {
                    for (int k = 0; k < Chunk.CHUNK_SIZE; k++)
                    {
                        chunk.voxels[i, j, k].id = (ushort)Random.Range(0, 6);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < Chunk.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < Chunk.CHUNK_SIZE; j++)
                {
                    for (int k = 0; k < Chunk.CHUNK_SIZE; k++)
                    {
                        ushort id = chunk.voxels[i, j, k].id;
                        if (id > 0)
                        {
                            switch (id)
                            {
                                case 1:
                                    Gizmos.color = Color.red;
                                    break;
                                case 2:
                                    Gizmos.color = Color.green;
                                    break;
                                case 3:
                                    Gizmos.color = Color.blue;
                                    break;
                                case 4:
                                    Gizmos.color = Color.magenta;
                                    break;
                                case 5:
                                    Gizmos.color = Color.yellow;
                                    break;
                                default:
                                    Gizmos.color = Color.cyan;
                                    break;
                            }
                            Gizmos.DrawCube(new Vector3(i, j, k), Vector3.one);
                        }
                    }
                }
            }
        }
    }
}
