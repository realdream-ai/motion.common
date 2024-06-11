using System.Collections;
using System.Collections.Generic;
using System.IO;
using RealDream.AI;
using EventUtil = RealDream.EventUtil;

namespace RealDream.Network
{
    public class ClientSend 
    {
        public static void SendTCPData(Packet packet)
        {
            packet.WriteLength();
            NetClient.instance.tcp.SendData(packet);
        }

        public static void WelcomeReceived()
        {
            using (Packet packet = new Packet((int)EMsgDefine.C2S_Hello))
            {
                packet.Write(NetClient.instance.myId);
                packet.Write("Hello Test");
                SendTCPData(packet);
            }
            EventUtil.Trigger(EGameEvent.OnServerConnected);
        }

        public static void OnRecvProgress(string fileName, float progress)
        {
            EventUtil.Trigger(EGameEvent.OnServerProgress, new List<object>() { fileName, progress });
        }
   
        public static void OnRecvResult(string fileName, string hash, byte[] bytes, int selectIdx)
        {
            EventUtil.Trigger(EGameEvent.OnServerResult, new List<object>() { fileName,bytes, hash });
        }
        public static void OnResService(string fileName,  byte[] bytes)
        {
            EventUtil.Trigger(EGameEvent.OnServerResService, new List<object>() { fileName,bytes });
        }

        private static int GlobalServiceId;

        public static int GetGlobalServiceId()
        {
            return GlobalServiceId++;
        }
        
        public static void ReqService(EServiceType serverType,string filePath,string prefix = "")
        {
            using (Packet packet = new Packet((int)EMsgDefine.C2S_ReqService))
            {
                var hash = HashUtil.CalcHash(filePath);
                var bytes = File.ReadAllBytes(filePath);
                packet.Write((int)serverType);
                packet.Write(prefix + Path.GetFileName(filePath));
                packet.Write(hash);
                packet.Write(bytes.Length);
                packet.Write(bytes);
                SendTCPData(packet);
            }
        }
        
        public static void UpdateLoadFile(string filePath,  int idx)
        {
            using (Packet packet = new Packet((int)EMsgDefine.C2S_UploadFile))
            {
                var hash = HashUtil.CalcHash(filePath);
                var bytes = File.ReadAllBytes(filePath);
                packet.Write(Path.GetFileName(filePath));
                packet.Write(hash);
                packet.Write(idx);
                packet.Write(bytes.Length);
                packet.Write(bytes);
                SendTCPData(packet);
            }
        }
    }
}