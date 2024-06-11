namespace RealDream.Network
{
    public enum EMsgDefine
    {
        S2C_Hello = 101,
        C2S_Hello = 102,
        C2S_ProgressReq = 103,
        S2C_ProgressRes = 104,
        C2S_UploadFile = 111,
        S2C_SyncResult = 112,
    }
}