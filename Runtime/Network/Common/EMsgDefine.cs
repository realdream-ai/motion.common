namespace RealDream.Network
{
        
    public enum EServiceType
    {
        Drawing = 1,
        Mocap = 2,
        Rigging = 3,
    }
    public enum EMsgDefine
    {
        S2C_Hello = 101,
        C2S_Hello = 102,
        C2S_ProgressReq = 103,
        S2C_ProgressRes = 104,
        C2S_UploadFile = 111,
        S2C_SyncResult = 112,
        C2S_ReqService = 113,
        S2C_ResService = 114,
    }
}