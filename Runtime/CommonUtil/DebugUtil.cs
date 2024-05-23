using System;

namespace RealDream
{
    public class Debug
    {
        public static void Log(string msg)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }
        
        public static void LogError(Exception msg)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.LogError(msg);
#else
            Console.WriteLine(msg.ToString()); 
#endif
        }
        public static void LogError(string msg)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.LogError(msg);
#else
            Console.WriteLine(msg); 
#endif
        }
    }
}