

using System.Collections.Generic;

public class RoomData
{
    public RoomType RoomType;
    public bool Pass;
}
/// <summary>
/// 房间数据类
/// </summary>
internal class FloorData : DataModelStorageBase
{
    private Dictionary<int, RoomData> m_RoomDataDic;

}
