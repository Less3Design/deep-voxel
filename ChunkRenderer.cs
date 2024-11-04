using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    public class ChunkRenderer : MonoBehaviour
    {
        public MeshFilter filter;
        public MeshRenderer mr;

        private Chunk _chunk;
        private RenderChunk _renderChunk;

        public void SetChunk(Chunk c)
        {
            filter = gameObject.AddComponent<MeshFilter>();
            mr = gameObject.AddComponent<MeshRenderer>();
            filter.sharedMesh = new Mesh();

            _chunk = c;
            _renderChunk = c.Render();
            filter.sharedMesh.vertices = _renderChunk.vertices.ToArray();
            filter.sharedMesh.triangles = _renderChunk.triangles.ToArray();
            filter.sharedMesh.uv = _renderChunk.uvs.ToArray();

            transform.position = _chunk.id * Chunk.CHUNK_SIZE;
        }
    }
}
