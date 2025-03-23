

using System.Collections.Generic;


public class RoomData
{
    public RoomType RoomType;
    public bool Pass;
    public Dictionary<int, GridData> Grids = new();
    public int x,y;
    public void Init(RoomType roomType,int x,int y)
    {
        this.RoomType = roomType;
        this.Pass = false;
        this.x = x;
        this.y = y;
    }
}

public class ItemGridData:GridData
{
    // 道具
}

public class MonsterGridData:GridData
{
    // 怪物
}

public class DestroyableItemGridData:GridData
{
    //可破坏道具
}
/// <summary>
/// 房间数据类
/// </summary>
internal class FloorData : DataModelStorageBase
{
    private Dictionary<int, RoomData> m_RoomDataDic;

}

public class GridData
{
    public int id;
    public int dataId;
    public int hp;
    public int x;
    public int y;
}