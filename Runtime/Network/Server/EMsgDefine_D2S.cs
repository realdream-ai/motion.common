namespace RealDream.Network
{
    public enum EMsgDefine_D2S
    {
        D2S_Hello = 201,
        S2D_Hello = 202,
        S2D_Request = 211,
        D2S_SyncResult = 212,
    }

    public enum EServerType
    {
        Master = 0,
        Slave_Drawing = 1
    }
}