using System;
using System.Collections.Generic;
using System.IO;

namespace RealDream.Network
{
    public class NetClient
    {
        public static NetClient instance;

        public int myId = 0;
        public TCP tcp;

        private bool isConnected = false;

        public bool IsDebugMode;


        private string ip => Constants.ServerAddress;
        private int port => Constants.ServerPort;

        public void Awake(string ip, int port)
        {
            MsgUtil.IsDebugMode = IsDebugMode;
            Constants.ServerAddress = ip;
            Constants.ServerPort = port;
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                instance.Close();
                instance = null;
                Debug.LogError("Instance already exists, destroying object!");
            }

            Debug.Log($"addr:  {ip}:{port}");
        }


        public void Start(Type type = null)
        {
            tcp = new TCP();
            MsgUtil.RegisterPacketHandlers(typeof(ClientHandle), tcp.packetHandlers);
            isConnected = true;
            tcp.Connect(ip, port, Disconnect); // Connect tcp, udp gets connected once tcp is done
        }

        public void Close()
        {
            Disconnect();
        }

        public void Update()
        {
            ThreadManager.UpdateMain();
        }


        public void OnApplicationQuit()
        {
            Disconnect(); // Disconnect when the game is closed
        }

        /// <summary>Disconnects from the server and stops all network traffic.</summary>
        public void Disconnect(int id = 0)
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                Debug.Log("Disconnected from server.");
            }
        }
    }
}