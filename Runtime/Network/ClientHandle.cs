using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RealDream.Network
{
    public class ClientHandle
    {
        [PacketHandler((int)EMsgDefine.S2C_Hello)]
        public static void S2C_Hello(Packet packet)
        {
            string msg = packet.ReadString();
            int id = packet.ReadInt();

            Debug.Log($"Message from server: {msg}  id= {id}");
            NetClient.instance.myId = id;
            ClientSend.WelcomeReceived();
        }

        [PacketHandler((int)EMsgDefine.S2C_SyncResult)]
        public static void S2C_SyncResult(Packet packet)
        {
            string fileName = packet.ReadString();
            string hash = packet.ReadString();
            var len = packet.ReadInt();
            var bytes = packet.ReadBytes(len);
            ClientSend.OnRecvResult(fileName, hash, bytes, 0);
        }

        [PacketHandler((int)EMsgDefine.S2C_ProgressRes)]
        public static void S2C_ProgressRes(Packet packet)
        {
            string fileName = packet.ReadString();
            var progress = packet.ReadFloat();
            ClientSend.OnRecvProgress(fileName, progress);
        }
    }
}