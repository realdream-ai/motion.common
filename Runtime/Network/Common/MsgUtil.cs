using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealDream.Network
{
    public class MsgUtil
    {
        public static bool IsDebugMode = false;
        public static void LogError(string msg)
        {
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.LogError(msg);
#else
            Console.WriteLine("[ERROR]=== " + msg);
#endif
        }
        public static void Log(string msg)
        {
#if UNITY_5_3_OR_NEWER 
            UnityEngine.Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        public static void LogBytes(byte[] byteArray)
        {
            if (IsDebugMode)
            {
                Log("LogBytes len ="+byteArray.Length);
                //Log(BitConverter.ToString(byteArray).Replace("-", " "));
            }
            else
            {
                //Log("Bytes " + byteArray.Length);
            }
        }

        public static void RegisterPacketHandlers<T>(System.Type typeStart, Dictionary<int, T> packetHandlers)
        {
            var types = typeStart.Assembly.GetTypes();
            foreach (var type in types)
            {
                var methods =
                    type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes<PacketHandlerAttribute>(false);
                    foreach (var attribute in attributes)
                    {
                        if (method.CreateDelegate(typeof(T)) is T handler)
                        {
                            Log($"Register {attribute.PacketId} {method.Name}");
                            packetHandlers.Add(attribute.PacketId, handler);
                        }
                    }
                }
            }
        }
    }
}