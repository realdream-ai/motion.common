using System.Collections;
using System.Collections.Generic;
using RealDream;
using UnityEngine;

namespace RealDream.AI
{
    public class EventUtil : MonoBehaviour
    {
        public static void RegisterEvent(object obj)
        {
            EventRegisterService.Instance.RegisterEvent(obj);
        }

        public static void UnRegisterEvent(object obj)
        {
            EventRegisterService.Instance.UnRegisterEvent(obj);
        }

        public static void Trigger(EGameEvent type, object param = null)
        {
            RealDream.EventUtil.Trigger(type, param);
        }
    }
}