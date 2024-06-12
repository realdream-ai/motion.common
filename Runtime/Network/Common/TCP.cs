using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace RealDream.Network
{
    public class TCP
    {
        public delegate void PacketHandler(int id, Packet packet);

        public Dictionary<int, PacketHandler> packetHandlers;
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public static int dataBufferSize = 65536;

        private string ip;
        private int port;
        private Action<int> disconnectAction;
        public int id;
        public bool isSendSync;
        public TCP()
        {
            packetHandlers = new Dictionary<int, TCP.PacketHandler>();
        }

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient client, Action<int> disconnectAction)
        {
            this.disconnectAction = disconnectAction;
            socket = client;
            IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            if (remoteEndPoint != null)
            {
                ip = remoteEndPoint.Address.ToString();
                port = remoteEndPoint.Port;
            }

            BeginSocket();
        }

        /// <summary>Attempts to connect to the server via TCP.</summary>
        public void Connect(string ip, int port, Action<int> disconnectAction)
        {
            this.disconnectAction = disconnectAction;
            this.ip = ip;
            this.port = port;
            var ipAddress = IPAddress.Parse(ip);
            socket = new TcpClient(ipAddress.AddressFamily);
            socket.BeginConnect(ip, port, ConnectCallback, socket);
            isSendSync = true;
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            if (!socket.Connected)
            {
                return;
            }

            BeginSocket();
        }

        private void BeginSocket()
        {
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;
            receiveBuffer = new byte[dataBufferSize];
            stream = socket.GetStream();
            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            MsgUtil.Log($"Connect succ {ip}:{port}");
        }

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="packet">The packet to send.</param>
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    MsgUtil.LogBytes(packet.ToArray());
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to server
                }
                else
                {
                    MsgUtil.LogError("Socket null");
                }
            }
            catch (Exception _ex)
            {
                MsgUtil.LogError($"Error sending data to server via TCP: {_ex}");
            }
        }

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int len = stream.EndRead(_result);
                if (len <= 0)
                {
                    disconnectAction?.Invoke(id);
                    return;
                }

                byte[] data = new byte[len];
                Array.Copy(receiveBuffer, data, len);

                MsgUtil.LogBytes(data);
                receivedData.Reset(HandleData(data)); // Reset receivedData if all data was handled
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                disconnectAction?.Invoke(id);
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        private bool HandleData(byte[] _data)
        {
            int packetLength = 0;

            receivedData.SetBytes(_data);
            MsgUtil.LogBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](id, packet); // Call appropriate method to handle the packet
                    }
                });

                packetLength = 0; // Reset packet length
                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }

        /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            disconnectAction = null;
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
}