using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealDream.AI
{
    public static class VectorExt
    {
        public static Vector3 ToVector3(this float[] data, int offset = 0)
        {
            Vector3 mat = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                mat[i] = data[i + offset];
            }

            return mat;
        }

        public static Matrix4x4 ToMatrix4x4(this float[] data, int offset = 0)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    mat[i, j] = data[i * 4 + j + offset];
                }
            }

            return mat;
        }
    }
}