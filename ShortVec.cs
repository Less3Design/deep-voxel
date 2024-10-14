using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Less3.DeepVoxel
{
    public struct ShortVec
    {
        public ushort x;
        public ushort y;
        public ushort z;

        public ShortVec(ushort x, ushort y, ushort z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
