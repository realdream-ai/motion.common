using System;

namespace RealDream.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        public int PacketId { get; private set; }

        public PacketHandlerAttribute(int packetId)
        {
            PacketId = packetId;
        }
    }
}