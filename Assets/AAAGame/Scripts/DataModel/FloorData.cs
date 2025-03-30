

using cfg;
using System.Collections.Generic;
using UnityEngine;
using static CombatUnitEntity;
using UnityGameFramework.Runtime;
using System;
using Cysharp.Threading.Tasks;


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

public class MonsterGridData:GridData
{
    // 怪物
    public override UniTask<EntityLogic> OnEnter(Vector3 centerPos)
    {
        var pos = new Vector3(x, -y, 0);
        var eParams = EntityParams.Create(centerPos + pos, Vector3.zero);
        GF.LogInfo($"MonsterGridData.OnEnter pos:{centerPos + pos}");
        eParams.param = this;
        eParams.Set<VarInt32>(P_CombatFlag, (int)CombatFlag.Enemy);
        var monster = Tables.Instance.TbcombatUnit.Get(dataId);
        eParams.Set(P_DataTableRow,monster);
        return GF.Entity.ShowEntityAwait<MonsterEntity>(monster.PrefabName, Const.EntityGroup.Player, eParams);
    }
}

public class DestroyableItemGridData:GridData
{
    //可破坏道具
    public override UniTask<EntityLogic> OnEnter(Vector3 centerPos)
    {
        var pos = new Vector3(x, -y, 0);
        var eParams = EntityParams.Create(centerPos + pos, Vector3.zero);
        eParams.Set<VarInt32>(P_CombatFlag, (int)CombatFlag.Enemy);
        eParams.param = this;
        return GF.Entity.ShowEntityAwait<GridEntity>("grid", Const.EntityGroup.Level, eParams);
    }
}

public class ItemGridData : GridData
{
    //道具
    public override UniTask<EntityLogic> OnEnter(Vector3 centerPos)
    {
        var pos = new Vector3(x, -y, 0);
        var eParams = EntityParams.Create(centerPos + pos, Vector3.zero,Vector3.one * 3);
        eParams.Set<VarInt32>(P_CombatFlag, (int)CombatFlag.Enemy);
        eParams.param = this;
        return GF.Entity.ShowEntityAwait<ItemEntity>("Item", Const.EntityGroup.Level, eParams);
    }
}
// <summary>
/// 房间数据类
/// </summary>
internal class FloorData : DataModelStorageBase
{
    private Dictionary<int, RoomData> m_RoomDataDic;

}

public abstract class GridData
{
    public int id;
    public int dataId;
    public int hp;
    public int x;
    public int y;

    public abstract UniTask<EntityLogic> OnEnter(Vector3 centerPos);
}