namespace RealDream.Network
{
    public enum EMsgDefine
    {
        S2C_Hello = 1,
        C2S_Hello = 2,
        C2S_ProgressReq = 3,
        S2C_ProgressRes = 4,
        C2S_UploadFile = 11,
        S2C_SyncResult = 12,
    }
}