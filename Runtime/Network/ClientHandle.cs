using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RealDream.Network
{
    public class ClientHandle
    {
        [PacketHandler((int)EMsgDefine.S2C_Hello)]
        public static void S2C_Hello(int id,Packet packet)
        {
            string msg = packet.ReadString();
            int clientId = packet.ReadInt();

            Debug.Log($"Message from server: {msg}  id= {clientId}");
            NetClient.instance.myId = clientId;
            ClientSend.WelcomeReceived();
        }

        [PacketHandler((int)EMsgDefine.S2C_SyncResult)]
        public static void S2C_SyncResult(int id,Packet packet)
        {
            string fileName = packet.ReadString();
            string hash = packet.ReadString();
            var len = packet.ReadInt();
            var bytes = packet.ReadBytes(len);
            ClientSend.OnRecvResult(fileName, hash, bytes, 0);
        }
        

        [PacketHandler((int)EMsgDefine.S2C_ProgressRes)]
        public static void S2C_ProgressRes(int id,Packet packet)
        {
            string fileName = packet.ReadString();
            var progress = packet.ReadFloat();
            ClientSend.OnRecvProgress(fileName, progress);
        }
        
        [PacketHandler((int)EMsgDefine.S2C_ResService)]
        public static void S2C_ResService(int id,Packet packet)
        {
            string fileName = packet.ReadString();
            var len = packet.ReadInt();
            var bytes = packet.ReadBytes(len);
            ClientSend.OnResService(fileName, bytes);
        }
    }
}