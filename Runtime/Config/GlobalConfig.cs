using System;
using UnityEngine.Serialization;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace RealDream
{
    [Serializable]
    public class GlobalConfig
    {
        public static GlobalConfig _instance;
        public string DefaultServerIp;
        public int DefaultServerPort;
        public static GlobalConfig Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_5_3_OR_NEWER
                    var config = Resources.Load<TextAsset>("rdx.common.config");
                    _instance = JsonUtility.FromJson<GlobalConfig>(config.text);
#else
                    // TODO
                    throw new NotImplementedException("load config from file not implemented yet!");
#endif
                }

                return _instance;
            }
        }
    }
}