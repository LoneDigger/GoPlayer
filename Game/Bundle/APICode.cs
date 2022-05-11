namespace Game.Bundle
{
    public enum APICode
    {
        AddBcstCode = 0,    //加入廣播
        RemoveReqCode = 1,  //移除請求
        RemoveBcstCode = 2, //移除廣播
        DegreeReqCode = 3,  //轉角請求
        DegreeAckCode = 4,  //轉角結果
        MoveBcstCode = 5,   //移動廣播
        AddAckCode = 6,     //加入回應
        RejectCode = 7,     //拒絕回應
        AddReqCode = 8,     //加入請求
        EchoReqCode = 9,    //心跳請求
        EchoAckCode = 10,   //心跳回應
        CloseCode = 11,     //關閉通知
    }
}
