using UnityEngine;

namespace RealDream
{
    public static class TransformExt
    {
        public static T GetOrAddComponent<T>(this Transform self) where T : Component
        {
            var comp = self.GetComponent<T>();
            if (comp == null)
            {
                comp = self.gameObject.AddComponent<T>();
            }

            return comp;
        }
    }
}