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

        public static void UpdateLoadFile(string filePath,  int idx)
        {
            using (Packet packet = new Packet((int)EMsgDefine.C2S_UploadFile))
            {
                var hash = HashUtil.CalcHash(filePath);
                var bytes = File.ReadAllBytes(filePath);
                packet.Write(Path.GetFileName(filePath));
                packet.Write(Path.GetFileName(hash));
                packet.Write(idx);
                packet.Write(bytes.Length);
                packet.Write(bytes);
                SendTCPData(packet);
            }
        }
    }
}