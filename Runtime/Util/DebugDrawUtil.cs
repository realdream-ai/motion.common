using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealDream.AI.RotationParser
{
    public class DebugUtil
    {
        public static Vector3 DebugOffset;

        public static void DrawRotation(Vector3 pos, Quaternion rot, float lineSize)
        {
            var f = rot * Vector3.forward * lineSize;
            var u = rot * Vector3.up * lineSize;
            var r = rot * Vector3.right * lineSize;
            UnityEngine.Debug.DrawLine(pos, pos + r, Color.red);
            UnityEngine.Debug.DrawLine(pos, pos + u, Color.green);
            UnityEngine.Debug.DrawLine(pos, pos + f, Color.blue);
        }
    }
}