using System.Collections.Generic;
using UnityEngine;

namespace RealDream.AI
{
    [CreateAssetMenu(fileName = "CachedInitRotation", menuName = "Config/CachedInitRotation")]
    public class CachedInitRotation : ScriptableObject
    {
        public List<Vector3> Rots = new List<Vector3>();
    }
}